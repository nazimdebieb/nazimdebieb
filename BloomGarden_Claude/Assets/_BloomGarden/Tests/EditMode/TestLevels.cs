using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BloomGarden.Core.Levels;
using BloomGarden.Core.Model;
using NUnit.Framework;

namespace BloomGarden.Tests
{
    /// <summary>Shared helpers: locating project files (works in Unity and in Tools/DotnetTests) and building small levels.</summary>
    internal static class TestLevels
    {
        private static string _root;

        /// <summary>Directory containing Assets/_BloomGarden (the Unity project root).</summary>
        public static string ProjectRoot
        {
            get
            {
                if (_root != null) return _root;
                foreach (string start in new[] { Directory.GetCurrentDirectory(), TestContext.CurrentContext.TestDirectory, AppContext.BaseDirectory })
                {
                    var dir = new DirectoryInfo(start);
                    while (dir != null)
                    {
                        if (Directory.Exists(Path.Combine(dir.FullName, "Assets", "_BloomGarden"))) return _root = dir.FullName;
                        dir = dir.Parent;
                    }
                }
                throw new DirectoryNotFoundException("Assets/_BloomGarden not found from the test working directory");
            }
        }

        public static string LevelsDirectory => Path.Combine(ProjectRoot, "Assets", "_BloomGarden", "Levels");
        public static string FixturesDirectory => Path.Combine(ProjectRoot, "Assets", "_BloomGarden", "Tests", "EditMode", "Fixtures");

        public static IEnumerable<string> ReferenceLevelFiles() =>
            Directory.GetFiles(LevelsDirectory, "BG-*.json").OrderBy(p => p, StringComparer.Ordinal);

        public static LevelDefinition LoadReference(string id)
        {
            string path = Path.Combine(LevelsDirectory, id + ".json");
            Assert.IsTrue(LevelLoader.TryLoad(File.ReadAllText(path), out LevelDefinition level, out ValidationResult result), result.ToString());
            return level;
        }

        public static IEnumerable<LevelDefinition> AllReferenceLevels()
        {
            foreach (string path in ReferenceLevelFiles())
            {
                Assert.IsTrue(LevelLoader.TryLoad(File.ReadAllText(path), out LevelDefinition level, out ValidationResult result),
                    Path.GetFileName(path) + ": " + result);
                yield return level;
            }
        }

        /// <summary>Compact level builder. cells: one string per cell, letters c/b/y/p from visible to hidden ("" = empty).
        /// lanes: one string per lane, cans as "id:color:capacity" separated by spaces.</summary>
        public static LevelDefinition Build(int width, int height, string[] cells, string[] lanes, string id = "TEST")
        {
            var parsedCells = cells.Select(c => c.Select(ParseLetter).ToList()).ToList();
            var parsedLanes = lanes.Select(l => l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(ParseCan).ToList()).ToList();
            return new LevelDefinition(id, LevelDefinition.SupportedRulesVersion, id, "test", 3, width, height, parsedCells, parsedLanes);
        }

        private static ColorId ParseLetter(char c)
        {
            switch (c)
            {
                case 'c': return ColorId.Coral;
                case 'b': return ColorId.Blue;
                case 'y': return ColorId.Yellow;
                case 'p': return ColorId.Purple;
                default: throw new ArgumentException("Unknown color letter " + c);
            }
        }

        private static CanDefinition ParseCan(string token)
        {
            string[] parts = token.Split(':');
            Assert.IsTrue(ColorIds.TryParse(parts[1], out ColorId color), token);
            return new CanDefinition(parts[0], color, int.Parse(parts[2]));
        }
    }
}
