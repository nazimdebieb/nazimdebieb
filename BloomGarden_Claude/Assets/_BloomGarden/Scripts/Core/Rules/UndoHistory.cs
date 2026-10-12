using System.Collections.Generic;
using BloomGarden.Core.Model;

namespace BloomGarden.Core.Rules
{
    /// <summary>
    /// Free, unlimited undo (D-003): a snapshot is pushed before every accepted selection.
    /// Snapshots are safe to share because <see cref="GameState"/> is immutable.
    /// </summary>
    public sealed class UndoHistory
    {
        private readonly Stack<GameState> _snapshots = new Stack<GameState>();

        public int Count => _snapshots.Count;
        public bool CanUndo => _snapshots.Count > 0;

        public void Push(GameState stateBeforeMove) => _snapshots.Push(stateBeforeMove);

        public bool TryPop(out GameState previous)
        {
            if (_snapshots.Count == 0)
            {
                previous = null;
                return false;
            }
            previous = _snapshots.Pop();
            return true;
        }

        public void Clear() => _snapshots.Clear();
    }
}
