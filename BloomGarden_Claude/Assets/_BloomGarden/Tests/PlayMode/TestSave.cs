using System.IO;
using BloomGarden.Presentation;
using UnityEngine;

namespace BloomGarden.Tests
{
    /// <summary>Points the game to an empty temporary save folder so tests never touch the player's progress.</summary>
    internal static class TestSave
    {
        public static string Directory => Path.Combine(Application.temporaryCachePath, "bloomgarden-tests");

        public static void UseFreshTemporarySave()
        {
            if (System.IO.Directory.Exists(Directory)) System.IO.Directory.Delete(Directory, true);
            GameController.SaveDirectoryOverride = Directory;
        }

        /// <summary>Same folder, kept as is (to test resuming after a reload).</summary>
        public static void KeepTemporarySave() => GameController.SaveDirectoryOverride = Directory;
    }
}
