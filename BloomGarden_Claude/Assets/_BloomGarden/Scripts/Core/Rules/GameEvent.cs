using BloomGarden.Core.Model;

namespace BloomGarden.Core.Rules
{
    public enum GameEventType
    {
        CanSelected = 0,
        FlowerBloomed = 1,
        CanEmptied = 2,
        Win = 3,
        Deadlock = 4,
    }

    /// <summary>
    /// Logical event emitted by <see cref="TurnResolver"/>, in resolution order. The presentation layer
    /// replays these to animate; it never decides game rules. All identifiers are stable.
    /// Fields that do not apply to an event type are -1 (indices) or null (ids).
    /// </summary>
    public readonly struct GameEvent
    {
        public GameEventType Type { get; }
        public string CanId { get; }
        public ColorId Color { get; }
        public int Lane { get; }
        public int Slot { get; }
        public int CellIndex { get; }
        public int Row { get; }
        public int Column { get; }

        /// <summary>Layer of the bloomed bud (0 = top layer of the original stack).</summary>
        public int Layer { get; }

        /// <summary>Water left in the can after this event.</summary>
        public int RemainingAfter { get; }

        private GameEvent(GameEventType type, string canId, ColorId color, int lane, int slot, int cellIndex, int row, int column, int layer, int remainingAfter)
        {
            Type = type;
            CanId = canId;
            Color = color;
            Lane = lane;
            Slot = slot;
            CellIndex = cellIndex;
            Row = row;
            Column = column;
            Layer = layer;
            RemainingAfter = remainingAfter;
        }

        public static GameEvent CanSelected(string canId, ColorId color, int lane, int slot, int capacity) =>
            new GameEvent(GameEventType.CanSelected, canId, color, lane, slot, -1, -1, -1, -1, capacity);

        public static GameEvent FlowerBloomed(string canId, ColorId color, int slot, int cellIndex, int row, int column, int layer, int remainingAfter) =>
            new GameEvent(GameEventType.FlowerBloomed, canId, color, -1, slot, cellIndex, row, column, layer, remainingAfter);

        public static GameEvent CanEmptied(string canId, ColorId color, int slot) =>
            new GameEvent(GameEventType.CanEmptied, canId, color, -1, slot, -1, -1, -1, -1, 0);

        public static GameEvent Win() =>
            new GameEvent(GameEventType.Win, null, default, -1, -1, -1, -1, -1, -1, -1);

        public static GameEvent Deadlock() =>
            new GameEvent(GameEventType.Deadlock, null, default, -1, -1, -1, -1, -1, -1, -1);

        public override string ToString()
        {
            switch (Type)
            {
                case GameEventType.CanSelected: return $"CanSelected({CanId},{Color.ToId()},lane={Lane},slot={Slot},cap={RemainingAfter})";
                case GameEventType.FlowerBloomed: return $"FlowerBloomed({CanId},{Color.ToId()},slot={Slot},cell={CellIndex}[{Row},{Column}],layer={Layer},left={RemainingAfter})";
                case GameEventType.CanEmptied: return $"CanEmptied({CanId},slot={Slot})";
                default: return Type.ToString();
            }
        }
    }
}
