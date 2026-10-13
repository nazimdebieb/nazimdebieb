using System.Collections.Generic;
using System.IO;
using System.Linq;
using BloomGarden.Core.Json;
using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;
using NUnit.Framework;

namespace BloomGarden.Tests
{
    /// <summary>
    /// Compares the C# TurnResolver against traces recorded from Kit/tools/solver_reference.py
    /// (Tools/python/generate_oracle_traces.py): reference solutions + seeded random playouts on the 20 levels.
    /// After every move the bloomed counts, lane heads, active cans (oldest first) and end status must match.
    /// </summary>
    public class PythonOracleTests
    {
        [Test]
        public void CSharpResolver_MatchesPythonReference_StepByStep()
        {
            string path = Path.Combine(TestLevels.FixturesDirectory, "python_oracle_traces.json");
            var root = (Dictionary<string, object>)MiniJson.Parse(File.ReadAllText(path));
            var traces = (List<object>)root["traces"];
            var levels = TestLevels.AllReferenceLevels().ToDictionary(l => l.Id);

            int moves = 0;
            foreach (Dictionary<string, object> trace in traces)
            {
                LevelDefinition level = levels[(string)trace["level"]];
                string label = $"{level.Id}/{trace["run"]}";
                var session = new GameSession(level);
                var steps = (List<object>)trace["steps"];
                for (int i = 0; i < steps.Count; i++)
                {
                    var step = (Dictionary<string, object>)steps[i];
                    string where = $"{label} step {i}";
                    MoveResult r = session.SelectLane((int)(long)step["lane"]);
                    Assert.IsTrue(r.Accepted, where + " rejected: " + r.Rejection);

                    GameState s = r.State;
                    CollectionAssert.AreEqual(Ints(step["bloomed"]), Enumerable.Range(0, s.CellCount).Select(s.BloomedInCell).ToList(), where + " bloomed");
                    CollectionAssert.AreEqual(Ints(step["laneHeads"]), Enumerable.Range(0, s.LaneCount).Select(s.LaneHead).ToList(), where + " lanes");

                    var expectedActive = ((List<object>)step["active"]).Cast<List<object>>().Select(a => (string)a[0] + ":" + (long)a[1]).ToList();
                    var actualActive = Enumerable.Range(0, s.SlotCount).Select(s.GetSlot).Where(x => x.IsOccupied)
                        .OrderBy(x => x.Order).Select(x => x.CanId + ":" + x.Remaining).ToList();
                    CollectionAssert.AreEqual(expectedActive, actualActive, where + " active cans");

                    Assert.AreEqual((bool)step["finished"], s.IsWon, where + " won");
                    Assert.AreEqual((bool)step["stuck"], s.IsDeadlocked, where + " deadlock");
                    moves++;
                }
            }
            Assert.Greater(moves, 1000, "fixture should cover the 20 levels with many playouts");
            TestContext.WriteLine($"{traces.Count} traces, {moves} moves compared with the Python reference");
        }

        private static List<int> Ints(object array) => ((List<object>)array).Select(x => (int)(long)x).ToList();
    }
}
