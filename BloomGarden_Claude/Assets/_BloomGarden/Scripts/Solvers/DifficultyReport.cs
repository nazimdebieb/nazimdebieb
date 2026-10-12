using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BloomGarden.Core.Levels;
using BloomGarden.Core.Model;

namespace BloomGarden.Solvers
{
    /// <summary>Teaching band of GDD chapter 08, with the constraints a level of that band must respect.</summary>
    public sealed class PedagogyBand
    {
        public int FirstLevel { get; }
        public int LastLevel { get; }
        public string Notion { get; }
        public int MinColors { get; }
        public int MaxColors { get; }
        public int MaxLayers { get; }

        public PedagogyBand(int first, int last, string notion, int minColors, int maxColors, int maxLayers)
        {
            FirstLevel = first;
            LastLevel = last;
            Notion = notion;
            MinColors = minColors;
            MaxColors = maxColors;
            MaxLayers = maxLayers;
        }

        /// <summary>MVP bands (levels 1-20), GDD chapter 08 table.</summary>
        public static readonly IReadOnlyList<PedagogyBand> Mvp = new[]
        {
            new PedagogyBand(1, 3, "Tap, couleurs, eclosion automatique", 2, 2, 1),
            new PedagogyBand(4, 6, "Fleurs cachees dans les piles", 2, 3, 2),
            new PedagogyBand(7, 10, "Gestion des 3 places et files", 3, 3, 3),
            new PedagogyBand(11, 20, "Anticiper plusieurs coups", 3, 4, 3),
        };

        public static PedagogyBand ForLevelNumber(int number) =>
            Mvp.FirstOrDefault(b => number >= b.FirstLevel && number <= b.LastLevel);
    }

    /// <summary>Everything known about one level after validation, solving, analysis and replays.</summary>
    public sealed class LevelReportRow
    {
        public LevelDefinition Level { get; }
        public int Number { get; }
        public ValidationResult Validation { get; }
        public SolveResult Solve { get; }
        public LevelAnalysis Analysis { get; }
        public bool ReferenceReplayWins { get; }
        public bool SolverReplayWins { get; }
        public int ColorCount { get; }
        public int MaxLayers { get; }
        public int CanCount { get; }
        public PedagogyBand Band { get; }
        public IReadOnlyList<string> Warnings { get; }

        /// <summary>GDD chapter 09: a level is publishable only with a proven solution that replays to a win.</summary>
        public bool Publishable => Validation.IsValid && Solve.IsSolvable && SolverReplayWins;

        private LevelReportRow(LevelDefinition level, int number, ValidationResult validation, SolveResult solve,
            LevelAnalysis analysis, bool referenceReplayWins, bool solverReplayWins)
        {
            Level = level;
            Number = number;
            Validation = validation;
            Solve = solve;
            Analysis = analysis;
            ReferenceReplayWins = referenceReplayWins;
            SolverReplayWins = solverReplayWins;
            ColorCount = level.Cells.SelectMany(c => c).Distinct().Count();
            MaxLayers = level.Cells.Count == 0 ? 0 : level.Cells.Max(c => c.Count);
            CanCount = level.Lanes.Sum(l => l.Count);
            Band = PedagogyBand.ForLevelNumber(number);
            Warnings = BuildWarnings();
        }

        public static LevelReportRow Build(LevelDefinition level, int maxStates = LevelSolver.DefaultMaxStates)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));
            ValidationResult validation = LevelValidator.Validate(level);
            SolveResult solve = LevelSolver.Solve(level, maxStates);
            LevelAnalysis analysis = LevelAnalyzer.Analyze(level, maxStates);
            bool referenceWins = level.ReferenceSolution.Count > 0 && LevelSolver.ReplayWins(level, level.ReferenceSolution);
            bool solverWins = solve.IsSolvable && LevelSolver.ReplayWins(level, solve.SolutionLanes);
            return new LevelReportRow(level, ParseNumber(level.Id), validation, solve, analysis, referenceWins, solverWins);
        }

        /// <summary>"BG-007" -> 7; 0 when the id has no trailing number.</summary>
        public static int ParseNumber(string id)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            int i = id.Length;
            while (i > 0 && char.IsDigit(id[i - 1])) i--;
            return i < id.Length && int.TryParse(id.Substring(i), NumberStyles.None, CultureInfo.InvariantCulture, out int n) ? n : 0;
        }

        private IReadOnlyList<string> BuildWarnings()
        {
            var w = new List<string>();
            if (!Validation.IsValid) w.Add("invalid");
            if (Solve.Status == SolverStatus.Unknown) w.Add("unknown-limit");
            if (Solve.Status == SolverStatus.Unsolvable) w.Add("unsolvable");
            if (Solve.IsSolvable && !SolverReplayWins) w.Add("solver-replay-failed");
            if (Level.ReferenceSolution.Count > 0 && !ReferenceReplayWins) w.Add("reference-replay-failed");
            if (!Analysis.Complete) w.Add("analysis-truncated");
            if (Analysis.IsTrivial) w.Add("trivial");
            if (Band == null) w.Add("no-band");
            else
            {
                if (ColorCount < Band.MinColors || ColorCount > Band.MaxColors) w.Add($"band-colors:{ColorCount}");
                if (MaxLayers > Band.MaxLayers) w.Add($"band-layers:{MaxLayers}");
            }
            return w.AsReadOnly();
        }
    }

    /// <summary>CSV export of the difficulty report (P2-07). Culture-invariant, one row per level.</summary>
    public static class DifficultyReport
    {
        public static readonly string[] Columns =
        {
            "id", "title", "theme", "band", "notion", "width", "height", "colors", "maxLayers", "buds", "cans",
            "valid", "status", "shortestMoves", "statesExplored", "reachableStates", "deadlockStates",
            "decisionStates", "losingMoves", "referenceReplay", "solverReplay", "publishable", "solution", "warnings",
        };

        public static string ToCsv(IEnumerable<LevelReportRow> rows)
        {
            var sb = new StringBuilder();
            sb.Append(string.Join(",", Columns)).Append('\n');
            foreach (LevelReportRow r in rows)
            {
                var a = r.Analysis;
                var cells = new object[]
                {
                    r.Level.Id, r.Level.Title, r.Level.Theme,
                    r.Band == null ? "" : $"{r.Band.FirstLevel}-{r.Band.LastLevel}", r.Band?.Notion ?? "",
                    r.Level.Width, r.Level.Height, r.ColorCount, r.MaxLayers, r.Level.TotalBuds, r.CanCount,
                    r.Validation.IsValid, r.Solve.Status, r.Solve.ShortestMoves, r.Solve.StatesExplored,
                    a.ReachableStates, a.DeadlockStates, a.DecisionStates, a.LosingMoves,
                    r.ReferenceReplayWins, r.SolverReplayWins, r.Publishable,
                    string.Join(" ", r.Solve.SolutionLanes), string.Join(" ", r.Warnings),
                };
                sb.Append(string.Join(",", cells.Select(Escape))).Append('\n');
            }
            return sb.ToString();
        }

        private static string Escape(object value)
        {
            string s = value is bool b ? (b ? "true" : "false") : Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
            return s.IndexOfAny(new[] { ',', '"', '\n' }) >= 0 ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
        }
    }
}
