using System.Collections;
using System.Collections.Generic;
using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;
using BloomGarden.Presentation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace BloomGarden.Tests
{
    /// <summary>
    /// P3 end-to-end checks in the real Game scene: screen taps go through the camera ray and the lane-head
    /// colliders to the TurnResolver, one move per tap, and the view, counters and end panel follow.
    /// </summary>
    public class GameScenePlayTests
    {
        private GameController _game;

        [UnitySetUp]
        public IEnumerator LoadScene()
        {
            SceneManager.LoadScene("Game");
            yield return null;
            yield return null;
            _game = Object.FindAnyObjectByType<GameController>();
            Assert.IsNotNull(_game, "GameController missing from the Game scene");
            Assert.IsNotNull(_game.Session, "No level loaded");
        }

        private IEnumerator Tap(int lane, bool expectAccepted = true)
        {
            bool accepted = _game.TapAt(_game.LaneHeadScreenPosition(lane));
            Assert.AreEqual(expectAccepted, accepted, $"tap on lane {lane + 1} ({_game.Session.Level.Id})");
            yield return WaitForAnimations();
        }

        private IEnumerator WaitForAnimations()
        {
            float limit = Time.realtimeSinceStartup + 10f;
            while (_game.IsAnimating)
            {
                Assert.Less(Time.realtimeSinceStartup, limit, "animation never finished");
                yield return null;
            }
            yield return null;
            AssertViewMatchesState();
        }

        private void AssertViewMatchesState()
        {
            Assert.IsTrue(_game.Garden.MatchesState(_game.Session.State, out string error), _game.Session.Level.Id + ": " + error);
        }

        [UnityTest]
        public IEnumerator Level1_WinByTaps_ThenLevel2()
        {
            Assert.AreEqual("BG-001", _game.Session.Level.Id);
            foreach (int lane in _game.Session.Level.ReferenceSolution) yield return Tap(lane);

            Assert.IsTrue(_game.Session.State.IsWon);
            Assert.IsTrue(_game.EndPanelVisible, "win panel");
            Assert.IsFalse(_game.TapAt(_game.LaneHeadScreenPosition(0)), "no move once the level is won");

            _game.NextLevel();
            yield return null;
            Assert.AreEqual("BG-002", _game.Session.Level.Id);
            Assert.IsFalse(_game.EndPanelVisible);
            foreach (int lane in _game.Session.Level.ReferenceSolution) yield return Tap(lane);
            Assert.IsTrue(_game.Session.State.IsWon);
        }

        [UnityTest]
        public IEnumerator TapDuringAnimation_IsExactlyOneMoreMove()
        {
            _game.LoadLevel(1); // BG-002: 3 moves, several lanes
            yield return null;
            var solution = _game.Session.Level.ReferenceSolution;
            Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(solution[0])));
            Assert.IsTrue(_game.IsAnimating);
            // Second tap before the first animation ends: the view snaps, then the new move plays.
            Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(solution[1])));
            Assert.AreEqual(2, _game.Session.State.MovesMade);
            yield return WaitForAnimations();
            AssertViewMatchesState();

            var session = new GameSession(_game.Session.Level);
            session.SelectLane(solution[0]);
            session.SelectLane(solution[1]);
            Assert.AreEqual(session.State.ComputeHash(), _game.Session.State.ComputeHash(), "same state as a direct replay");
        }

        [UnityTest]
        public IEnumerator UndoAndRestart_RestoreExactStates()
        {
            _game.LoadLevel(3); // BG-004
            yield return null;
            ulong start = _game.Session.State.ComputeHash();
            var solution = _game.Session.Level.ReferenceSolution;
            yield return Tap(solution[0]);
            ulong afterOne = _game.Session.State.ComputeHash();
            yield return Tap(solution[1]);

            _game.Undo();
            yield return null;
            Assert.AreEqual(afterOne, _game.Session.State.ComputeHash());
            Assert.IsFalse(_game.IsAnimating);
            AssertViewMatchesState();

            _game.Restart();
            yield return null;
            Assert.AreEqual(start, _game.Session.State.ComputeHash());
            Assert.IsFalse(_game.Session.CanUndo);
            AssertViewMatchesState();
        }

        [UnityTest]
        public IEnumerator Bg018_Deadlock_ShowsPanel_AndUndoRecovers()
        {
            _game.LoadLevel(17); // BG-018: the only reference level with traps
            yield return null;
            List<int> trap = FindDeadlockPath(_game.Session.Level);
            Assert.IsNotNull(trap, "BG-018 should contain a reachable deadlock");

            foreach (int lane in trap) yield return Tap(lane);
            Assert.IsTrue(_game.Session.State.IsDeadlocked);
            Assert.IsTrue(_game.EndPanelVisible, "deadlock panel");

            _game.Undo();
            yield return null;
            Assert.AreEqual(GameStatus.InProgress, _game.Session.State.Status);
            Assert.IsFalse(_game.EndPanelVisible);
            AssertViewMatchesState();
        }

        [UnityTest]
        public IEnumerator AllTwentyLevels_PlayToWin_ViewInSync()
        {
            for (int i = 0; i < 20; i++)
            {
                _game.LoadLevel(i);
                yield return null;
                AssertViewMatchesState();
                // Rapid taps: each one interrupts the previous animation, the view must still end in sync.
                foreach (int lane in _game.Session.Level.ReferenceSolution)
                    Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(lane)), $"{_game.Session.Level.Id} lane {lane + 1}");
                yield return WaitForAnimations();
                Assert.IsTrue(_game.Session.State.IsWon, _game.Session.Level.Id);
                Assert.IsTrue(_game.EndPanelVisible, _game.Session.Level.Id + " win panel");
            }
        }

        /// <summary>Depth-first search of a move sequence that ends in a deadlock.</summary>
        private static List<int> FindDeadlockPath(LevelDefinition level)
        {
            var seen = new HashSet<string>();
            var path = new List<int>();
            return Search(GameState.CreateInitial(level)) ? path : null;

            bool Search(GameState state)
            {
                if (state.IsDeadlocked) return true;
                if (state.IsWon || !seen.Add(state.ComputePositionKey())) return false;
                for (int lane = 0; lane < level.Lanes.Count; lane++)
                {
                    if (!TurnResolver.CanSelect(level, state, lane)) continue;
                    path.Add(lane);
                    if (Search(TurnResolver.SelectLane(level, state, lane).State)) return true;
                    path.RemoveAt(path.Count - 1);
                }
                return false;
            }
        }
    }
}
