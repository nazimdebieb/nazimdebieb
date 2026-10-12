using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
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
        private const string UrpPackage = "com.unity.render-pipelines.universal";
        private static AddRequest _urpRequest;

        [MenuItem("Bloom Garden/P0 - Configurer le projet")]
        public static void ConfigureAll()
        {
            ConfigurePlayerSettings();
            CreateGameScene();
            InstallUrp();
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

        [MenuItem("Bloom Garden/P0 - Installer URP")]
        public static void InstallUrp()
        {
            if (GraphicsSettings.defaultRenderPipeline != null)
            {
                Debug.Log("[BloomGarden] A render pipeline is already assigned: " + GraphicsSettings.defaultRenderPipeline.name);
                return;
            }
            // No version: the Package Manager picks the one matching this editor.
            _urpRequest = Client.Add(UrpPackage);
            EditorApplication.update += WaitForUrp;
        }

        private static void WaitForUrp()
        {
            if (_urpRequest == null || !_urpRequest.IsCompleted) return;
            EditorApplication.update -= WaitForUrp;
            if (_urpRequest.Status == StatusCode.Success)
                Debug.Log("[BloomGarden] URP installed: " + _urpRequest.Result.version +
                          ". Next: Assets > Create > Rendering > URP Asset (with Universal Renderer), then assign it in Project Settings > Graphics and Quality.");
            else
                Debug.LogError("[BloomGarden] URP install failed: " + _urpRequest.Error?.message);
            _urpRequest = null;
        }
    }
}
