using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;

namespace BloomGarden.Presentation
{
    /// <summary>A tutorial hint: localization key and, optionally, the lane whose head should be pointed at (-1 = none).</summary>
    public readonly struct TutorialHint
    {
        public string Key { get; }
        public int Lane { get; }
        public bool IsNone => Key == null;

        public TutorialHint(string key, int lane = -1)
        {
            Key = key;
            Lane = lane;
        }

        public static TutorialHint None => default;
    }

    /// <summary>
    /// In-game tutorial (P5-03, GDD chapter 08: "playable in 3 taps, understood without explanation").
    /// One idea per level, shown only until the level has been completed once:
    /// level 1 tap + automatic watering, level 2 the three places, level 3 free undo, level 4 hidden flowers.
    /// Pure function of the level and the current state, so it is testable and never blocks a move.
    /// </summary>
    public static class Tutorial
    {
        public static TutorialHint For(int levelIndex, LevelDefinition level, GameState state, bool levelCompletedBefore)
        {
            if (levelCompletedBefore || state.IsWon || state.IsDeadlocked) return TutorialHint.None;
            switch (levelIndex)
            {
                case 0:
                    return state.MovesMade == 0 ? new TutorialHint("tut_tap", FirstSelectableLane(level, state)) : new TutorialHint("tut_auto");
                case 1:
                    return state.MovesMade <= 1 ? new TutorialHint("tut_places") : TutorialHint.None;
                case 2:
                    return state.MovesMade <= 1 ? new TutorialHint("tut_undo") : TutorialHint.None;
                case 3:
                    return state.MovesMade <= 1 ? new TutorialHint("tut_hidden") : TutorialHint.None;
                default:
                    return TutorialHint.None;
            }
        }

        /// <summary>The lane of the reference solution's first move when available, else the first legal lane.</summary>
        private static int FirstSelectableLane(LevelDefinition level, GameState state)
        {
            if (level.ReferenceSolution.Count > 0 && TurnResolver.CanSelect(level, state, level.ReferenceSolution[0]))
                return level.ReferenceSolution[0];
            for (int lane = 0; lane < level.Lanes.Count; lane++)
                if (TurnResolver.CanSelect(level, state, lane)) return lane;
            return -1;
        }
    }
}
