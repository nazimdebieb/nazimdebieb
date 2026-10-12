using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using BloomGarden.Core.Levels;
using BloomGarden.Core.Model;
using BloomGarden.Levels;
using BloomGarden.Solvers;
using UnityEditor;
using UnityEngine;

namespace BloomGarden.EditorTools
{
    /// <summary>
    /// P2 tooling: level catalog asset and difficulty report.
    /// Batch mode: Unity -batchmode -quit -projectPath . -executeMethod BloomGarden.EditorTools.LevelTools.BuildCatalogAndReport
    /// (exit code 1 when a level is not publishable).
    /// </summary>
    public static class LevelTools
    {
        public const string LevelsFolder = "Assets/_BloomGarden/Levels";
        public const string CatalogPath = "Assets/_BloomGarden/ScriptableObjects/LevelCatalog.asset";
        private const string ReportCsv = "Docs/QA/P2_difficulty_report.csv";
        private const string ReportLog = "Docs/QA/P2_solver_log.txt";

        [MenuItem("Bloom Garden/P2 - Construire le catalogue de niveaux")]
        public static LevelCatalog BuildCatalog()
        {
            var files = AssetDatabase.FindAssets("t:TextAsset BG-", new[] { LevelsFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => Path.GetFileName(p).StartsWith("BG-") && p.EndsWith(".json"))
                .OrderBy(p => p, System.StringComparer.Ordinal)
                .Select(AssetDatabase.LoadAssetAtPath<TextAsset>)
                .ToList();

            var catalog = AssetDatabase.LoadAssetAtPath<LevelCatalog>(CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<LevelCatalog>();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }
            catalog.SetFiles(files);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            Debug.Log($"[BloomGarden] Level catalog: {files.Count} files -> {CatalogPath}");
            return catalog;
        }

        [MenuItem("Bloom Garden/P2 - Exporter le rapport de difficulte")]
        public static bool ExportReport()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<LevelCatalog>(CatalogPath) ?? BuildCatalog();
            var rows = new List<LevelReportRow>();
            var log = new StringBuilder();
            log.Append("Bloom Garden - P2 solver log (rules ").Append(LevelDefinition.SupportedRulesVersion)
               .Append(", Unity ").Append(Application.unityVersion).Append(")\n");

            bool allPublishable = true;
            for (int i = 0; i < catalog.Count; i++)
            {
                string file = catalog.Files[i] != null ? catalog.Files[i].name : "(missing)";
                if (!catalog.TryLoad(i, out LevelDefinition level, out ValidationResult result))
                {
                    allPublishable = false;
                    log.Append(file).Append(": INVALID ").Append(result).Append('\n');
                    continue;
                }
                var watch = System.Diagnostics.Stopwatch.StartNew();
                LevelReportRow row = LevelReportRow.Build(level);
                watch.Stop();
                rows.Add(row);
                allPublishable &= row.Publishable;
                log.AppendFormat(CultureInfo.InvariantCulture,
                    "{0}: validation={1} status={2} moves={3} explored={4} reachable={5} deadlocks={6} losingMoves={7} " +
                    "referenceReplay={8} solverReplay={9} publishable={10} warnings=[{11}] time={12}ms\n",
                    level.Id, row.Validation, row.Solve.Status, row.Solve.ShortestMoves, row.Solve.StatesExplored,
                    row.Analysis.ReachableStates, row.Analysis.DeadlockStates, row.Analysis.LosingMoves,
                    row.ReferenceReplayWins, row.SolverReplayWins, row.Publishable, string.Join(" ", row.Warnings),
                    watch.ElapsedMilliseconds);
            }
            log.Append(allPublishable ? "RESULT: all levels publishable\n" : "RESULT: at least one level NOT publishable\n");

            string root = Directory.GetParent(Application.dataPath).FullName;
            File.WriteAllText(Path.Combine(root, ReportCsv), DifficultyReport.ToCsv(rows), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(root, ReportLog), log.ToString(), new UTF8Encoding(false));
            Debug.Log($"[BloomGarden] Difficulty report: {rows.Count} levels -> {ReportCsv}; publishable={allPublishable}");
            return allPublishable;
        }

        public static void BuildCatalogAndReport()
        {
            BuildCatalog();
            bool ok = ExportReport();
            if (Application.isBatchMode) EditorApplication.Exit(ok ? 0 : 1);
        }
    }
}
