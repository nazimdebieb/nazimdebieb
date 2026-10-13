using System;
using System.Collections.Generic;

namespace BloomGarden.Core.Model
{
    /// <summary>A watering can as declared in a level file. Immutable.</summary>
    public sealed class CanDefinition
    {
        public string Id { get; }
        public ColorId Color { get; }
        public int Capacity { get; }

        public CanDefinition(string id, ColorId color, int capacity)
        {
            Id = id;
            Color = color;
            Capacity = capacity;
        }
    }

    /// <summary>
    /// Versioned, immutable description of a level (contract of Kit/levels/README.md).
    /// Cells are stored in reading order (row-major: top to bottom, left to right);
    /// each stack lists its buds from the visible layer down to the hidden ones.
    /// </summary>
    public sealed class LevelDefinition
    {
        public const string SupportedRulesVersion = "0.1.0";
        public const int LaneCount = 3;
        public const int MvpSlotCount = 3;
        public const int MaxLayersPerCell = 3;
        public const int MaxGridSize = 7;

        public string Id { get; }
        public string RulesVersion { get; }
        public string Title { get; }
        public string Theme { get; }
        public int SlotCount { get; }
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyList<IReadOnlyList<ColorId>> Cells { get; }
        public IReadOnlyList<IReadOnlyList<CanDefinition>> Lanes { get; }

        /// <summary>Lane indices of a known solution; empty when the file has none.</summary>
        public IReadOnlyList<int> ReferenceSolution { get; }

        public int CellCount => Cells.Count;

        public LevelDefinition(
            string id,
            string rulesVersion,
            string title,
            string theme,
            int slotCount,
            int width,
            int height,
            IEnumerable<IEnumerable<ColorId>> cells,
            IEnumerable<IEnumerable<CanDefinition>> lanes,
            IEnumerable<int> referenceSolution = null)
        {
            if (cells == null) throw new ArgumentNullException(nameof(cells));
            if (lanes == null) throw new ArgumentNullException(nameof(lanes));

            Id = id;
            RulesVersion = rulesVersion;
            Title = title;
            Theme = theme;
            SlotCount = slotCount;
            Width = width;
            Height = height;
            Cells = Freeze(cells);
            Lanes = Freeze(lanes);
            ReferenceSolution = Array.AsReadOnly(new List<int>(referenceSolution ?? Array.Empty<int>()).ToArray());
        }

        public int RowOf(int cellIndex) => Width > 0 ? cellIndex / Width : 0;
        public int ColumnOf(int cellIndex) => Width > 0 ? cellIndex % Width : 0;

        public int TotalBuds
        {
            get
            {
                int total = 0;
                for (int i = 0; i < Cells.Count; i++) total += Cells[i].Count;
                return total;
            }
        }

        private static IReadOnlyList<IReadOnlyList<T>> Freeze<T>(IEnumerable<IEnumerable<T>> source)
        {
            var outer = new List<IReadOnlyList<T>>();
            foreach (var inner in source)
            {
                outer.Add(Array.AsReadOnly(new List<T>(inner ?? Array.Empty<T>()).ToArray()));
            }
            return outer.AsReadOnly();
        }
    }
}
