using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    /// P5 end-to-end: home and level selection, progression lock, tutorial, save and exact resume after a reload,
    /// pause during an animation, and FR/EN texts without overflow on every screen. Uses a temporary save folder.
    /// </summary>
    public class FlowAndSaveTests
    {
        private GameController _game;

        private IEnumerator LoadScene(bool freshSave = true)
        {
            if (freshSave) TestSave.UseFreshTemporarySave();
            else TestSave.KeepTemporarySave();
            SceneManager.LoadScene("Game");
            yield return null;
            yield return null;
            _game = Object.FindAnyObjectByType<GameController>();
            Assert.IsNotNull(_game);
        }

        private IEnumerator WaitAnimations()
        {
            float limit = Time.realtimeSinceStartup + 20f;
            while (_game.IsAnimating)
            {
                Assert.Less(Time.realtimeSinceStartup, limit);
                yield return null;
            }
            yield return null;
        }

        private IEnumerator PlayReferenceSolution()
        {
            foreach (int lane in _game.Session.Level.ReferenceSolution)
            {
                Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(lane)));
                yield return WaitAnimations();
            }
        }

        [TearDown]
        public void ResetLanguage()
        {
            if (_game != null) _game.Settings.SetLanguage(Language.French);
        }

        [UnityTest]
        public IEnumerator FreshStart_ShowsHome_ThenLevel1WithTutorial()
        {
            yield return LoadScene();
            Assert.AreEqual(GameScreen.Home, _game.CurrentScreen);
            Assert.IsTrue(_game.Menu.HomeVisible);
            Assert.AreEqual("new", _game.Saves.LastLoadSource == "migrated" ? "new" : _game.Saves.LastLoadSource);
            Assert.IsFalse(_game.TapAt(_game.LaneHeadScreenPosition(0)), "no move from the home screen");

            _game.Play(_game.ContinueLevelIndex());
            yield return null;
            Assert.AreEqual(GameScreen.Playing, _game.CurrentScreen);
            Assert.AreEqual("BG-001", _game.Session.Level.Id);
            Assert.AreEqual("tut_tap", _game.Hud.HintKey, "level 1 starts with the tap hint");
            Assert.IsTrue(_game.Hud.ArrowVisible, "the arrow points at the can to tap");

            Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(_game.Session.Level.ReferenceSolution[0])));
            Assert.AreEqual("tut_auto", _game.Hud.HintKey, "after the first tap: automatic watering hint");
            Assert.IsFalse(_game.Hud.ArrowVisible);
            yield return WaitAnimations();
        }

        [UnityTest]
        public IEnumerator Levels_AreLocked_UntilThePreviousOneIsWon()
        {
            yield return LoadScene();
            _game.ShowLevels();
            yield return null;
            Assert.IsTrue(_game.Menu.LevelsVisible);
            Assert.IsTrue(_game.Menu.IsLevelButtonInteractable(0));
            Assert.IsFalse(_game.Menu.IsLevelButtonInteractable(1), "level 2 locked at start");

            _game.Play(0);
            yield return null;
            yield return PlayReferenceSolution();
            Assert.IsTrue(_game.Session.State.IsWon);
            Assert.AreEqual(2, _game.SaveData.unlocked);
            Assert.IsTrue(_game.SaveData.IsCompleted("BG-001"));
            Assert.AreEqual(_game.Session.Level.ReferenceSolution.Count, _game.SaveData.Record("BG-001").bestMoves);
            Assert.IsNull(_game.Hud.HintKey, "no tutorial on the win screen");

            _game.ShowLevels();
            yield return null;
            Assert.IsTrue(_game.Menu.IsLevelButtonInteractable(1), "level 2 unlocked after winning level 1");
            Assert.IsFalse(_game.Menu.IsLevelButtonInteractable(2));

            // The win is on disk: a reload keeps it.
            yield return LoadScene(freshSave: false);
            Assert.AreEqual(2, _game.SaveData.unlocked);
            Assert.AreEqual(1, _game.ContinueLevelIndex(), "Play now continues at level 2");
        }

        [UnityTest]
        public IEnumerator GameInProgress_ResumesExactly_AfterReload_WithUndo()
        {
            yield return LoadScene();
            _game.LoadLevel(3); // BG-004
            yield return null;
            var solution = _game.Session.Level.ReferenceSolution;
            Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(solution[0])));
            yield return WaitAnimations();
            ulong afterOne = _game.Session.State.ComputeHash();
            Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(solution[1])));
            yield return WaitAnimations();
            ulong afterTwo = _game.Session.State.ComputeHash();

            yield return LoadScene(freshSave: false);
            Assert.AreEqual(GameScreen.Home, _game.CurrentScreen);
            Assert.AreEqual(3, _game.ContinueLevelIndex(), "home offers to continue BG-004");
            _game.Play(_game.ContinueLevelIndex());
            yield return null;
            Assert.AreEqual("BG-004", _game.Session.Level.Id);
            Assert.AreEqual(afterTwo, _game.Session.State.ComputeHash(), "exact state restored");
            Assert.IsTrue(_game.Garden.MatchesState(_game.Session.State, out string error), error);
            Assert.IsTrue(_game.Session.CanUndo, "undo history restored");
            _game.Undo();
            yield return null;
            Assert.AreEqual(afterOne, _game.Session.State.ComputeHash());
        }

        [UnityTest]
        public IEnumerator Pause_DuringAnimation_SnapsAndSaves()
        {
            yield return LoadScene();
            _game.LoadLevel(12); // BG-013: long cascades
            yield return null;
            int lane = _game.Session.Level.ReferenceSolution[0];
            Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(lane)));
            Assert.IsTrue(_game.IsAnimating);

            _game.SendMessage("OnApplicationPause", true);
            Assert.IsFalse(_game.IsAnimating, "no animation left half-way");
            yield return null;
            Assert.IsTrue(_game.Garden.MatchesState(_game.Session.State, out string error), error);

            string json = File.ReadAllText(_game.Saves.Path);
            var saved = JsonUtility.FromJson<SaveData>(json);
            Assert.AreEqual("BG-013", saved.progress.levelId);
            CollectionAssert.AreEqual(new[] { lane }, saved.progress.lanes);
        }

        [UnityTest]
        public IEnumerator BothLanguages_AllScreens_NoTextOverflow()
        {
            yield return LoadScene();
            foreach (Language language in new[] { Language.French, Language.English })
            {
                _game.Settings.SetLanguage(language);
                yield return null;
                var problems = new List<string>();

                _game.ShowHome();
                yield return null;
                problems.AddRange(Tag("home", _game.Hud.Ui.FindOverflows()));
                StringAssert.Contains(Loc.In(language, "home_levels"), Collect());

                _game.ShowLevels();
                yield return null;
                problems.AddRange(Tag("levels", _game.Hud.Ui.FindOverflows()));

                _game.LoadLevel(0); // tutorial bubble
                yield return null;
                problems.AddRange(Tag("level 1 + hint", _game.Hud.Ui.FindOverflows()));

                _game.Hud.ToggleSettings();
                yield return null;
                problems.AddRange(Tag("options", _game.Hud.Ui.FindOverflows()));
                _game.Hud.ShowPlaytest();
                yield return null;
                problems.AddRange(Tag("playtest data", _game.Hud.Ui.FindOverflows()));
                _game.Hud.HideOverlays();

                _game.LoadLevel(17); // deadlock panel
                yield return null;
                foreach (int lane in DeadlockPath(_game.Session.Level)) _game.TapAt(_game.LaneHeadScreenPosition(lane));
                yield return WaitAnimations();
                Assert.IsTrue(_game.Session.State.IsDeadlocked);
                problems.AddRange(Tag("deadlock", _game.Hud.Ui.FindOverflows()));

                _game.LoadLevel(19); // win panel on the last level
                yield return null;
                _game.Settings.SetFastAnimations(true);
                yield return PlayReferenceSolution();
                _game.Settings.SetFastAnimations(false);
                problems.AddRange(Tag("win", _game.Hud.Ui.FindOverflows()));

                Assert.IsEmpty(problems, language + ":\n" + string.Join("\n", problems));
            }
        }

        private static IEnumerable<string> Tag(string screen, List<string> problems)
        {
            foreach (string p in problems) yield return $"[{screen}] {p}";
        }

        private string Collect()
        {
            var all = new System.Text.StringBuilder();
            foreach (var label in _game.Hud.Ui.Labels)
                if (label != null && label.gameObject.activeInHierarchy) all.Append(label.text).Append('|');
            return all.ToString();
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
