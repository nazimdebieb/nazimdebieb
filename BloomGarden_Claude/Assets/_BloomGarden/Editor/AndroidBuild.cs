using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BloomGarden.EditorTools
{
    /// <summary>
    /// P3-10: test APK (non-development build, so no Pipeline debug server inside; Unity debug signing).
    /// Output: Builds/ (ignored by Git). Summary written to Docs/QA/P3_android_build.txt.
    /// Batch mode: -executeMethod BloomGarden.EditorTools.AndroidBuild.BuildTestApk
    /// </summary>
    public static class AndroidBuild
    {
        private const string Scene = "Assets/_BloomGarden/Scenes/Game.unity";

        [MenuItem("Bloom Garden/P3 - Build APK de test")]
        public static string BuildTestApk()
        {
            ProjectSetup.ConfigureAndroid();
            EditorUserBuildSettings.buildAppBundle = false;

            string root = Directory.GetParent(Application.dataPath).FullName;
            string output = Path.Combine(root, "Builds", $"BloomGarden_Claude_{PlayerSettings.bundleVersion}.apk");
            Directory.CreateDirectory(Path.GetDirectoryName(output));

            var options = new BuildPlayerOptions
            {
                scenes = new[] { Scene },
                locationPathName = output,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.None,
            };
            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary s = report.summary;

            var log = new StringBuilder();
            log.AppendLine($"Bloom Garden (Claude) - Android test build - Unity {Application.unityVersion}");
            log.AppendLine($"result={s.result} platform={s.platform} output={Path.GetFileName(s.outputPath)}");
            log.AppendLine($"size={s.totalSize / (1024f * 1024f):0.0} MB time={s.totalTime.TotalSeconds:0}s errors={s.totalErrors} warnings={s.totalWarnings}");
            log.AppendLine($"version={PlayerSettings.bundleVersion} ({PlayerSettings.Android.bundleVersionCode}) id={PlayerSettings.GetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android)}");
            log.AppendLine("backend=IL2CPP arch=ARM64 development=false signing=Unity debug keystore");
            foreach (var step in report.steps)
                foreach (var m in step.messages.Where(m => m.type == LogType.Error || m.type == LogType.Exception))
                    log.AppendLine($"ERROR [{step.name}] {m.content}");
            File.WriteAllText(Path.Combine(root, "Docs", "QA", "P3_android_build.txt"), log.ToString(), new UTF8Encoding(false));
            Debug.Log("[BloomGarden] " + log.ToString().Replace("\n", " | "));
            return $"{s.result} {s.totalSize / (1024f * 1024f):0.0}MB {s.totalTime.TotalSeconds:0}s errors={s.totalErrors}";
        }
    }
}
