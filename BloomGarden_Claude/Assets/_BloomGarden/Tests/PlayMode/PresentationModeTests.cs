using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BloomGarden.Core.Rules;
using BloomGarden.Presentation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace BloomGarden.Tests
{
    /// <summary>
    /// P4: animation modes never change results (P4-07), the logic never waits for effects (P4-02),
    /// and visuals are pooled so replaying a level creates no new objects (P4-08, with frame-time report).
    /// Player preferences are saved before and restored after each test.
    /// </summary>
    public class PresentationModeTests
    {
        private GameController _game;
        private float _volume;
        private bool _fast, _reduced;

        [UnitySetUp]
        public IEnumerator LoadScene()
        {
            SceneManager.LoadScene("Game");
            yield return null;
            yield return null;
            _game = Object.FindAnyObjectByType<GameController>();
            Assert.IsNotNull(_game);
            _volume = _game.Settings.SoundVolume;
            _fast = _game.Settings.FastAnimations;
            _reduced = _game.Settings.ReducedMotion;
        }

        [TearDown]
        public void RestoreSettings()
        {
            if (_game == null) return;
            _game.Settings.SetSoundVolume(_volume);
            _game.Settings.SetFastAnimations(_fast);
            _game.Settings.SetReducedMotion(_reduced);
        }

        private void SetMode(bool fast, bool reduced)
        {
            _game.Settings.SetFastAnimations(fast);
            _game.Settings.SetReducedMotion(reduced);
        }

        private IEnumerator PlaySolution(List<float> frameTimes = null)
        {
            foreach (int lane in _game.Session.Level.ReferenceSolution)
            {
                Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(lane)));
                float limit = Time.realtimeSinceStartup + 20f;
                while (_game.IsAnimating)
                {
                    Assert.Less(Time.realtimeSinceStartup, limit, "animation never finished");
                    frameTimes?.Add(Time.unscaledDeltaTime);
                    yield return null;
                }
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator SameFinalState_InEveryAnimationMode()
        {
            var modes = new[] { (fast: false, reduced: false), (fast: true, reduced: false), (fast: false, reduced: true) };
            var hashes = new List<ulong>();
            var durations = new List<float>();
            foreach (var mode in modes)
            {
                SetMode(mode.fast, mode.reduced);
                _game.LoadLevel(19); // BG-020: longest solution
                yield return null;
                float start = Time.realtimeSinceStartup;
                yield return PlaySolution();
                durations.Add(Time.realtimeSinceStartup - start);
                Assert.IsTrue(_game.Session.State.IsWon, $"fast={mode.fast} reduced={mode.reduced}");
                Assert.IsTrue(_game.Garden.MatchesState(_game.Session.State, out string error), error);
                hashes.Add(_game.Session.State.ComputeHash());
            }
            Assert.AreEqual(hashes[0], hashes[1], "fast mode changed the result");
            Assert.AreEqual(hashes[0], hashes[2], "reduced motion changed the result");
            Assert.Less(durations[1], durations[0], "fast mode should be quicker");
            Assert.Less(durations[2], durations[0], "reduced motion should be quicker");
            Debug.Log($"[BloomGarden] BG-020 play time: normal={durations[0]:0.00}s fast={durations[1]:0.00}s reduced={durations[2]:0.00}s");
        }

        [UnityTest]
        public IEnumerator Sounds_AreAudible_AndFollowTheSetting()
        {
            // Exactly one listener, carried by the scene camera itself (not the runtime safety net).
            Assert.AreEqual(1, Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length, "one AudioListener expected");
            Assert.IsNotNull(Camera.main.GetComponent<AudioListener>(), "the scene camera must carry the AudioListener");
            Assert.Greater(AudioListener.volume, 0f);

            _game.Settings.SetSoundVolume(0.8f);
            int before = _game.Audio.PlayedCount;
            Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(_game.Session.Level.ReferenceSolution[0])));
            yield return null;
            Assert.Greater(_game.Audio.PlayedCount, before, "selecting a can should play a sound");
            bool playing = false;
            foreach (var source in _game.Audio.GetComponents<AudioSource>()) playing |= source.isPlaying && source.volume > 0f;
            Assert.IsTrue(playing, "an AudioSource should be playing with a non-zero volume");
            while (_game.IsAnimating) yield return null;

            _game.Settings.SetSoundVolume(0f);
            _game.Restart();
            before = _game.Audio.PlayedCount;
            Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(_game.Session.Level.ReferenceSolution[0])));
            while (_game.IsAnimating) yield return null;
            Assert.AreEqual(before, _game.Audio.PlayedCount, "no sound when sound is off");
        }

        [UnityTest]
        public IEnumerator Logic_IsResolved_BeforeAnimationsPlay()
        {
            SetMode(false, false);
            _game.LoadLevel(12); // BG-013
            yield return null;
            int lane = _game.Session.Level.ReferenceSolution[0];
            Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(lane)));
            Assert.IsTrue(_game.IsAnimating, "animation should be running");

            var direct = new GameSession(_game.Session.Level);
            direct.SelectLane(lane);
            Assert.AreEqual(direct.State.ComputeHash(), _game.Session.State.ComputeHash(),
                "the logical state must already be final while effects are still playing");
            while (_game.IsAnimating) yield return null;
        }

        [UnityTest]
        public IEnumerator ReplayingALevel_ReusesPooledVisuals_AndReportsFrameTimes()
        {
            SetMode(false, false);
            _game.LoadLevel(12); // BG-013: largest state space, long cascades
            yield return null;
            var firstRun = new List<float>();
            yield return PlaySolution(firstRun);
            Assert.IsTrue(_game.Session.State.IsWon);
            int createdAfterFirst = _game.Garden.PooledObjectsCreated;

            _game.Restart();
            yield return null;
            var secondRun = new List<float>();
            long gcBefore = System.GC.CollectionCount(0);
            yield return PlaySolution(secondRun);
            long gcRuns = System.GC.CollectionCount(0) - gcBefore;
            int createdAfterSecond = _game.Garden.PooledObjectsCreated;

            Assert.AreEqual(createdAfterFirst, createdAfterSecond, "replaying created new visuals instead of reusing the pool");

            secondRun.Sort();
            float p50 = secondRun[secondRun.Count / 2] * 1000f;
            float p95 = secondRun[(int)(secondRun.Count * 0.95f)] * 1000f;
            float max = secondRun[secondRun.Count - 1] * 1000f;
            var report = new StringBuilder();
            report.AppendLine("Bloom Garden - P4 performance sample (Unity editor, Play mode, not a device measurement)");
            report.AppendLine($"Unity {Application.unityVersion}, level BG-013 reference solution, normal speed, second run after Restart");
            report.AppendLine($"frames={secondRun.Count} p50={p50:0.0}ms p95={p95:0.0}ms max={max:0.0}ms");
            report.AppendLine($"pooled objects created: first run={createdAfterFirst}, after replay={createdAfterSecond} (0 new)");
            report.AppendLine($"gen0 GC collections during replay={gcRuns}");
            string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Docs", "QA", "P4_perf.txt");
            File.WriteAllText(path, report.ToString());
            Debug.Log("[BloomGarden] " + report.ToString().Replace("\n", " | "));
        }
    }
}
