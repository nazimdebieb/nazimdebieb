using System.IO;
using BloomGarden.Core.Model;
using BloomGarden.Presentation;
using NUnit.Framework;
using UnityEngine;

namespace BloomGarden.Tests
{
    /// <summary>P5-05 / P5-06: versioned save robustness, legacy migration, text table completeness, tutorial rules.</summary>
    public class SaveAndLocalizationTests
    {
        private string _dir;

        [SetUp]
        public void CreateFolder()
        {
            _dir = Path.Combine(Path.GetTempPath(), "bloomgarden-save-tests-" + System.Guid.NewGuid().ToString("N"));
            PlayerPrefs.DeleteKey("bg.sound.volume");
            PlayerPrefs.DeleteKey("bg.anim.fast");
            PlayerPrefs.DeleteKey("bg.anim.reduced");
        }

        [TearDown]
        public void DeleteFolder()
        {
            if (Directory.Exists(_dir)) Directory.Delete(_dir, true);
            PlayerPrefs.DeleteKey("bg.sound.volume");
            PlayerPrefs.DeleteKey("bg.anim.fast");
            PlayerPrefs.DeleteKey("bg.anim.reduced");
        }

        private static LevelDefinition Level(string id)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "_BloomGarden", "Levels", id + ".json");
            Assert.IsTrue(BloomGarden.Core.Levels.LevelLoader.TryLoad(File.ReadAllText(path), out LevelDefinition level, out var result), result.ToString());
            return level;
        }

        private static SaveData Sample()
        {
            var data = new SaveData { unlocked = 4 };
            data.RecordWin("BG-001", 3);
            data.RecordWin("BG-002", 5);
            data.RecordWin("BG-002", 4);
            data.settings.fast = true;
            data.settings.language = "en";
            data.progress.levelId = "BG-004";
            data.progress.lanes.AddRange(new[] { 0, 2, 1 });
            return data;
        }

        [Test]
        public void Save_RoundTrips_AllFields()
        {
            var service = new SaveService(_dir);
            service.Save(Sample());
            SaveData loaded = new SaveService(_dir).Load();
            Assert.AreEqual(SaveData.CurrentVersion, loaded.version);
            Assert.AreEqual(4, loaded.unlocked);
            Assert.AreEqual(3, loaded.Record("BG-001").bestMoves);
            Assert.AreEqual(4, loaded.Record("BG-002").bestMoves, "best move count kept");
            Assert.IsTrue(loaded.settings.fast);
            Assert.AreEqual("en", loaded.settings.language);
            Assert.AreEqual("BG-004", loaded.progress.levelId);
            CollectionAssert.AreEqual(new[] { 0, 2, 1 }, loaded.progress.lanes);
        }

        [Test]
        public void InterruptedWrite_IsRecovered_FromTemporaryFile()
        {
            var service = new SaveService(_dir);
            service.Save(Sample());
            // Simulate a crash after the old save was removed but before the new one was moved in place.
            File.Copy(service.Path, service.Path + ".tmp", true);
            File.Delete(service.Path);
            var reader = new SaveService(_dir);
            Assert.AreEqual(4, reader.Load().unlocked);
            Assert.AreEqual("temp", reader.LastLoadSource);
        }

        [Test]
        public void CorruptSave_FallsBack_ToBackup()
        {
            var service = new SaveService(_dir);
            var first = Sample();
            service.Save(first);
            first.unlocked = 7;
            service.Save(first); // previous one becomes the backup
            File.WriteAllText(service.Path, "{ this is not json");
            var reader = new SaveService(_dir);
            SaveData loaded = reader.Load();
            Assert.AreEqual("backup", reader.LastLoadSource);
            Assert.AreEqual(4, loaded.unlocked);
        }

        [Test]
        public void EverythingUnreadable_StartsFresh_WithoutThrowing()
        {
            var service = new SaveService(_dir);
            File.WriteAllText(service.Path, "garbage");
            File.WriteAllText(service.Path + ".bak", "");
            SaveData loaded = service.Load();
            Assert.AreEqual("reset", service.LastLoadSource);
            Assert.AreEqual(1, loaded.unlocked);
            Assert.IsEmpty(loaded.records);
        }

        [Test]
        public void LegacyPlayerPrefsSettings_AreMigrated()
        {
            PlayerPrefs.SetFloat("bg.sound.volume", 0f);
            PlayerPrefs.SetInt("bg.anim.reduced", 1);
            var service = new SaveService(_dir);
            SaveData loaded = service.Load();
            Assert.AreEqual("migrated", service.LastLoadSource);
            Assert.AreEqual(0f, loaded.settings.volume);
            Assert.IsTrue(loaded.settings.reduced);
        }

        [Test]
        public void NewerSaveVersion_IsReadAsFarAsPossible()
        {
            Directory.CreateDirectory(_dir);
            File.WriteAllText(Path.Combine(_dir, SaveService.FileName),
                "{\"version\":2,\"unlocked\":3,\"futureField\":42,\"records\":[{\"id\":\"BG-001\",\"bestMoves\":2}]}");
            SaveData loaded = new SaveService(_dir).Load();
            Assert.AreEqual(3, loaded.unlocked);
            Assert.AreEqual(2, loaded.Record("BG-001").bestMoves);
            Assert.IsNotNull(loaded.progress);
            Assert.IsNotNull(loaded.settings);
        }

        [Test]
        public void TextTable_HasFrenchAndEnglish_ForEveryKey()
        {
            foreach (string key in Loc.Keys)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(Loc.In(Language.French, key)), "fr missing: " + key);
                Assert.IsFalse(string.IsNullOrWhiteSpace(Loc.In(Language.English, key)), "en missing: " + key);
            }
            Assert.AreEqual("#nope", Loc.T("nope"), "missing keys are visible");
        }

        [Test]
        public void Tutorial_OneIdeaPerFirstLevel_AndSilentOnceCompleted()
        {
            LevelDefinition level1 = Level("BG-001");
            GameState start = GameState.CreateInitial(level1);
            TutorialHint hint = Tutorial.For(0, level1, start, levelCompletedBefore: false);
            Assert.AreEqual("tut_tap", hint.Key);
            Assert.AreEqual(level1.ReferenceSolution[0], hint.Lane);
            Assert.IsTrue(Tutorial.For(0, level1, start, levelCompletedBefore: true).IsNone);

            Assert.AreEqual("tut_places", Tutorial.For(1, Level("BG-002"), GameState.CreateInitial(Level("BG-002")), false).Key);
            Assert.AreEqual("tut_undo", Tutorial.For(2, Level("BG-003"), GameState.CreateInitial(Level("BG-003")), false).Key);
            Assert.AreEqual("tut_hidden", Tutorial.For(3, Level("BG-004"), GameState.CreateInitial(Level("BG-004")), false).Key);
            Assert.IsTrue(Tutorial.For(4, Level("BG-005"), GameState.CreateInitial(Level("BG-005")), false).IsNone);
        }
    }
}
