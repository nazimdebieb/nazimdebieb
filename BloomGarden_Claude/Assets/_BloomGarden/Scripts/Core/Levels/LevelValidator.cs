using System.Collections.Generic;
using BloomGarden.Core.Model;

namespace BloomGarden.Core.Levels
{
    /// <summary>
    /// Semantic checks of rules 0.1.0, aligned with validate() of Kit/tools/solver_reference.py:
    /// version, 3 slots, grid 1..7, cell count, max 3 layers, 3 lanes, capacity >= 1,
    /// unique can ids, non-empty level, and exact water/bud conservation per color.
    /// </summary>
    public static class LevelValidator
    {
        public static ValidationResult Validate(LevelDefinition level)
        {
            var result = new ValidationResult();
            if (level == null)
            {
                result.AddError("Level is null");
                return result;
            }

            if (string.IsNullOrEmpty(level.Id)) result.AddError("Missing level id");
            if (level.RulesVersion != LevelDefinition.SupportedRulesVersion)
                result.AddError($"Unsupported rulesVersion '{level.RulesVersion}' (expected {LevelDefinition.SupportedRulesVersion})");
            if (level.SlotCount != LevelDefinition.MvpSlotCount)
                result.AddError($"MVP requires {LevelDefinition.MvpSlotCount} slots (got {level.SlotCount})");
            if (level.Width < 1 || level.Width > LevelDefinition.MaxGridSize || level.Height < 1 || level.Height > LevelDefinition.MaxGridSize)
                result.AddError($"Invalid grid {level.Width}x{level.Height}");
            if (level.Cells.Count != level.Width * level.Height)
                result.AddError($"Wrong number of grid cells: {level.Cells.Count} for {level.Width}x{level.Height}");

            var buds = new int[ColorIds.Count];
            int totalBuds = 0;
            for (int i = 0; i < level.Cells.Count; i++)
            {
                var stack = level.Cells[i];
                if (stack.Count > LevelDefinition.MaxLayersPerCell)
                    result.AddError($"Cell {i} has {stack.Count} layers (max {LevelDefinition.MaxLayersPerCell})");
                foreach (ColorId color in stack)
                {
                    if ((int)color < 0 || (int)color >= ColorIds.Count) result.AddError($"Unknown flower color at cell {i}");
                    else { buds[(int)color]++; totalBuds++; }
                }
            }

            if (level.Lanes.Count != LevelDefinition.LaneCount)
                result.AddError($"Three lanes required (got {level.Lanes.Count})");

            var water = new int[ColorIds.Count];
            var ids = new HashSet<string>();
            for (int l = 0; l < level.Lanes.Count; l++)
            {
                for (int k = 0; k < level.Lanes[l].Count; k++)
                {
                    CanDefinition can = level.Lanes[l][k];
                    if (string.IsNullOrEmpty(can.Id)) result.AddError($"Missing can id at lanes[{l}][{k}]");
                    else if (!ids.Add(can.Id)) result.AddError($"Duplicate can ID '{can.Id}'");
                    if (can.Capacity < 1) result.AddError($"Invalid capacity {can.Capacity} for can '{can.Id}'");
                    if ((int)can.Color < 0 || (int)can.Color >= ColorIds.Count) result.AddError($"Invalid watering can color for '{can.Id}'");
                    else if (can.Capacity > 0) water[(int)can.Color] += can.Capacity;
                }
            }

            if (totalBuds == 0) result.AddError("Empty level");

            for (int c = 0; c < ColorIds.Count; c++)
            {
                if (buds[c] != water[c])
                    result.AddError($"Unequal charge to flowers for {((ColorId)c).ToId()}: buds={buds[c]}, charge={water[c]}");
            }

            foreach (int lane in level.ReferenceSolution)
            {
                if (lane < 0 || lane >= LevelDefinition.LaneCount)
                {
                    result.AddError($"Invalid referenceSolution lane {lane}");
                    break;
                }
            }

            return result;
        }
    }
}
