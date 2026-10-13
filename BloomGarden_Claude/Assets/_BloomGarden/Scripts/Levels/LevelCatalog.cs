using System.Collections.Generic;
using BloomGarden.Core.Levels;
using BloomGarden.Core.Model;
using UnityEngine;

namespace BloomGarden.Levels
{
    /// <summary>
    /// Ordered list of the level JSON files shipped with the game. Referencing them as TextAssets includes them
    /// in the build without Resources/ or StreamingAssets/. Rebuilt from the menu
    /// "Bloom Garden/P2 - Construire le catalogue de niveaux".
    /// </summary>
    [CreateAssetMenu(fileName = "LevelCatalog", menuName = "Bloom Garden/Level Catalog")]
    public sealed class LevelCatalog : ScriptableObject
    {
        [SerializeField] private List<TextAsset> levels = new List<TextAsset>();

        public int Count => levels.Count;

        public IReadOnlyList<TextAsset> Files => levels;

        public void SetFiles(IEnumerable<TextAsset> files)
        {
            levels = new List<TextAsset>(files);
        }

        /// <summary>Parses and validates one level. Invalid data is reported, never silently skipped.</summary>
        public bool TryLoad(int index, out LevelDefinition level, out ValidationResult result)
        {
            TextAsset file = index >= 0 && index < levels.Count ? levels[index] : null;
            if (file == null)
            {
                level = null;
                result = new ValidationResult();
                result.AddError($"No level file at index {index}");
                return false;
            }
            return LevelLoader.TryLoad(file.text, out level, out result);
        }
    }
}
