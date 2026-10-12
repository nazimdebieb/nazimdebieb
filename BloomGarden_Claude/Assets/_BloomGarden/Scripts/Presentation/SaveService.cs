using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BloomGarden.Presentation
{
    [Serializable]
    public sealed class LevelRecord
    {
        public string id;
        public int bestMoves;
    }

    [Serializable]
    public sealed class SettingsData
    {
        public float volume = 0.8f;
        public bool fast;
        public bool reduced;
        /// <summary>"fr", "en", or empty = follow the device.</summary>
        public string language = "";
    }

    [Serializable]
    public sealed class ProgressData
    {
        /// <summary>Level being played; empty when no game is in progress.</summary>
        public string levelId = "";
        /// <summary>Lane of every move since the start of the level. Replaying them rebuilds the exact state
        /// and the undo history (the rules are deterministic).</summary>
        public List<int> lanes = new List<int>();
    }

    /// <summary>Everything kept between sessions (P5-06). <see cref="version"/> drives migrations.</summary>
    [Serializable]
    public sealed class SaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        /// <summary>Number of unlocked levels (at least 1).</summary>
        public int unlocked = 1;
        public List<LevelRecord> records = new List<LevelRecord>();
        public SettingsData settings = new SettingsData();
        public ProgressData progress = new ProgressData();

        public LevelRecord Record(string levelId) => records.Find(r => r.id == levelId);

        public bool IsCompleted(string levelId) => Record(levelId) != null;

        /// <summary>Stores a win; keeps the smallest move count. Returns the best count.</summary>
        public int RecordWin(string levelId, int moves)
        {
            LevelRecord record = Record(levelId);
            if (record == null) records.Add(record = new LevelRecord { id = levelId, bestMoves = moves });
            else record.bestMoves = Math.Min(record.bestMoves, moves);
            return record.bestMoves;
        }
    }

    /// <summary>
    /// JSON save in the app's persistent folder. Writes go to a temporary file first, the previous save is kept
    /// as a backup, then the temporary file replaces the save: an interrupted write never loses the progress.
    /// Loading tries save, then temporary, then backup; unreadable data falls back to a fresh save, never a crash.
    /// Version 0 (P4, settings in PlayerPrefs) is migrated on first load.
    /// </summary>
    public sealed class SaveService
    {
        public const string FileName = "bloomgarden_save.json";

        private readonly string _path, _tmp, _backup;

        public string Path => _path;
        /// <summary>What happened at the last load: "loaded", "temp", "backup", "migrated", "new", or "reset".</summary>
        public string LastLoadSource { get; private set; }

        public SaveService(string directory)
        {
            Directory.CreateDirectory(directory);
            _path = System.IO.Path.Combine(directory, FileName);
            _tmp = _path + ".tmp";
            _backup = _path + ".bak";
        }

        public SaveData Load()
        {
            foreach (var (file, source) in new[] { (_path, "loaded"), (_tmp, "temp"), (_backup, "backup") })
            {
                SaveData data = TryRead(file);
                if (data == null) continue;
                LastLoadSource = source;
                return Upgrade(data);
            }
            bool anyFile = File.Exists(_path) || File.Exists(_tmp) || File.Exists(_backup);
            LastLoadSource = anyFile ? "reset" : "new";
            SaveData fresh = new SaveData();
            if (!anyFile && ImportLegacyPlayerPrefs(fresh.settings)) LastLoadSource = "migrated";
            return fresh;
        }

        public void Save(SaveData data)
        {
            data.version = SaveData.CurrentVersion;
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_tmp, json);
            if (File.Exists(_path))
            {
                File.Copy(_path, _backup, true);
                File.Delete(_path);
            }
            File.Move(_tmp, _path);
        }

        private static SaveData TryRead(string file)
        {
            try
            {
                if (!File.Exists(file)) return null;
                string json = File.ReadAllText(file);
                if (string.IsNullOrWhiteSpace(json)) return null;
                var data = JsonUtility.FromJson<SaveData>(json);
                return data != null && data.version >= 1 ? data : null;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[BloomGarden] Unreadable save {System.IO.Path.GetFileName(file)}: {e.Message}");
                return null;
            }
        }

        /// <summary>Repairs missing parts; a save written by a newer version is read as far as possible.</summary>
        private static SaveData Upgrade(SaveData data)
        {
            if (data.version > SaveData.CurrentVersion)
                Debug.LogWarning($"[BloomGarden] Save version {data.version} is newer than {SaveData.CurrentVersion}; unknown fields are ignored.");
            data.records ??= new List<LevelRecord>();
            data.settings ??= new SettingsData();
            data.progress ??= new ProgressData();
            data.progress.lanes ??= new List<int>();
            data.progress.levelId ??= "";
            data.settings.language ??= "";
            data.settings.volume = Mathf.Clamp01(data.settings.volume);
            data.unlocked = Math.Max(1, data.unlocked);
            return data;
        }

        /// <summary>P4 stored settings in PlayerPrefs (save version 0). Returns true when something was imported.</summary>
        private static bool ImportLegacyPlayerPrefs(SettingsData settings)
        {
            bool found = false;
            if (PlayerPrefs.HasKey("bg.sound.volume")) { settings.volume = Mathf.Clamp01(PlayerPrefs.GetFloat("bg.sound.volume")); found = true; }
            if (PlayerPrefs.HasKey("bg.anim.fast")) { settings.fast = PlayerPrefs.GetInt("bg.anim.fast") == 1; found = true; }
            if (PlayerPrefs.HasKey("bg.anim.reduced")) { settings.reduced = PlayerPrefs.GetInt("bg.anim.reduced") == 1; found = true; }
            return found;
        }
    }
}
