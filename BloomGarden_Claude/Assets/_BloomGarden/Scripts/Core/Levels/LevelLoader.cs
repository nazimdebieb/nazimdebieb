using System.Collections.Generic;
using BloomGarden.Core.Json;
using BloomGarden.Core.Model;

namespace BloomGarden.Core.Levels
{
    /// <summary>
    /// Reads a level JSON document into a <see cref="LevelDefinition"/>, then validates it.
    /// Never throws on bad input: every problem is reported in the <see cref="ValidationResult"/>.
    /// </summary>
    public static class LevelLoader
    {
        public static bool TryLoad(string json, out LevelDefinition level, out ValidationResult result)
        {
            level = null;
            result = new ValidationResult();

            object root;
            try
            {
                root = MiniJson.Parse(json);
            }
            catch (JsonParseException e)
            {
                result.AddError("Malformed JSON: " + e.Message);
                return false;
            }

            if (!(root is Dictionary<string, object> obj))
            {
                result.AddError("Root must be a JSON object");
                return false;
            }

            string id = ReadString(obj, "id", result, required: true);
            string rulesVersion = ReadString(obj, "rulesVersion", result, required: true);
            string title = ReadString(obj, "title", result, required: false) ?? string.Empty;
            string theme = ReadString(obj, "theme", result, required: false) ?? string.Empty;
            int slotCount = ReadInt(obj, "slotCount", result);

            int width = 0, height = 0;
            var cells = new List<List<ColorId>>();
            if (obj.TryGetValue("grid", out object gridObj) && gridObj is Dictionary<string, object> grid)
            {
                width = ReadInt(grid, "width", result, "grid.");
                height = ReadInt(grid, "height", result, "grid.");
                if (grid.TryGetValue("cells", out object cellsObj) && cellsObj is List<object> rawCells)
                {
                    for (int i = 0; i < rawCells.Count; i++)
                    {
                        var stack = new List<ColorId>();
                        if (rawCells[i] is List<object> rawStack)
                        {
                            for (int layer = 0; layer < rawStack.Count; layer++)
                            {
                                if (rawStack[layer] is string s && ColorIds.TryParse(s, out ColorId color))
                                    stack.Add(color);
                                else
                                    result.AddError($"Unknown flower color '{rawStack[layer]}' at cell {i} layer {layer}");
                            }
                        }
                        else
                        {
                            result.AddError($"Cell {i} must be an array of colors");
                        }
                        cells.Add(stack);
                    }
                }
                else
                {
                    result.AddError("grid.cells must be an array");
                }
            }
            else
            {
                result.AddError("Missing object 'grid'");
            }

            var lanes = new List<List<CanDefinition>>();
            if (obj.TryGetValue("lanes", out object lanesObj) && lanesObj is List<object> rawLanes)
            {
                for (int l = 0; l < rawLanes.Count; l++)
                {
                    var lane = new List<CanDefinition>();
                    if (rawLanes[l] is List<object> rawCans)
                    {
                        for (int k = 0; k < rawCans.Count; k++)
                        {
                            string where = $"lanes[{l}][{k}]";
                            if (!(rawCans[k] is Dictionary<string, object> rawCan))
                            {
                                result.AddError($"{where} must be an object");
                                continue;
                            }
                            string canId = ReadString(rawCan, "id", result, required: true, prefix: where + ".");
                            int capacity = ReadInt(rawCan, "capacity", result, where + ".");
                            ColorId canColor = ColorId.Coral;
                            if (!(rawCan.TryGetValue("color", out object c) && c is string cs && ColorIds.TryParse(cs, out canColor)))
                                result.AddError($"Invalid watering can color at {where}");
                            lane.Add(new CanDefinition(canId, canColor, capacity));
                        }
                    }
                    else
                    {
                        result.AddError($"lanes[{l}] must be an array");
                    }
                    lanes.Add(lane);
                }
            }
            else
            {
                result.AddError("Missing array 'lanes'");
            }

            var solution = new List<int>();
            if (obj.TryGetValue("referenceSolution", out object solObj) && solObj != null)
            {
                if (solObj is List<object> rawSol)
                {
                    foreach (object step in rawSol)
                    {
                        if (step is long lane && lane >= 0 && lane < LevelDefinition.LaneCount) solution.Add((int)lane);
                        else result.AddError($"Invalid referenceSolution entry '{step}'");
                    }
                }
                else
                {
                    result.AddError("referenceSolution must be an array");
                }
            }

            if (!result.IsValid) return false;

            var candidate = new LevelDefinition(id, rulesVersion, title, theme, slotCount, width, height, cells, lanes, solution);
            foreach (string error in LevelValidator.Validate(candidate).Errors) result.AddError(error);
            if (!result.IsValid) return false;

            level = candidate;
            return true;
        }

        private static string ReadString(Dictionary<string, object> obj, string key, ValidationResult result, bool required, string prefix = "")
        {
            if (obj.TryGetValue(key, out object value) && value is string s) return s;
            if (required || (value != null && !(value is string))) result.AddError($"'{prefix}{key}' must be a string");
            return null;
        }

        private static int ReadInt(Dictionary<string, object> obj, string key, ValidationResult result, string prefix = "")
        {
            if (obj.TryGetValue(key, out object value) && value is long l && l >= int.MinValue && l <= int.MaxValue) return (int)l;
            result.AddError($"'{prefix}{key}' must be an integer");
            return 0;
        }
    }
}
