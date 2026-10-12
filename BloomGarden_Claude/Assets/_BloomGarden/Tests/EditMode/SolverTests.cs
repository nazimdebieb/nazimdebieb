using System.Collections.Generic;
using System.IO;
using System.Linq;
using BloomGarden.Core.Json;
using BloomGarden.Core.Model;
using BloomGarden.Solvers;
using NUnit.Framework;

namespace BloomGarden.Tests
{
    /// <summary>
    /// P2: C# solver and analyzer against the Python reference (Tools/python/generate_solver_fixture.py),
    /// replays of every known solution, and the limit cases (unsolvable, unknown).
    /// </summary>
    public class SolverTests
    {
        private static Dictionary<string, Dictionary<string, object>> LoadPythonReference()
        {
            string path = Path.Combine(TestLevels.FixturesDirectory, "python_solver_reference.json");
            var root = (Dictionary<string, object>)MiniJson.Parse(File.ReadAllText(path));
            return ((List<object>)root["levels"]).Cast<Dictionary<string, object>>()
                .ToDictionary(r => (string)r["level"]);
        }

        private static int Int(Dictionary<string, object> record, string key) => (int)(long)record[key];

        // P2-03 / P2-04: same status, same shortest solution, same number of explored states as the Python BFS.
        [Test]
        public void Solver_MatchesPythonReference_OnAllLevels()
        {
            var reference = LoadPythonReference();
            var levels = TestLevels.AllReferenceLevels().ToList();
            Assert.AreEqual(20, levels.Count);
            Assert.AreEqual(20, reference.Count);

            foreach (LevelDefinition level in levels)
            {
                var expected = reference[level.Id];
                SolveResult result = LevelSolver.Solve(level);
                Assert.AreEqual("solvable", (string)expected["status"], level.Id);
                Assert.AreEqual(SolverStatus.Solvable, result.Status, level.Id);
                var expectedLanes = ((List<object>)expected["solutionLanes"]).Select(x => (int)(long)x).ToList();
                CollectionAssert.AreEqual(expectedLanes, result.SolutionLanes, level.Id + " solution");
                CollectionAssert.AreEqual(level.ReferenceSolution, result.SolutionLanes, level.Id + " vs referenceSolution");
                Assert.AreEqual(Int(expected, "statesExplored"), result.StatesExplored, level.Id + " statesExplored");
                Assert.AreEqual(Int(expected, "shortestMoves"), result.ShortestMoves, level.Id + " shortestMoves");
            }
        }

        [Test]
        public void Analyzer_MatchesPythonReference_OnAllLevels()
        {
            var reference = LoadPythonReference();
            foreach (LevelDefinition level in TestLevels.AllReferenceLevels())
            {
                var expected = reference[level.Id];
                LevelAnalysis a = LevelAnalyzer.Analyze(level);
                Assert.IsTrue(a.Complete, level.Id);
                Assert.AreEqual(Int(expected, "reachableStates"), a.ReachableStates, level.Id + " reachable");
                Assert.AreEqual(Int(expected, "winStates"), a.WinStates, level.Id + " wins");
                Assert.AreEqual(Int(expected, "deadlockStates"), a.DeadlockStates, level.Id + " deadlocks");
                Assert.AreEqual(Int(expected, "decisionStates"), a.DecisionStates, level.Id + " decisions");
                Assert.AreEqual(Int(expected, "losingMoves"), a.LosingMoves, level.Id + " losingMoves");
            }
        }

        // P2-04: the 20 referenceSolution and the 20 solver solutions replay to a win through GameSession.
        [Test]
        public void ReferenceAndSolverSolutions_ReplayToWin()
        {
            foreach (LevelDefinition level in TestLevels.AllReferenceLevels())
            {
                Assert.IsTrue(LevelSolver.ReplayWins(level, level.ReferenceSolution), level.Id + " referenceSolution");
                Assert.IsTrue(LevelSolver.ReplayWins(level, LevelSolver.Solve(level).SolutionLanes), level.Id + " solver solution");
            }
        }

        [Test]
        public void Solver_SmallLevel_IsSolvable()
        {
            LevelDefinition level = TestLevels.Build(2, 1, new[] { "c", "b" }, new[] { "c1:coral:1", "b1:blue:1", "" });
            SolveResult r = LevelSolver.Solve(level);
            Assert.AreEqual(SolverStatus.Solvable, r.Status);
            CollectionAssert.AreEqual(new[] { 0, 1 }, r.SolutionLanes);
            Assert.IsTrue(LevelSolver.ReplayWins(level, r.SolutionLanes));
        }

        // P2-05: one lane only, three blue cans arrive before the coral ones while coral hides every blue bud.
        [Test]
        public void Solver_ForcedDeadlock_IsUnsolvable_NotUnknown()
        {
            LevelDefinition level = TestLevels.Build(2, 1, new[] { "cbb", "cb" },
                new[] { "b1:blue:1 b2:blue:1 b3:blue:1 c1:coral:1 c2:coral:1", "", "" });
            Assert.IsTrue(BloomGarden.Core.Levels.LevelValidator.Validate(level).IsValid);

            SolveResult r = LevelSolver.Solve(level);
            Assert.AreEqual(SolverStatus.Unsolvable, r.Status);
            Assert.IsFalse(r.IsSolvable);
            Assert.AreEqual(-1, r.ShortestMoves);
            CollectionAssert.IsEmpty(r.SolutionLanes);

            LevelAnalysis a = LevelAnalyzer.Analyze(level);
            Assert.IsTrue(a.Complete);
            Assert.AreEqual(0, a.WinStates);
            Assert.AreEqual(1, a.DeadlockStates);
        }

        // P2-05: hitting the state limit must answer Unknown, never Solvable, even on a solvable level.
        [Test]
        public void Solver_StateLimit_ReturnsUnknown_NeverSolvable()
        {
            LevelDefinition level = TestLevels.LoadReference("BG-020");
            SolveResult r = LevelSolver.Solve(level, maxStates: 10);
            Assert.AreEqual(SolverStatus.Unknown, r.Status);
            Assert.IsFalse(r.IsSolvable);
            Assert.AreEqual(10, r.StatesExplored);
            CollectionAssert.IsEmpty(r.SolutionLanes);

            LevelAnalysis a = LevelAnalyzer.Analyze(level, maxStates: 10);
            Assert.IsFalse(a.Complete);
            Assert.IsFalse(a.IsTrivial, "a truncated analysis cannot claim the level is trivial");

            LevelReportRow row = LevelReportRow.Build(level, maxStates: 10);
            Assert.IsFalse(row.Publishable);
            CollectionAssert.Contains(row.Warnings, "unknown-limit");
        }

        [Test]
        public void Analyzer_FindsTheTrapsOfBg018()
        {
            LevelAnalysis a = LevelAnalyzer.Analyze(TestLevels.LoadReference("BG-018"));
            Assert.IsFalse(a.IsTrivial);
            Assert.Greater(a.LosingMoves, 0);
            Assert.IsTrue(LevelAnalyzer.Analyze(TestLevels.LoadReference("BG-001")).IsTrivial);
        }

        // P2-06 / P2-07: every reference level is publishable and fits the teaching band of GDD chapter 08.
        [Test]
        public void Report_AllLevelsPublishable_AndInTheirTeachingBand()
        {
            var rows = TestLevels.AllReferenceLevels().Select(l => LevelReportRow.Build(l)).ToList();
            foreach (LevelReportRow row in rows)
            {
                Assert.IsTrue(row.Publishable, row.Level.Id + ": " + string.Join(" ", row.Warnings));
                Assert.IsNotNull(row.Band, row.Level.Id);
                Assert.IsFalse(row.Warnings.Any(w => w.StartsWith("band-")), row.Level.Id + ": " + string.Join(" ", row.Warnings));
            }

            string csv = DifficultyReport.ToCsv(rows);
            string[] lines = csv.TrimEnd('\n').Split('\n');
            Assert.AreEqual(21, lines.Length);
            Assert.AreEqual(string.Join(",", DifficultyReport.Columns), lines[0]);
            StringAssert.StartsWith("BG-001,", lines[1]);
        }

        [TestCase("BG-007", 7)]
        [TestCase("BG-020", 20)]
        [TestCase("TEST", 0)]
        public void ParseNumber_ReadsTrailingDigits(string id, int expected) =>
            Assert.AreEqual(expected, LevelReportRow.ParseNumber(id));
    }
}
