using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace BloomGarden.EditorTools
{
    /// <summary>
    /// P0 setup, reproducible from the menu "Bloom Garden": portrait player settings, URP package,
    /// and an empty orthographic Game scene registered in the build settings.
    /// Written without access to a Unity editor: to validate on first open (see Docs/QA/P0.md).
    /// </summary>
    public static class ProjectSetup
    {
        private const string ScenePath = "Assets/_BloomGarden/Scenes/Game.unity";
        private const string UrpAssetPath = "Assets/_BloomGarden/Settings/URP_Mobile.asset";
        private const string UrpRendererPath = "Assets/_BloomGarden/Settings/URP_Mobile_Renderer.asset";

        [MenuItem("Bloom Garden/P0 - Configurer le projet")]
        public static void ConfigureAll()
        {
            ConfigurePlayerSettings();
            ConfigureUrp();
            CreateGameScene();
        }

        [MenuItem("Bloom Garden/P0 - Reglages portrait uniquement")]
        public static void ConfigurePlayerSettings()
        {
            PlayerSettings.companyName = "NazimDebieb";
            PlayerSettings.productName = "Bloom Garden (Claude)";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.nazimdebieb.bloomgarden.claude");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            // P8-02: no hardware statistics sent to Unity by the player (no public API: serialized field).
            var player = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            player.FindProperty("submitAnalytics").boolValue = false;
            player.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            Debug.Log("[BloomGarden] Player settings: portrait, ids set.");
        }

        /// <summary>
        /// P4 diorama look on a mobile budget: one soft main-light shadow cascade at 1024 px over the visible garden,
        /// 2x MSAA for the round primitives. The low-end mode of P7 can turn both down.
        /// </summary>
        public static void ConfigureShadowsAndAa(UniversalRenderPipelineAsset asset)
        {
            var so = new SerializedObject(asset);
            so.FindProperty("m_MainLightShadowsSupported").boolValue = true;
            so.FindProperty("m_MainLightShadowmapResolution").intValue = 1024;
            so.FindProperty("m_ShadowDistance").floatValue = 45f;
            so.FindProperty("m_ShadowCascadeCount").intValue = 1;
            so.FindProperty("m_SoftShadowsSupported").boolValue = true;
            so.FindProperty("m_MSAA").intValue = 2;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Android player settings for test builds (P3-10): IL2CPP + ARM64 (required by Google Play),
        /// portrait only. Signing stays on Unity's debug keystore: no release key is created or stored in Git.
        /// </summary>
        [MenuItem("Bloom Garden/P3 - Reglages Android")]
        public static void ConfigureAndroid()
        {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.bundleVersion = "0.7.0";
            PlayerSettings.Android.bundleVersionCode = 8;
            PlayerSettings.Android.useCustomKeystore = false;
            // App icon rendered from the game's own pieces (StoreArt, P8-04).
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_BloomGarden/Art/Icon/AppIcon.png");
            if (icon != null)
            {
                PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { icon }, IconKind.Any);
                // Unity 6 Android uses adaptive icons only: layer 0 = background (our rendered icon), layer 1 =
                // foreground (transparent). Launchers crop to their mask shape around the centre.
                Texture2D clear = TransparentLayer();
                var kind = UnityEditor.Android.AndroidPlatformIconKind.Adaptive;
                var slots = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, kind);
                foreach (var slot in slots)
                {
                    slot.SetTexture(icon, 0);
                    slot.SetTexture(clear, 1);
                }
                PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, kind, slots);
            }
            ConfigurePlayerSettings();
            Debug.Log("[BloomGarden] Android settings: IL2CPP, ARM64, version 0.7.0 (8), debug signing.");
        }

        private static Texture2D TransparentLayer()
        {
            const string path = "Assets/_BloomGarden/Art/Icon/AppIconForeground.png";
            if (!System.IO.File.Exists(path))
            {
                var tex = new Texture2D(432, 432, TextureFormat.RGBA32, false);
                var pixels = new Color32[432 * 432];
                tex.SetPixels32(pixels);
                System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(path);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        /// <summary>
        /// P7-05: iOS player settings prepared in advance (identifier, portrait, minimum iOS, version). They are
        /// stored in ProjectSettings even without the iOS module; building requires the iOS Build Support module,
        /// then a Mac with Xcode (or a cloud build service) and an Apple developer account. See Docs/IOS.md.
        /// </summary>
        [MenuItem("Bloom Garden/P7 - Reglages iOS")]
        public static void ConfigureIos()
        {
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.nazimdebieb.bloomgarden.claude");
            PlayerSettings.iOS.targetOSVersionString = "15.0";
            PlayerSettings.iOS.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
            PlayerSettings.iOS.requiresFullScreen = true;
            PlayerSettings.iOS.hideHomeButton = false;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            ConfigurePlayerSettings();
            Debug.Log("[BloomGarden] iOS settings prepared: com.nazimdebieb.bloomgarden.claude, iOS 15.0+, portrait.");
        }

        [MenuItem("Bloom Garden/P0 - Creer la scene Game")]
        public static void CreateGameScene()
        {
            if (File.Exists(ScenePath))
            {
                Debug.Log("[BloomGarden] Game scene already exists: " + ScenePath);
            }
            else
            {
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                var cameraGo = new GameObject("Main Camera") { tag = "MainCamera" };
                var camera = cameraGo.AddComponent<Camera>();
                camera.orthographic = true;
                camera.orthographicSize = 9.6f; // 1080x1920 reference at 100 px/unit
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color32(0xFF, 0xF9, 0xEE, 0xFF); // ivory #FFF9EE
                cameraGo.transform.position = new Vector3(0f, 0f, -10f);

                var lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

                EditorSceneManager.SaveScene(scene, ScenePath);
                Debug.Log("[BloomGarden] Created " + ScenePath);
            }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        /// <summary>
        /// URP is declared in Packages/manifest.json; this creates the mobile pipeline asset and assigns it
        /// to Graphics and every Quality level. Synchronous, so it also runs in batch mode.
        /// </summary>
        [MenuItem("Bloom Garden/P0 - Configurer URP")]
        public static void ConfigureUrp()
        {
            var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(UrpAssetPath);
            if (asset == null)
            {
                var renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, UrpRendererPath);
                asset = UniversalRenderPipelineAsset.Create(renderer);
                asset.supportsHDR = false;
                AssetDatabase.CreateAsset(asset, UrpAssetPath);
            }
            ConfigureShadowsAndAa(asset);
            GraphicsSettings.defaultRenderPipeline = asset;
            int current = QualitySettings.GetQualityLevel();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = asset;
            }
            QualitySettings.SetQualityLevel(current, false);
            AssetDatabase.SaveAssets();
            Debug.Log("[BloomGarden] URP assigned: " + UrpAssetPath);
        }
    }
}
