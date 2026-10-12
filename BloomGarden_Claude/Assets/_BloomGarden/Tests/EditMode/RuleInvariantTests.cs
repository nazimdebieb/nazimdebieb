using System.Collections.Generic;
using System.Linq;
using BloomGarden.Core.Levels;
using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;
using NUnit.Framework;

namespace BloomGarden.Tests
{
    /// <summary>The 15 EditMode invariants of GDD chapter 16, numbered as in the GDD (I01..I15).</summary>
    public class RuleInvariantTests
    {
        // Three cans of the wrong colors fill the places, coral can never be placed.
        private static LevelDefinition DeadlockLevel() => TestLevels.Build(3, 1,
            new[] { "cb", "cy", "cp" },
            new[] { "b1:blue:1 y1:yellow:1 p1:purple:1", "c1:coral:3", "" });

        // Third selection fills all places, but its cascade frees two of them: not a deadlock.
        private static LevelDefinition CascadeLevel() => TestLevels.Build(2, 1,
            new[] { "cb", "yp" },
            new[] { "b1:blue:1 p1:purple:1 c1:coral:1", "y1:yellow:1", "" });

        [Test]
        public void I01_SelectingEmptyLane_IsRejected_AndStateUnchanged()
        {
            LevelDefinition level = TestLevels.LoadReference("BG-001"); // lane 2 is empty
            GameState state = GameState.CreateInitial(level);
            ulong before = state.ComputeHash();

            MoveResult result = TurnResolver.SelectLane(level, state, 2);

            Assert.AreEqual(MoveRejection.LaneEmpty, result.Rejection);
            Assert.AreSame(state, result.State);
            Assert.IsEmpty(result.Events);
            Assert.AreEqual(before, state.ComputeHash());
            Assert.AreEqual(MoveRejection.InvalidLane, TurnResolver.SelectLane(level, state, 3).Rejection);
            Assert.AreEqual(MoveRejection.InvalidLane, TurnResolver.SelectLane(level, state, -1).Rejection);
        }

        [TestCase("0")]
        [TestCase("-2")]
        [TestCase("2.5")]
        [TestCase("\"3\"")]
        public void I02_NonPositiveOrNonIntegerCapacity_IsRejectedAtLoad(string capacity)
        {
            string json = "{\"id\":\"X\",\"rulesVersion\":\"0.1.0\",\"slotCount\":3," +
                          "\"grid\":{\"width\":1,\"height\":1,\"cells\":[[\"coral\"]]}," +
                          "\"lanes\":[[{\"id\":\"c\",\"color\":\"coral\",\"capacity\":" + capacity + "}],[],[]]}";

            Assert.IsFalse(LevelLoader.TryLoad(json, out LevelDefinition level, out ValidationResult result));
            Assert.IsNull(level);
            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void I03_CanNeverWatersAnotherColor()
        {
            foreach (LevelDefinition level in TestLevels.AllReferenceLevels())
            {
                var session = new GameSession(level);
                var events = new List<GameEvent>();
                Assert.IsTrue(session.Replay(level.ReferenceSolution, events), level.Id);
                foreach (GameEvent e in events.Where(e => e.Type == GameEventType.FlowerBloomed))
                {
                    Assert.AreEqual(e.Color, level.Cells[e.CellIndex][e.Layer], $"{level.Id}: {e}");
                    Assert.AreEqual(e.Color, CanColor(level, e.CanId), $"{level.Id}: {e}");
                }
            }
        }

        [Test]
        public void I04_HiddenBud_IsNotTargetedBeforeReveal()
        {
            LevelDefinition level = TestLevels.Build(1, 1, new[] { "cb" }, new[] { "b1:blue:1", "c1:coral:1", "" });
            GameState state = GameState.CreateInitial(level);
            Assert.IsTrue(state.TryGetVisibleBud(level, 0, out ColorId visible));
            Assert.AreEqual(ColorId.Coral, visible);

            MoveResult result = TurnResolver.SelectLane(level, state, 0);

            Assert.IsTrue(result.Accepted);
            Assert.AreEqual(0, result.State.BloomedInCell(0), "blue bud is hidden under coral");
            Assert.IsFalse(result.Events.Any(e => e.Type == GameEventType.FlowerBloomed));
        }

        [Test]
        public void I05_Resolution_IsDeterministic_ForSameInput()
        {
            foreach (LevelDefinition level in TestLevels.AllReferenceLevels())
            {
                string first = PlayTranscript(level, level.ReferenceSolution);
                string second = PlayTranscript(level, level.ReferenceSolution);
                Assert.AreEqual(first, second, level.Id);
            }
        }

        [Test]
        public void I06_Can_LeavesItsPlace_WhenEmpty()
        {
            LevelDefinition level = TestLevels.LoadReference("BG-001");
            MoveResult result = TurnResolver.SelectLane(level, GameState.CreateInitial(level), 0);

            Assert.IsFalse(result.State.GetSlot(0).IsOccupied);
            Assert.AreEqual(GameEventType.CanEmptied, result.Events.Last().Type);
            Assert.AreEqual(3, result.Events.Count(e => e.Type == GameEventType.FlowerBloomed));
        }

        [Test]
        public void I07_CanWithoutTarget_WaitsAndKeepsItsPlace()
        {
            LevelDefinition level = TestLevels.Build(1, 1, new[] { "cb" }, new[] { "b1:blue:1", "c1:coral:1", "" });
            GameState state = TurnResolver.SelectLane(level, GameState.CreateInitial(level), 0).State;

            SlotState slot = state.GetSlot(0);
            Assert.IsTrue(slot.IsOccupied);
            Assert.AreEqual("b1", slot.CanId);
            Assert.AreEqual(1, slot.Remaining);
            Assert.AreEqual(GameStatus.InProgress, state.Status);
        }

        [Test]
        public void I08_ThreeOccupiedPlaces_BlockAFourthDeparture()
        {
            LevelDefinition level = DeadlockLevel();
            var session = new GameSession(level);
            Assert.IsTrue(session.Replay(new[] { 0, 0, 0 }));
            Assert.AreEqual(3, session.State.OccupiedSlotCount);
            ulong before = session.State.ComputeHash();

            MoveResult fourth = session.SelectLane(1);

            Assert.AreEqual(MoveRejection.SlotsFull, fourth.Rejection);
            Assert.AreEqual(0, session.State.LaneHead(1));
            Assert.AreEqual(before, session.State.ComputeHash());
        }

        [Test]
        public void I09_WaitingCans_AreReevaluated_AfterReveal()
        {
            LevelDefinition level = CascadeLevel();
            var session = new GameSession(level);
            session.SelectLane(0); // blue waits (hidden under coral)
            session.SelectLane(0); // purple waits (hidden under yellow)
            MoveResult coral = session.SelectLane(0);

            var blooms = coral.Events.Where(e => e.Type == GameEventType.FlowerBloomed).ToList();
            Assert.AreEqual(2, blooms.Count);
            Assert.AreEqual("c1", blooms[0].CanId);
            Assert.AreEqual("b1", blooms[1].CanId, "the waiting blue can waters the revealed blue bud");
            Assert.AreEqual(0, blooms[1].CellIndex);
            Assert.AreEqual(1, blooms[1].Layer);
        }

        [Test]
        public void I10_Undo_RestoresExactPreviousState()
        {
            foreach (LevelDefinition level in TestLevels.AllReferenceLevels())
            {
                var session = new GameSession(level);
                ulong initial = session.State.ComputeHash();
                foreach (int lane in level.ReferenceSolution)
                {
                    ulong before = session.State.ComputeHash();
                    Assert.IsTrue(session.SelectLane(lane).Accepted);
                    ulong after = session.State.ComputeHash();
                    Assert.AreNotEqual(before, after);
                    Assert.IsTrue(session.Undo());
                    Assert.AreEqual(before, session.State.ComputeHash(), level.Id);
                    session.SelectLane(lane);
                    Assert.AreEqual(after, session.State.ComputeHash(), level.Id + " redo");
                }
                while (session.Undo()) { }
                Assert.AreEqual(initial, session.State.ComputeHash(), level.Id + " undo to start");
                Assert.IsFalse(session.Undo());
            }
        }

        [Test]
        public void I11_Victory_OnlyWhenEveryBudHasBloomed()
        {
            foreach (LevelDefinition level in TestLevels.AllReferenceLevels())
            {
                var session = new GameSession(level);
                for (int i = 0; i < level.ReferenceSolution.Count; i++)
                {
                    MoveResult r = session.SelectLane(level.ReferenceSolution[i]);
                    bool last = i == level.ReferenceSolution.Count - 1;
                    Assert.AreEqual(last, r.State.IsWon, $"{level.Id} move {i}");
                    Assert.AreEqual(r.State.IsWon, r.State.RemainingBuds(level) == 0, $"{level.Id} move {i}");
                    Assert.AreEqual(last, r.Events.Any(e => e.Type == GameEventType.Win), $"{level.Id} move {i}");
                }
                Assert.AreEqual(MoveRejection.GameOver, TurnResolver.CheckSelection(level, session.State, 0));
            }
        }

        [Test]
        public void I12_Deadlock_OnlyAfterStabilization()
        {
            // All three places get occupied by the third selection, but the cascade frees them: in progress.
            var cascade = new GameSession(CascadeLevel());
            cascade.SelectLane(0);
            cascade.SelectLane(0);
            MoveResult third = cascade.SelectLane(0);
            Assert.AreEqual(GameStatus.InProgress, third.State.Status);
            Assert.IsFalse(third.Events.Any(e => e.Type == GameEventType.Deadlock));
            Assert.IsTrue(cascade.SelectLane(1).State.IsWon);

            // Three waiting cans with no possible target: deadlock, reported once, undo still works.
            var stuck = new GameSession(DeadlockLevel());
            Assert.AreEqual(GameStatus.InProgress, stuck.SelectLane(0).State.Status);
            Assert.AreEqual(GameStatus.InProgress, stuck.SelectLane(0).State.Status);
            MoveResult last = stuck.SelectLane(0);
            Assert.AreEqual(GameStatus.Deadlocked, last.State.Status);
            Assert.AreEqual(GameEventType.Deadlock, last.Events.Last().Type);
            Assert.IsTrue(stuck.Undo());
            Assert.AreEqual(GameStatus.InProgress, stuck.State.Status);
        }

        [TestCase("")]
        [TestCase("{")]
        [TestCase("not json")]
        [TestCase("[1,2,3]")]
        [TestCase("{\"id\":\"X\",}")]
        [TestCase("{\"id\":\"X\",\"rulesVersion\":\"0.1.0\"}")]
        [TestCase("{\"id\":\"X\",\"rulesVersion\":\"0.1.0\",\"slotCount\":3,\"grid\":{\"width\":1,\"height\":1,\"cells\":[[\"neon\"]]},\"lanes\":[[],[],[]]}")]
        public void I13_MalformedJson_FailsCleanly(string json)
        {
            bool ok = true;
            Assert.DoesNotThrow(() => ok = LevelLoader.TryLoad(json, out _, out _));
            Assert.IsFalse(ok);
            LevelLoader.TryLoad(json, out LevelDefinition level, out ValidationResult result);
            Assert.IsNull(level);
            Assert.IsNotEmpty(result.Errors);
        }

        [Test]
        public void I14_EveryReferenceLevel_HasAVerifiedSolution()
        {
            // Full solver check arrives in P2 (BFS C#). For now: each level ships a solution that replays to a win.
            int count = 0;
            foreach (LevelDefinition level in TestLevels.AllReferenceLevels())
            {
                Assert.IsNotEmpty(level.ReferenceSolution, level.Id);
                count++;
            }
            Assert.AreEqual(20, count);
        }

        [Test]
        public void I15_ReplayingTheReferenceSolution_CompletesTheLevel()
        {
            foreach (LevelDefinition level in TestLevels.AllReferenceLevels())
            {
                var session = new GameSession(level);
                Assert.IsTrue(session.Replay(level.ReferenceSolution), level.Id);
                Assert.IsTrue(session.State.IsWon, level.Id);
                Assert.AreEqual(level.ReferenceSolution.Count, session.State.MovesMade, level.Id);
            }
        }

        private static ColorId CanColor(LevelDefinition level, string canId) =>
            level.Lanes.SelectMany(l => l).First(c => c.Id == canId).Color;

        private static string PlayTranscript(LevelDefinition level, IEnumerable<int> lanes)
        {
            var session = new GameSession(level);
            var events = new List<GameEvent>();
            session.Replay(lanes, events);
            return string.Join("\n", events.Select(e => e.ToString())) + "\n#" + session.State.ComputeHash();
        }
    }
}
