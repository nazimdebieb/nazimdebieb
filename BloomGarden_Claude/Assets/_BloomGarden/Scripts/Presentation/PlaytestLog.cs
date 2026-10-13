using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Local, anonymous playtest instrumentation (P6-01, GDD chapter 15). One JSON object per line in a file of the
    /// app's own folder; nothing is sent over the network, no device or advertising identifier, no name: only a
    /// random session id drawn at each launch. Testers share their data on purpose with the "Copy" button.
    /// </summary>
    public sealed class PlaytestLog
    {
        public const string FileName = "bloomgarden_playtest.jsonl";
        private const long MaxBytes = 1_000_000;

        /// <summary>Allowed property names: the schema is fixed so no personal data can slip in.</summary>
        public static readonly HashSet<string> Schema = new HashSet<string>
        {
            "e", "ts", "sid", "app", "platform", "lang", "level_id", "attempt", "resumed", "lane", "color", "cap",
            "turn_index", "ms", "used_slots", "steps", "duration_ms", "undos", "restarts", "deadlocks",
            "last_state_hash", "reason", "setting_name", "value", "hint",
            "fps_avg", "frame_p95_ms", "managed_mb", "alloc_mb", "ram_gb", "quality",
        };

        private readonly string _path;

        public string Path => _path;
        public string SessionId { get; }

        public PlaytestLog(string directory)
        {
            Directory.CreateDirectory(directory);
            _path = System.IO.Path.Combine(directory, FileName);
            SessionId = Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public void Write(string eventName, params (string key, object value)[] props)
        {
            var sb = new StringBuilder(128);
            sb.Append("{\"e\":").Append(Quote(eventName));
            sb.Append(",\"ts\":").Append(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            sb.Append(",\"sid\":").Append(Quote(SessionId));
            foreach (var (key, value) in props)
            {
                if (!Schema.Contains(key)) throw new ArgumentException("Property not in the playtest schema: " + key);
                sb.Append(',').Append(Quote(key)).Append(':').Append(Format(value));
            }
            sb.Append('}');
            try
            {
                if (File.Exists(_path) && new FileInfo(_path).Length > MaxBytes) Trim();
                File.AppendAllText(_path, sb.Append('\n').ToString());
            }
            catch (Exception e)
            {
                Debug.LogWarning("[BloomGarden] Playtest log not written: " + e.Message);
            }
        }

        public string ReadAll() => File.Exists(_path) ? File.ReadAllText(_path) : "";

        public void Clear()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }

        /// <summary>Keeps the newest half when the file grows too large.</summary>
        private void Trim()
        {
            string[] lines = File.ReadAllLines(_path);
            File.WriteAllLines(_path, lines.Skip(lines.Length / 2));
        }

        private static string Quote(string s) => "\"" + (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

        private static string Format(object value)
        {
            switch (value)
            {
                case null: return "null";
                case bool b: return b ? "true" : "false";
                case int i: return i.ToString(CultureInfo.InvariantCulture);
                case long l: return l.ToString(CultureInfo.InvariantCulture);
                case float f: return f.ToString("0.###", CultureInfo.InvariantCulture);
                case double d: return d.ToString("0.###", CultureInfo.InvariantCulture);
                default: return Quote(Convert.ToString(value, CultureInfo.InvariantCulture));
            }
        }

        // ------------------------------------------------------------------ on-device summary

        /// <summary>Per-level summary computed from the log (shown in Options → playtest data).</summary>
        public static List<string> Summarize(string jsonl, Func<string, string> levelLabel)
        {
            var levels = new SortedDictionary<string, (int starts, int wins, int abandons, int deadlocks, int undos, List<long> times)>();
            foreach (string line in jsonl.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string e = Field(line, "e"), level = Field(line, "level_id");
                if (string.IsNullOrEmpty(level)) continue;
                levels.TryGetValue(level, out var s);
                s.times ??= new List<long>();
                switch (e)
                {
                    case "level_started": s.starts++; break;
                    case "level_completed":
                        s.wins++;
                        if (long.TryParse(Field(line, "duration_ms"), out long ms)) s.times.Add(ms);
                        break;
                    case "level_abandoned": s.abandons++; break;
                    case "deadlock_detected": s.deadlocks++; break;
                    case "undo_used": s.undos++; break;
                }
                levels[level] = s;
            }
            var rows = new List<string>();
            foreach (var pair in levels)
            {
                var s = pair.Value;
                string median = s.times.Count == 0 ? "-" : (s.times.OrderBy(t => t).ElementAt(s.times.Count / 2) / 1000f).ToString("0", CultureInfo.InvariantCulture) + "s";
                rows.Add($"{levelLabel(pair.Key)} : {s.starts} / {s.wins} / {s.abandons} / {s.deadlocks} / {s.undos} / {median}");
            }
            return rows;
        }

        /// <summary>Reads a scalar property from one of our own lines (flat objects, no nesting).</summary>
        public static string Field(string line, string key)
        {
            string marker = "\"" + key + "\":";
            int i = line.IndexOf(marker, StringComparison.Ordinal);
            if (i < 0) return null;
            i += marker.Length;
            if (i < line.Length && line[i] == '"')
            {
                int end = line.IndexOf('"', i + 1);
                return end < 0 ? null : line.Substring(i + 1, end - i - 1);
            }
            int stop = line.IndexOfAny(new[] { ',', '}' }, i);
            return stop < 0 ? null : line.Substring(i, stop - i);
        }
    }
}
