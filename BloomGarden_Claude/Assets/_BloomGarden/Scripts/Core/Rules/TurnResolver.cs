using System;
using System.Collections.Generic;
using BloomGarden.Core.Model;

namespace BloomGarden.Core.Rules
{
    public enum MoveRejection
    {
        None = 0,
        GameOver = 1,
        InvalidLane = 2,
        LaneEmpty = 3,
        SlotsFull = 4,
    }

    public sealed class MoveResult
    {
        public bool Accepted => Rejection == MoveRejection.None;
        public MoveRejection Rejection { get; }

        /// <summary>The new state when accepted; the unchanged input state when rejected.</summary>
        public GameState State { get; }

        public IReadOnlyList<GameEvent> Events { get; }

        internal MoveResult(MoveRejection rejection, GameState state, IReadOnlyList<GameEvent> events)
        {
            Rejection = rejection;
            State = state;
            Events = events;
        }
    }

    /// <summary>
    /// Pure, deterministic application of rules 0.1.0 (GDD chapter 05):
    /// the head can of a lane goes to the leftmost free place, then the board is stabilized one bud at a
    /// time — active places evaluated oldest first, each watering its smallest (row, column) visible bud of
    /// its color — until no place can water anything. A can leaves its place when empty; a can with water
    /// but no visible target waits and keeps its place. No time, frame rate or animation is involved.
    /// Semantics match stabilize()/move() of Kit/tools/solver_reference.py.
    /// </summary>
    public static class TurnResolver
    {
        public static MoveResult SelectLane(LevelDefinition level, GameState state, int lane)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));
            if (state == null) throw new ArgumentNullException(nameof(state));

            MoveRejection rejection = CheckSelection(level, state, lane);
            if (rejection != MoveRejection.None)
                return new MoveResult(rejection, state, Array.Empty<GameEvent>());

            int[] bloomed = state.CopyBloomed();
            int[] heads = state.CopyLaneHeads();
            SlotState[] slots = state.CopySlots();
            var events = new List<GameEvent>();

            CanDefinition can = level.Lanes[lane][heads[lane]];
            heads[lane]++;
            int slot = Array.FindIndex(slots, s => !s.IsOccupied);
            slots[slot] = new SlotState(can.Id, lane, can.Color, can.Capacity, can.Capacity, state.NextOrder);
            events.Add(GameEvent.CanSelected(can.Id, can.Color, lane, slot, can.Capacity));

            Stabilize(level, bloomed, slots, events);

            GameStatus status = EvaluateStatus(level, bloomed, heads, slots);
            if (status == GameStatus.Won) events.Add(GameEvent.Win());
            else if (status == GameStatus.Deadlocked) events.Add(GameEvent.Deadlock());

            var next = new GameState(bloomed, heads, slots, state.NextOrder + 1, state.MovesMade + 1, status);
            return new MoveResult(MoveRejection.None, next, events.AsReadOnly());
        }

        /// <summary>
        /// Why a lane cannot be selected, or None. A deadlocked state needs no special case:
        /// by definition every lane is then empty or every place is occupied.
        /// </summary>
        public static MoveRejection CheckSelection(LevelDefinition level, GameState state, int lane)
        {
            if (lane < 0 || lane >= level.Lanes.Count) return MoveRejection.InvalidLane;
            if (state.Status == GameStatus.Won) return MoveRejection.GameOver;
            if (state.LaneHead(lane) >= level.Lanes[lane].Count) return MoveRejection.LaneEmpty;
            if (!state.HasFreeSlot) return MoveRejection.SlotsFull;
            return MoveRejection.None;
        }

        public static bool CanSelect(LevelDefinition level, GameState state, int lane) =>
            CheckSelection(level, state, lane) == MoveRejection.None;

        private static void Stabilize(LevelDefinition level, int[] bloomed, SlotState[] slots, List<GameEvent> events)
        {
            // Slot indices sorted oldest first; re-sorted after each change because a slot may be freed.
            var order = new List<int>(slots.Length);
            while (true)
            {
                order.Clear();
                for (int i = 0; i < slots.Length; i++) if (slots[i].IsOccupied) order.Add(i);
                order.Sort((a, b) => slots[a].Order.CompareTo(slots[b].Order));

                bool progressed = false;
                foreach (int s in order)
                {
                    SlotState active = slots[s];
                    int target = FindTarget(level, bloomed, active.Color);
                    if (target < 0) continue;

                    int layer = bloomed[target];
                    bloomed[target]++;
                    int remaining = active.Remaining - 1;
                    events.Add(GameEvent.FlowerBloomed(active.CanId, active.Color, s, target,
                        level.RowOf(target), level.ColumnOf(target), layer, remaining));

                    if (remaining == 0)
                    {
                        slots[s] = SlotState.Empty;
                        events.Add(GameEvent.CanEmptied(active.CanId, active.Color, s));
                    }
                    else
                    {
                        slots[s] = active.WithRemaining(remaining);
                    }
                    progressed = true;
                    break; // one bud per logical step, then re-evaluate from the oldest place
                }
                if (!progressed) return;
            }
        }

        /// <summary>Smallest row-major cell index whose visible bud has the given color, or -1.</summary>
        private static int FindTarget(LevelDefinition level, int[] bloomed, ColorId color)
        {
            for (int i = 0; i < bloomed.Length; i++)
            {
                var stack = level.Cells[i];
                if (bloomed[i] < stack.Count && stack[bloomed[i]] == color) return i;
            }
            return -1;
        }

        private static GameStatus EvaluateStatus(LevelDefinition level, int[] bloomed, int[] heads, SlotState[] slots)
        {
            bool allBloomed = true;
            for (int i = 0; i < bloomed.Length; i++)
            {
                if (bloomed[i] < level.Cells[i].Count) { allBloomed = false; break; }
            }
            if (allBloomed) return GameStatus.Won;

            // Called only after stabilization, so no active can has a target: the state is stable.
            bool freeSlot = Array.Exists(slots, s => !s.IsOccupied);
            bool canLeft = false;
            for (int l = 0; l < heads.Length; l++)
            {
                if (heads[l] < level.Lanes[l].Count) { canLeft = true; break; }
            }
            return freeSlot && canLeft ? GameStatus.InProgress : GameStatus.Deadlocked;
        }
    }
}
