using System.Collections;
using System.Linq;
using BloomGarden.Core.Rules;
using BloomGarden.Presentation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace BloomGarden.Tests
{
    /// <summary>
    /// P7: economy graphics (P7-04) and automatic downgrade never change results, the target frame rate is 60,
    /// performance samples reach the playtest log (P7-03), and every theme builds its own decor (P7-02).
    /// </summary>
    public class GraphicsAndThemeTests
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

        [TearDown]
        public void Restore()
        {
            if (_game != null) _game.Settings.SetGraphics(GraphicsMode.Auto);
        }

        private IEnumerator PlaySolution()
        {
            foreach (int lane in _game.Session.Level.ReferenceSolution)
            {
                Assert.IsTrue(_game.TapAt(_game.LaneHeadScreenPosition(lane)));
                while (_game.IsAnimating) yield return null;
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator EconomyMode_DropsShadowsAndMsaa_SameResult()
        {
            Assert.AreEqual(GraphicsQuality.TargetFps, Application.targetFrameRate, "Android defaults to 30 fps: the game asks for 60");
            Camera camera = Camera.main;

            _game.Settings.SetGraphics(GraphicsMode.High);
            Assert.IsFalse(_game.Quality.Economy);
            Assert.IsTrue(camera.allowMSAA);
            Assert.IsTrue(camera.GetUniversalAdditionalCameraData().renderShadows);

            _game.Settings.SetGraphics(GraphicsMode.Economy);
            Assert.IsTrue(_game.Quality.Economy);
            Assert.IsFalse(camera.allowMSAA);
            Assert.IsFalse(camera.GetUniversalAdditionalCameraData().renderShadows);
            Assert.AreEqual(LightShadows.None, Object.FindAnyObjectByType<Light>().shadows);

            _game.LoadLevel(4); // BG-005
            yield return null;
            yield return PlaySolution();
            Assert.IsTrue(_game.Session.State.IsWon);
            var direct = new GameSession(_game.Session.Level);
            direct.Replay(_game.Session.Level.ReferenceSolution);
            Assert.AreEqual(direct.State.ComputeHash(), _game.Session.State.ComputeHash(), "economy graphics must not change the game");
            Assert.IsTrue(_game.Garden.MatchesState(_game.Session.State, out string error), error);
        }

        [UnityTest]
        public IEnumerator AutoMode_Downgrades_AndLogsIt_AndPerfSamplesAreRecorded()
        {
            _game.Settings.SetGraphics(GraphicsMode.Auto);
            Assert.IsFalse(_game.Quality.Economy, "auto starts with high quality");
            _game.Quality.ForceAutoDowngrade();
            Assert.IsTrue(_game.Quality.Economy);
            Assert.IsTrue(_game.Quality.AutoDowngraded);

            _game.LoadLevel(0);
            for (int i = 0; i < 30; i++) yield return null;
            _game.LogPerfSample();

            string[] lines = _game.Playtest.ReadAll().Split('\n');
            Assert.IsTrue(lines.Any(l => l.Contains("\"setting_name\":\"graphics_auto\"") && l.Contains("\"value\":\"economy\"")));
            string perf = lines.Last(l => l.Contains("\"e\":\"perf_sample\""));
            foreach (string key in new[] { "fps_avg", "frame_p95_ms", "managed_mb", "alloc_mb", "ram_gb", "quality" })
                Assert.IsNotNull(PlaytestLog.Field(perf, key), "perf_sample misses " + key);
            Assert.AreEqual("economy", PlaytestLog.Field(perf, "quality"));
            Assert.Greater(float.Parse(PlaytestLog.Field(perf, "fps_avg"), System.Globalization.CultureInfo.InvariantCulture), 0f);
        }

        [UnityTest]
        public IEnumerator EachTheme_BuildsItsOwnDecor()
        {
            var expected = new[] { (index: 0, theme: "meadow", piece: "Tree"), (12, "pond", "Pond"), (16, "zen", "Lantern") };
            foreach (var (index, theme, piece) in expected)
            {
                _game.LoadLevel(index);
                yield return null;
                Assert.AreEqual(theme, _game.Session.Level.Theme);
                Transform decor = _game.Garden.transform.Find("Decor");
                Assert.IsNotNull(decor);
                Assert.IsTrue(decor.Cast<Transform>().Any(t => t.name == piece), $"{theme} should contain a {piece}");
                if (theme == "zen") Assert.IsTrue(decor.Cast<Transform>().Any(t => t.name == "Rake line"));
                if (theme != "zen") Assert.IsFalse(decor.Cast<Transform>().Any(t => t.name == "Lantern"));
                Assert.IsTrue(_game.Garden.MatchesState(_game.Session.State, out string error), error);
            }
        }
    }
}
