using System.IO;
using BloomGarden.Levels;
using BloomGarden.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace BloomGarden.EditorTools
{
    /// <summary>
    /// P3: builds the palette materials (URP Lit, GDD chapter 03 colors) and sets up the Game scene:
    /// tilted orthographic camera, soft light, GameController wired to the level catalog. Idempotent.
    /// </summary>
    public static class SceneBuilder
    {
        private const string MaterialsFolder = "Assets/_BloomGarden/Art/Materials";
        private const string PalettePath = "Assets/_BloomGarden/Settings/GardenPalette.asset";
        private const string ScenePath = "Assets/_BloomGarden/Scenes/Game.unity";

        [MenuItem("Bloom Garden/P3 - Construire la scene Game")]
        public static void Build()
        {
            // Open the scene first: opening a scene unloads unreferenced assets, which would turn
            // previously loaded palette/catalog objects into destroyed references.
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GardenPalette palette = BuildPalette();
            LevelCatalog catalog = AssetDatabase.LoadAssetAtPath<LevelCatalog>(LevelTools.CatalogPath);
            if (catalog == null) catalog = LevelTools.BuildCatalog();

            Camera camera = Object.FindAnyObjectByType<Camera>();
            if (camera == null) camera = new GameObject("Main Camera") { tag = "MainCamera" }.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 7.4f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(0xFF, 0xF9, 0xEE, 0xFF);
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 80f;
            camera.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
            Vector3 target = new Vector3(0f, 0f, -0.3f);
            camera.transform.position = target - camera.transform.forward * 30f;
            // The camera is created by code, so it has no AudioListener unless we add one: without it nothing is heard.
            if (camera.GetComponent<AudioListener>() == null) camera.gameObject.AddComponent<AudioListener>();

            Light light = Object.FindAnyObjectByType<Light>();
            if (light == null) light = new GameObject("Directional Light").AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color32(0xFF, 0xF4, 0xE0, 0xFF);
            light.intensity = 0.8f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.45f;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.46f, 0.45f, 0.43f);

            var controller = Object.FindAnyObjectByType<GameController>();
            if (controller == null) controller = new GameObject("Game").AddComponent<GameController>();
            var so = new SerializedObject(controller);
            so.FindProperty("catalog").objectReferenceValue = catalog;
            so.FindProperty("palette").objectReferenceValue = palette;
            so.FindProperty("viewCamera").objectReferenceValue = camera;
            so.ApplyModifiedPropertiesWithoutUndo();
            if (so.FindProperty("catalog").objectReferenceValue == null || so.FindProperty("palette").objectReferenceValue == null)
                throw new System.InvalidOperationException("GameController references were not assigned");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            Debug.Log("[BloomGarden] Game scene built: " + ScenePath);
        }

        public static GardenPalette BuildPalette()
        {
            Directory.CreateDirectory(MaterialsFolder);
            var palette = AssetDatabase.LoadAssetAtPath<GardenPalette>(PalettePath);
            if (palette == null)
            {
                palette = ScriptableObject.CreateInstance<GardenPalette>();
                AssetDatabase.CreateAsset(palette, PalettePath);
            }
            palette.flowers = new[]
            {
                Mat("Coral", "#EF7988"),
                Mat("Blue", "#65AEE7"),
                Mat("Yellow", "#F5C55F"),
                Mat("Purple", "#AD91DF"),
            };
            palette.soil = Mat("Soil", "#9C6B47");
            palette.grass = Mat("Grass", "#7DB866");
            palette.leaf = Mat("Leaf", "#5FAE82");
            palette.slot = Mat("Place", "#E2CFAF");
            palette.lane = Mat("Lane", "#D9B98F");
            palette.marker = Mat("Marker", "#FFFFFF");
            palette.dormant = Mat("Dormant", "#B9B2A6");
            palette.highlight = Mat("Highlight", "#5DB585");
            palette.flowerHeart = Mat("FlowerHeart", "#FFD45E");
            palette.water = Mat("Water", "#8FD3F4");
            palette.wood = Mat("Wood", "#B07C52");
            palette.woodDark = Mat("WoodDark", "#7E5636");
            palette.stone = Mat("Stone", "#C9C3B6");
            palette.earth = Mat("Earth", "#A87A55");
            palette.sand = Mat("Sand", "#E6D8BF");
            palette.blossom = Mat("Blossom", "#F6B8C8");
            EditorUtility.SetDirty(palette);
            AssetDatabase.SaveAssets();
            return palette;
        }

        private static Material Mat(string name, string hex)
        {
            string path = $"{MaterialsFolder}/M_{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }
            ColorUtility.TryParseHtmlString(hex, out Color color);
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", 0.25f);
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}
