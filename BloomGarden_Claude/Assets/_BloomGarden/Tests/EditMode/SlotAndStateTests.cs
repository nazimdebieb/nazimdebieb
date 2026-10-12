using System.Linq;
using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;
using NUnit.Framework;

namespace BloomGarden.Tests
{
    public class SlotAndStateTests
    {
        [Test]
        public void Can_TakesLeftmostFreePlace_ButOlderPlaceWatersFirst()
        {
            // purple and blue A wait; yellow frees place 2 and reveals purple, which frees place 0.
            // blue B then lands in place 0 (leftmost free) yet blue A (older, place 1) must water first.
            LevelDefinition level = TestLevels.Build(3, 1,
                new[] { "cb", "yp", "cb" },
                new[] { "p1:purple:1 bA:blue:1", "y1:yellow:1 c2:coral:2", "bB:blue:1" });
            var session = new GameSession(level);

            session.SelectLane(0); // purple -> place 0
            session.SelectLane(0); // blue A -> place 1
            session.SelectLane(1); // yellow -> place 2, cascade frees places 0 and 2
            Assert.IsFalse(session.State.GetSlot(0).IsOccupied);
            Assert.AreEqual("bA", session.State.GetSlot(1).CanId);
            Assert.IsFalse(session.State.GetSlot(2).IsOccupied);

            MoveResult b = session.SelectLane(2);
            Assert.AreEqual(0, b.Events[0].Slot, "newest can fills leftmost free place");
            MoveResult coral = session.SelectLane(1);

            var blooms = coral.Events.Where(e => e.Type == GameEventType.FlowerBloomed).ToList();
            CollectionAssert.AreEqual(new[] { "c2", "bA", "c2", "bB" }, blooms.Select(e => e.CanId).ToArray());
            CollectionAssert.AreEqual(new[] { 0, 0, 2, 2 }, blooms.Select(e => e.CellIndex).ToArray());
            Assert.IsTrue(coral.State.IsWon);
        }

        [Test]
        public void Target_IsSmallestRowThenColumn()
        {
            LevelDefinition level = TestLevels.Build(2, 2, new[] { "", "c", "c", "c" }, new[] { "c1:coral:3", "", "" });
            MoveResult r = TurnResolver.SelectLane(level, GameState.CreateInitial(level), 0);

            var cells = r.Events.Where(e => e.Type == GameEventType.FlowerBloomed).Select(e => (e.Row, e.Column)).ToArray();
            CollectionAssert.AreEqual(new[] { (0, 1), (1, 0), (1, 1) }, cells);
        }

        [Test]
        public void SelectLane_DoesNotMutateInputState()
        {
            LevelDefinition level = TestLevels.LoadReference("BG-004");
            GameState start = GameState.CreateInitial(level);
            ulong hash = start.ComputeHash();
            TurnResolver.SelectLane(level, start, level.ReferenceSolution[0]);
            Assert.AreEqual(hash, start.ComputeHash());
            Assert.AreEqual(0, start.MovesMade);
        }

        [Test]
        public void PositionKey_IgnoresMoveCountAndAbsoluteOrder()
        {
            LevelDefinition level = TestLevels.Build(2, 1, new[] { "c", "b" }, new[] { "c1:coral:1", "b1:blue:1", "" });
            var a = new GameSession(level);
            a.SelectLane(0);
            a.SelectLane(1);
            var b = new GameSession(level);
            b.SelectLane(1);
            b.SelectLane(0);
            Assert.AreEqual(a.State.ComputePositionKey(), b.State.ComputePositionKey());
        }

        [Test]
        public void Hash_IsStableAcrossRuns_GoldenValue()
        {
            // Golden value computed once; guards against accidental use of non-deterministic hashing.
            GameState initial = GameState.CreateInitial(TestLevels.LoadReference("BG-001"));
            Assert.AreEqual(initial.ComputeHash(), initial.ComputeHash());
            Assert.AreEqual(GoldenInitialHashBg001, initial.ComputeHash());
        }

        [Test]
        public void Restart_ClearsHistory_AndReturnsToInitialState()
        {
            LevelDefinition level = TestLevels.LoadReference("BG-012");
            var session = new GameSession(level);
            ulong initial = session.State.ComputeHash();
            session.Replay(level.ReferenceSolution.Take(3));
            session.Restart();
            Assert.AreEqual(initial, session.State.ComputeHash());
            Assert.IsFalse(session.CanUndo);
        }

        private const ulong GoldenInitialHashBg001 = 8352563957941649324UL;
    }
}
