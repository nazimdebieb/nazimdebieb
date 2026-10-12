using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BloomGarden.Core.Json;
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
    /// P6-01: the local playtest log records the GDD chapter 15 events in a fixed schema, stays in the app folder,
    /// and its on-device summary is readable. These are automated checks, not human playtest data.
    /// </summary>
    public class PlaytestLogTests
    {
        private GameController _game;

        [UnitySetUp]
        public IEnumerator LoadScene()
        {
            TestSave.UseFreshTemporarySave();
            SceneManager.LoadScene("Game");
            yield return null;
            yield return null;
            _game = Object.FindAnyObjectByType<GameController>();
            Assert.IsNotNull(_game);
        }

        private IEnumerator Wait()
        {
            while (_game.IsAnimating) yield return null;
            yield return null;
        }

        private List<Dictionary<string, object>> Events() =>
            _game.Playtest.ReadAll().Split('\n').Where(l => l.Trim().Length > 0)
                .Select(l => (Dictionary<string, object>)MiniJson.Parse(l)).ToList();

        [UnityTest]
        public IEnumerator Events_FollowTheSchema_AndTellWhatHappened()
        {
            StringAssert.StartsWith(TestSave.Directory, _game.Playtest.Path, "log stays in the app folder");

            _game.Play(0);
            yield return null;
            foreach (int lane in _game.Session.Level.ReferenceSolution)
            {
                Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(lane)));
                yield return Wait();
            }

            _game.LoadLevel(17); // BG-018: reach a deadlock, undo, then leave through the menu
            yield return null;
            foreach (int lane in DeadlockPath(_game.Session.Level)) _game.TapAt(_game.LaneHeadScreenPosition(lane));
            yield return Wait();
            Assert.IsTrue(_game.Session.State.IsDeadlocked);
            _game.Undo();
            _game.Settings.SetFastAnimations(true);
            _game.Settings.SetFastAnimations(false);
            _game.ShowHome();
            yield return null;

            var events = Events();
            foreach (var e in events)
                foreach (string key in e.Keys)
                    Assert.IsTrue(PlaytestLog.Schema.Contains(key), "property outside the schema: " + key);
            Assert.IsTrue(events.All(e => (string)e["sid"] == _game.Playtest.SessionId), "one anonymous session id");

            string[] names = events.Select(e => (string)e["e"]).ToArray();
            CollectionAssert.Contains(names, "session_started");
            var level1 = events.Where(e => e.TryGetValue("level_id", out object id) && (string)id == "BG-001").ToList();
            Assert.AreEqual("level_started", level1.First()["e"]);
            Assert.AreEqual(2, level1.Count(e => (string)e["e"] == "can_selected"), "BG-001 is won in 2 taps");
            var completed = level1.Single(e => (string)e["e"] == "level_completed");
            Assert.AreEqual(2L, completed["steps"]);
            Assert.AreEqual(0L, completed["undos"]);
            Assert.IsTrue(level1.Any(e => (string)e["e"] == "tutorial_hint_shown" && (string)e["hint"] == "tut_tap"));

            var level18 = events.Where(e => e.TryGetValue("level_id", out object id) && (string)id == "BG-018").ToList();
            Assert.IsTrue(level18.Any(e => (string)e["e"] == "deadlock_detected" && (long)e["used_slots"] == 3));
            Assert.IsTrue(level18.Any(e => (string)e["e"] == "undo_used"));
            var abandoned = level18.Single(e => (string)e["e"] == "level_abandoned");
            Assert.AreEqual("menu", abandoned["reason"]);
            Assert.AreEqual(2, events.Count(e => (string)e["e"] == "settings_changed" && (string)e["setting_name"] == "speed"));
        }

        [UnityTest]
        public IEnumerator Summary_IsShownOnThePhone_AndCanBeCleared()
        {
            _game.Play(0);
            yield return null;
            foreach (int lane in _game.Session.Level.ReferenceSolution)
            {
                Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(lane)));
                yield return Wait();
            }
            _game.Settings.SetLanguage(Language.French);
            _game.Hud.ShowPlaytest();
            yield return null;
            Assert.IsTrue(_game.Hud.PlaytestVisible);
            StringAssert.StartsWith("Niveau 1 : 1 / 1 / 0 / 0 / 0 /", _game.Hud.PlaytestText);

            _game.Playtest.Clear();
            _game.Hud.ShowPlaytest();
            yield return null;
            Assert.AreEqual(Loc.T("playtest_empty"), _game.Hud.PlaytestText);
            Assert.IsFalse(File.Exists(_game.Playtest.Path));
            _game.Hud.HideOverlays();
        }

        private static List<int> DeadlockPath(LevelDefinition level)
        {
            var seen = new HashSet<string>();
            var path = new List<int>();
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
            Search(GameState.CreateInitial(level));
            return path;
        }
    }
}
