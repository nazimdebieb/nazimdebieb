namespace BloomGarden.Core.Model
{
    /// <summary>Gameplay colors of the MVP (rules 0.1.0). Maximum 4, locked by D-002.</summary>
    public enum ColorId
    {
        Coral = 0,
        Blue = 1,
        Yellow = 2,
        Purple = 3,
    }

    public static class ColorIds
    {
        public const int Count = 4;

        /// <summary>Parses the exact lowercase identifiers used in the level JSON files.</summary>
        public static bool TryParse(string value, out ColorId color)
        {
            switch (value)
            {
                case "coral": color = ColorId.Coral; return true;
                case "blue": color = ColorId.Blue; return true;
                case "yellow": color = ColorId.Yellow; return true;
                case "purple": color = ColorId.Purple; return true;
                default: color = default; return false;
            }
        }

        public static string ToId(this ColorId color)
        {
            switch (color)
            {
                case ColorId.Coral: return "coral";
                case ColorId.Blue: return "blue";
                case ColorId.Yellow: return "yellow";
                case ColorId.Purple: return "purple";
                default: return "unknown";
            }
        }
    }
}
