using BloomGarden.Core.Levels;
using BloomGarden.Core.Model;
using BloomGarden.EditorTools;
using BloomGarden.Levels;
using NUnit.Framework;
using UnityEditor;

namespace BloomGarden.Tests
{
    /// <summary>P2-01: the 20 level files are loaded through Unity (TextAsset in the catalog asset), in order, all valid.
    /// Needs the editor, so it lives outside the dotnet test harness.</summary>
    public class LevelCatalogTests
    {
        [Test]
        public void Catalog_LoadsTwentyValidLevels_InOrder()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<LevelCatalog>(LevelTools.CatalogPath);
            Assert.IsNotNull(catalog, "Run 'Bloom Garden/P2 - Construire le catalogue de niveaux' first");
            Assert.AreEqual(20, catalog.Count);
            for (int i = 0; i < catalog.Count; i++)
            {
                Assert.IsTrue(catalog.TryLoad(i, out LevelDefinition level, out ValidationResult result), result.ToString());
                Assert.AreEqual($"BG-{i + 1:000}", level.Id);
            }
        }

        [Test]
        public void Catalog_OutOfRange_ReportsAnError()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<LevelCatalog>(LevelTools.CatalogPath);
            Assert.IsFalse(catalog.TryLoad(99, out _, out ValidationResult result));
            Assert.IsFalse(result.IsValid);
        }
    }
}
