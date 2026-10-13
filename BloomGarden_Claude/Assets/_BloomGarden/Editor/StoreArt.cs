using System.IO;
using BloomGarden.Core.Model;
using BloomGarden.Presentation;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

namespace BloomGarden.EditorTools
{
    /// <summary>
    /// P8-04: renders the app icon and the store feature graphic from the game's own pieces (bloom, can, soil),
    /// so store art matches the real game. Run in Play mode (the garden pieces use runtime-only calls):
    /// <c>StoreArt.RenderAll()</c>. Writes Docs/Store/*.png and Assets/_BloomGarden/Art/Icon/AppIcon.png,
    /// then sets it as the default app icon.
    /// </summary>
    public static class StoreArt
    {
        private const string IconAsset = "Assets/_BloomGarden/Art/Icon/AppIcon.png";
        private static readonly Vector3 Stage = new Vector3(500f, 0f, 0f);

        public static string RenderAll()
        {
            var palette = AssetDatabase.LoadAssetAtPath<GardenPalette>("Assets/_BloomGarden/Settings/GardenPalette.asset");
            string root = Directory.GetParent(Application.dataPath).FullName;
            string store = Path.Combine(root, "Docs", "Store");
            Directory.CreateDirectory(store);

            var stage = new GameObject("Store art stage");
            stage.transform.position = Stage;
            try
            {
                BuildIconScene(palette, stage.transform);
                byte[] icon = Render(stage.transform, 1024, 1024, new Vector3(0f, 3.1f, -3.6f), new Vector3(0.1f, 0.55f, 0f), 34f, null);
                Directory.CreateDirectory(Path.Combine(root, Path.GetDirectoryName(IconAsset)));
                File.WriteAllBytes(Path.Combine(root, IconAsset), icon);
                File.WriteAllBytes(Path.Combine(store, "icon_1024.png"), icon);
                File.WriteAllBytes(Path.Combine(store, "icon_512.png"),
                    Render(stage.transform, 512, 512, new Vector3(0f, 3.1f, -3.6f), new Vector3(0.1f, 0.55f, 0f), 34f, null));

                // Destroy is deferred to the end of the frame: clear the icon scene immediately before the next render.
                for (int i = stage.transform.childCount - 1; i >= 0; i--) Object.DestroyImmediate(stage.transform.GetChild(i).gameObject);
                BuildFeatureScene(palette, stage.transform);
                File.WriteAllBytes(Path.Combine(store, "feature_graphic_1024x500.png"),
                    Render(stage.transform, 1024, 500, new Vector3(0f, 3.9f, -6.6f), new Vector3(0f, -0.1f, 0.4f), 32f, Loc.T("app_title")));
            }
            finally
            {
                Object.Destroy(stage);
            }

            AssetDatabase.ImportAsset(IconAsset);
            var importer = (TextureImporter)AssetImporter.GetAtPath(IconAsset);
            importer.textureType = TextureImporterType.Default;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(IconAsset);
            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { tex }, IconKind.Any);
            AssetDatabase.SaveAssets();
            return "icon + feature graphic written to Docs/Store";
        }

        private static void BuildIconScene(GardenPalette palette, Transform parent)
        {
            Disc(parent, palette.grass, Vector3.down * 0.12f, 3.6f, 0.1f);
            Disc(parent, palette.soil, Vector3.down * 0.04f, 1.9f, 0.06f);
            var bloom = FlowerFactory.Bloom(ColorId.Coral, palette, parent);
            bloom.transform.localPosition = new Vector3(-0.25f, 0f, 0.15f);
            bloom.transform.localScale = Vector3.one * 2.2f;
            var can = CanView.Create(new CanDefinition("icon", ColorId.Blue, 3), palette, parent);
            can.gameObject.SetActive(true);
            can.transform.localPosition = new Vector3(0.95f, 0.55f, -0.35f);
            can.transform.localRotation = Quaternion.Euler(0f, 200f, -26f);
            can.transform.localScale = Vector3.one * 1.05f;
            for (int i = 0; i < 4; i++)
            {
                var drop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.Destroy(drop.GetComponent<Collider>());
                drop.transform.SetParent(parent, false);
                drop.transform.localPosition = new Vector3(0.25f - i * 0.18f, 1.45f - i * 0.12f + i * i * 0.02f, -0.15f);
                drop.transform.localScale = new Vector3(0.12f, 0.16f, 0.12f);
                drop.GetComponent<Renderer>().sharedMaterial = palette.water;
            }
        }

        private static void BuildFeatureScene(GardenPalette palette, Transform parent)
        {
            Disc(parent, palette.grass, Vector3.down * 0.12f, 14f, 0.1f);
            ColorId[] colors = { ColorId.Coral, ColorId.Blue, ColorId.Yellow, ColorId.Purple };
            for (int i = 0; i < 9; i++)
            {
                float x = (i - 4) * 1.15f;
                float z = 0.6f + Mathf.Abs(i - 4) * 0.18f;
                Disc(parent, palette.soil, new Vector3(x, -0.04f, z), 0.95f, 0.05f);
                GameObject piece = i % 2 == 0 ? FlowerFactory.Bloom(colors[i / 2 % 4], palette, parent) : FlowerFactory.Bud(colors[(i + 1) % 4], palette, parent);
                piece.transform.localPosition = new Vector3(x, 0f, z);
                piece.transform.localScale = Vector3.one * 1.3f;
            }
            for (int i = 0; i < 4; i++)
            {
                var can = CanView.Create(new CanDefinition("feature" + i, colors[i], 3 - i % 2), palette, parent);
                can.gameObject.SetActive(true);
                can.transform.localPosition = new Vector3((i - 1.5f) * 1.25f, 0f, -0.9f);
                can.transform.localScale = Vector3.one * 0.75f;
            }
        }

        private static void Disc(Transform parent, Material material, Vector3 position, float diameter, float height)
        {
            var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.Destroy(disc.GetComponent<Collider>());
            disc.transform.SetParent(parent, false);
            disc.transform.localPosition = position;
            disc.transform.localScale = new Vector3(diameter, height, diameter);
            disc.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static byte[] Render(Transform stage, int width, int height, Vector3 cameraOffset, Vector3 lookOffset, float fov, string title)
        {
            var camGo = new GameObject("Store camera");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color32(0xFF, 0xF1, 0xD6, 0xFF);
            cam.fieldOfView = fov;
            cam.transform.position = stage.position + cameraOffset;
            cam.transform.LookAt(stage.position + lookOffset);
            cam.allowMSAA = true;

            GameObject canvasGo = null;
            if (!string.IsNullOrEmpty(title))
            {
                canvasGo = new GameObject("Store title", typeof(Canvas), typeof(CanvasScaler));
                var canvas = canvasGo.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = cam;
                canvas.planeDistance = 1f;
                var text = new GameObject("Title", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
                text.transform.SetParent(canvasGo.transform, false);
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                text.text = title;
                text.fontSize = 86;
                text.fontStyle = FontStyle.Bold;
                text.alignment = TextAnchor.UpperCenter;
                text.color = new Color32(0x25, 0x48, 0x3B, 0xFF);
                var rt = text.rectTransform;
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0f, -24f);
                rt.sizeDelta = new Vector2(0f, 120f);
            }

            var rtTex = new RenderTexture(width, height, 24) { antiAliasing = 4 };
            cam.targetTexture = rtTex;
            cam.Render();
            RenderTexture.active = rtTex;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            cam.targetTexture = null;
            byte[] png = tex.EncodeToPNG();
            Object.Destroy(tex);
            rtTex.Release();
            Object.Destroy(rtTex);
            Object.Destroy(camGo);
            if (canvasGo != null) Object.Destroy(canvasGo);
            return png;
        }
    }
}
