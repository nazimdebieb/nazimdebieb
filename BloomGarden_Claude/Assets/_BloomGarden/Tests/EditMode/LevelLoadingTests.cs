using System.Collections.Generic;
using System.IO;
using System.Linq;
using BloomGarden.Core.Json;
using BloomGarden.Core.Levels;
using BloomGarden.Core.Model;
using NUnit.Framework;

namespace BloomGarden.Tests
{
    public class LevelLoadingTests
    {
        private const string ValidJson =
            "{\"id\":\"X\",\"rulesVersion\":\"0.1.0\",\"slotCount\":3,\"title\":\"T\",\"theme\":\"meadow\"," +
            "\"grid\":{\"width\":2,\"height\":1,\"cells\":[[\"coral\",\"blue\"],[]]}," +
            "\"lanes\":[[{\"id\":\"c\",\"color\":\"coral\",\"capacity\":1}],[{\"id\":\"b\",\"color\":\"blue\",\"capacity\":1}],[]]," +
            "\"referenceSolution\":[0,1]}";

        [Test]
        public void AllTwentyReferenceLevels_LoadAndValidate()
        {
            var files = TestLevels.ReferenceLevelFiles().ToList();
            Assert.AreEqual(20, files.Count);
            foreach (string path in files)
            {
                bool ok = LevelLoader.TryLoad(File.ReadAllText(path), out LevelDefinition level, out ValidationResult result);
                Assert.IsTrue(ok, Path.GetFileName(path) + ": " + result);
                Assert.AreEqual(Path.GetFileNameWithoutExtension(path), level.Id);
                Assert.AreEqual(level.Width * level.Height, level.CellCount);
            }
        }

        [Test]
        public void ValidJson_IsParsedInReadingOrder_VisibleLayerFirst()
        {
            Assert.IsTrue(LevelLoader.TryLoad(ValidJson, out LevelDefinition level, out ValidationResult result), result.ToString());
            Assert.AreEqual(ColorId.Coral, level.Cells[0][0]);
            Assert.AreEqual(ColorId.Blue, level.Cells[0][1]);
            Assert.AreEqual(0, level.Cells[1].Count);
            Assert.AreEqual("c", level.Lanes[0][0].Id);
            CollectionAssert.AreEqual(new[] { 0, 1 }, level.ReferenceSolution);
        }

        [TestCase("\"rulesVersion\":\"0.1.0\"", "\"rulesVersion\":\"9.9.9\"", TestName = "Rejects unsupported rules version")]
        [TestCase("\"slotCount\":3", "\"slotCount\":4", TestName = "Rejects slot count other than 3")]
        [TestCase("\"width\":2", "\"width\":3", TestName = "Rejects wrong cell count")]
        [TestCase("[\"coral\",\"blue\"]", "[\"coral\",\"blue\",\"coral\",\"blue\"]", TestName = "Rejects more than 3 layers")]
        [TestCase("\"id\":\"b\"", "\"id\":\"c\"", TestName = "Rejects duplicate can id")]
        [TestCase("\"color\":\"blue\"", "\"color\":\"green\"", TestName = "Rejects unknown can color")]
        [TestCase("[\"coral\",\"blue\"],[]", "[\"coral\",\"blue\"],[\"blue\"]", TestName = "Rejects unequal charge and buds")]
        [TestCase(",[]],\"referenceSolution\"", "],\"referenceSolution\"", TestName = "Rejects two lanes")]
        [TestCase("[0,1]", "[0,3]", TestName = "Rejects invalid solution lane")]
        public void InvalidLevels_AreRejected(string find, string replace)
        {
            StringAssert.Contains(find, ValidJson);
            string json = ValidJson.Replace(find, replace);
            Assert.IsFalse(LevelLoader.TryLoad(json, out LevelDefinition level, out ValidationResult result));
            Assert.IsNull(level);
            Assert.IsNotEmpty(result.Errors);
        }

        [Test]
        public void EmptyLevel_IsRejected()
        {
            var level = TestLevels.Build(1, 1, new[] { "" }, new[] { "", "", "" });
            StringAssert.Contains("Empty level", LevelValidator.Validate(level).ToString());
        }

        [Test]
        public void MiniJson_ParsesScalarsAndNesting()
        {
            var obj = (Dictionary<string, object>)MiniJson.Parse("{\"a\":[1,-2,3.5,true,false,null,\"x\\u00e9\\n\"],\"b\":{}}");
            var a = (List<object>)obj["a"];
            Assert.AreEqual(1L, a[0]);
            Assert.AreEqual(-2L, a[1]);
            Assert.AreEqual(3.5, a[2]);
            Assert.AreEqual(true, a[3]);
            Assert.AreEqual(false, a[4]);
            Assert.IsNull(a[5]);
            Assert.AreEqual("xé\n", a[6]);
            Assert.IsEmpty((Dictionary<string, object>)obj["b"]);
        }

        [TestCase("{\"a\":1,\"a\":2}")]
        [TestCase("[01]")]
        [TestCase("[1,]")]
        [TestCase("\"unterminated")]
        [TestCase("{} {}")]
        public void MiniJson_RejectsInvalidDocuments(string json)
        {
            Assert.Throws<JsonParseException>(() => MiniJson.Parse(json));
        }
    }
}
