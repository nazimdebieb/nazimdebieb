using System;
using System.Collections.Generic;
using BloomGarden.Core.Model;

namespace BloomGarden.Core.Rules
{
    /// <summary>
    /// Single entry point for the presentation layer: current state + undo history for one level.
    /// One call = one logical move; events are returned for the views to animate afterwards.
    /// </summary>
    public sealed class GameSession
    {
        private readonly UndoHistory _history = new UndoHistory();

        public LevelDefinition Level { get; }
        public GameState State { get; private set; }
        public bool CanUndo => _history.CanUndo;

        public event Action<GameState> StateChanged;

        public GameSession(LevelDefinition level)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            State = GameState.CreateInitial(level);
        }

        public MoveResult SelectLane(int lane)
        {
            MoveResult result = TurnResolver.SelectLane(Level, State, lane);
            if (!result.Accepted) return result;
            _history.Push(State);
            State = result.State;
            StateChanged?.Invoke(State);
            return result;
        }

        public bool Undo()
        {
            if (!_history.TryPop(out GameState previous)) return false;
            State = previous;
            StateChanged?.Invoke(State);
            return true;
        }

        public void Restart()
        {
            _history.Clear();
            State = GameState.CreateInitial(Level);
            StateChanged?.Invoke(State);
        }

        /// <summary>Plays a sequence of lane indices; stops at the first rejected move.</summary>
        public bool Replay(IEnumerable<int> lanes, List<GameEvent> eventsOut = null)
        {
            foreach (int lane in lanes)
            {
                MoveResult result = SelectLane(lane);
                if (!result.Accepted) return false;
                eventsOut?.AddRange(result.Events);
            }
            return true;
        }
    }
}
