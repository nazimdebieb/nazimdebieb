using System;
using System.Collections.Generic;

namespace BloomGarden.Core.Model
{
    public enum GameStatus
    {
        InProgress = 0,
        Won = 1,
        /// <summary>Stable state with buds left and no legal selection (GDD 5.5).</summary>
        Deadlocked = 2,
    }

    /// <summary>One of the 3 active watering places. Empty when <see cref="IsOccupied"/> is false.</summary>
    public readonly struct SlotState : IEquatable<SlotState>
    {
        public bool IsOccupied { get; }
        public string CanId { get; }
        public int Lane { get; }
        public ColorId Color { get; }
        public int Capacity { get; }
        public int Remaining { get; }

        /// <summary>Monotonic selection order; lower = older. Older places are evaluated first (GDD 5.4).</summary>
        public int Order { get; }

        public SlotState(string canId, int lane, ColorId color, int capacity, int remaining, int order)
        {
            IsOccupied = true;
            CanId = canId;
            Lane = lane;
            Color = color;
            Capacity = capacity;
            Remaining = remaining;
            Order = order;
        }

        public static SlotState Empty => default;

        public SlotState WithRemaining(int remaining) => new SlotState(CanId, Lane, Color, Capacity, remaining, Order);

        public bool Equals(SlotState other) =>
            IsOccupied == other.IsOccupied && CanId == other.CanId && Lane == other.Lane && Color == other.Color &&
            Capacity == other.Capacity && Remaining == other.Remaining && Order == other.Order;

        public override bool Equals(object obj) => obj is SlotState other && Equals(other);

        public override int GetHashCode() => IsOccupied ? (Order * 397) ^ Remaining : 0;
    }

    /// <summary>
    /// Complete logical state of a game in progress. Immutable from the outside: every move produces
    /// a new instance (see TurnResolver), which makes undo snapshots safe and replays deterministic.
    /// Contains no timing or animation data.
    /// </summary>
    public sealed class GameState
    {
        private readonly int[] _bloomed;
        private readonly int[] _laneHeads;
        private readonly SlotState[] _slots;

        public GameStatus Status { get; }
        public int MovesMade { get; }
        public int NextOrder { get; }

        internal GameState(int[] bloomed, int[] laneHeads, SlotState[] slots, int nextOrder, int movesMade, GameStatus status)
        {
            _bloomed = bloomed;
            _laneHeads = laneHeads;
            _slots = slots;
            NextOrder = nextOrder;
            MovesMade = movesMade;
            Status = status;
        }

        public static GameState CreateInitial(LevelDefinition level)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));
            var bloomed = new int[level.CellCount];
            var heads = new int[level.Lanes.Count];
            var slots = new SlotState[level.SlotCount];
            GameStatus status = level.TotalBuds == 0 ? GameStatus.Won : GameStatus.InProgress;
            return new GameState(bloomed, heads, slots, 0, 0, status);
        }

        public int CellCount => _bloomed.Length;
        public int LaneCount => _laneHeads.Length;
        public int SlotCount => _slots.Length;

        /// <summary>Number of buds already bloomed in the given cell (= index of the visible layer).</summary>
        public int BloomedInCell(int cellIndex) => _bloomed[cellIndex];

        /// <summary>Index, inside the lane, of the can currently at the head of the lane.</summary>
        public int LaneHead(int lane) => _laneHeads[lane];

        public SlotState GetSlot(int slotIndex) => _slots[slotIndex];

        public bool IsWon => Status == GameStatus.Won;
        public bool IsDeadlocked => Status == GameStatus.Deadlocked;

        public int OccupiedSlotCount
        {
            get
            {
                int n = 0;
                foreach (var s in _slots) if (s.IsOccupied) n++;
                return n;
            }
        }

        public bool HasFreeSlot => OccupiedSlotCount < _slots.Length;

        /// <summary>The single visible (targetable) bud of a cell, if any. Hidden layers are never exposed as targets.</summary>
        public bool TryGetVisibleBud(LevelDefinition level, int cellIndex, out ColorId color)
        {
            var stack = level.Cells[cellIndex];
            int layer = _bloomed[cellIndex];
            if (layer < stack.Count)
            {
                color = stack[layer];
                return true;
            }
            color = default;
            return false;
        }

        public bool TryGetLaneHead(LevelDefinition level, int lane, out CanDefinition can)
        {
            if (lane >= 0 && lane < _laneHeads.Length && _laneHeads[lane] < level.Lanes[lane].Count)
            {
                can = level.Lanes[lane][_laneHeads[lane]];
                return true;
            }
            can = null;
            return false;
        }

        public int RemainingBuds(LevelDefinition level)
        {
            int remaining = 0;
            for (int i = 0; i < _bloomed.Length; i++) remaining += level.Cells[i].Count - _bloomed[i];
            return remaining;
        }

        internal int[] CopyBloomed() => (int[])_bloomed.Clone();
        internal int[] CopyLaneHeads() => (int[])_laneHeads.Clone();
        internal SlotState[] CopySlots() => (SlotState[])_slots.Clone();

        /// <summary>
        /// Deterministic 64-bit FNV-1a hash of the full state (cells, lanes, slots, counters, status).
        /// Stable across runs and platforms (does not use string.GetHashCode).
        /// </summary>
        public ulong ComputeHash()
        {
            ulong h = 14695981039346656037UL;
            Mix(ref h, _bloomed.Length);
            foreach (int b in _bloomed) Mix(ref h, b);
            Mix(ref h, _laneHeads.Length);
            foreach (int l in _laneHeads) Mix(ref h, l);
            Mix(ref h, _slots.Length);
            foreach (SlotState s in _slots)
            {
                Mix(ref h, s.IsOccupied ? 1 : 0);
                if (!s.IsOccupied) continue;
                MixString(ref h, s.CanId);
                Mix(ref h, s.Lane);
                Mix(ref h, (int)s.Color);
                Mix(ref h, s.Capacity);
                Mix(ref h, s.Remaining);
                Mix(ref h, s.Order);
            }
            Mix(ref h, NextOrder);
            Mix(ref h, MovesMade);
            Mix(ref h, (int)Status);
            return h;
        }

        /// <summary>
        /// Canonical key of the puzzle position, independent of move count and absolute selection numbers:
        /// two states with the same key have exactly the same future. Intended for the solver (P2).
        /// Active cans are listed oldest first, matching the evaluation order.
        /// </summary>
        public string ComputePositionKey()
        {
            var parts = new List<string>(3);
            parts.Add(string.Join(",", _bloomed));
            parts.Add(string.Join(",", _laneHeads));
            var active = new List<SlotState>();
            foreach (var s in _slots) if (s.IsOccupied) active.Add(s);
            active.Sort((a, b) => a.Order.CompareTo(b.Order));
            var slotParts = new List<string>(active.Count);
            foreach (var s in active) slotParts.Add(s.CanId + ":" + s.Remaining);
            parts.Add(string.Join(",", slotParts));
            return string.Join("|", parts);
        }

        private static void Mix(ref ulong h, int value)
        {
            unchecked
            {
                for (int i = 0; i < 4; i++)
                {
                    h ^= (byte)(value >> (8 * i));
                    h *= 1099511628211UL;
                }
            }
        }

        private static void MixString(ref ulong h, string value)
        {
            if (value == null) { Mix(ref h, -1); return; }
            Mix(ref h, value.Length);
            foreach (char c in value) Mix(ref h, c);
        }
    }
}
