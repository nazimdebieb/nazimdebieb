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
            AssetDatabase.SaveAssets();
            Debug.Log("[BloomGarden] Player settings: portrait, ids set.");
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
                // Mobile budget: no HDR, no MSAA, no shadows needed for a flat 2.5D puzzle board.
                asset.supportsHDR = false;
                asset.msaaSampleCount = 1;
                asset.shadowDistance = 0f;
                AssetDatabase.CreateAsset(asset, UrpAssetPath);
            }
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
