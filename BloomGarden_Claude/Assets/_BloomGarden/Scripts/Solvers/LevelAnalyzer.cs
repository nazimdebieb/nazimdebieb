using System;
using System.Collections.Generic;
using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;

namespace BloomGarden.Solvers
{
    /// <summary>Shape of the full reachable state graph of a level. Same definitions as Tools/python/generate_solver_fixture.py.</summary>
    public sealed class LevelAnalysis
    {
        /// <summary>False when the state limit was hit: every count is then a lower bound.</summary>
        public bool Complete { get; }
        public int ReachableStates { get; }
        public int WinStates { get; }
        /// <summary>Positions with buds left and no legal move (GDD 5.5).</summary>
        public int DeadlockStates { get; }
        /// <summary>Non-terminal positions offering at least two legal lanes.</summary>
        public int DecisionStates { get; }
        /// <summary>Legal moves that turn a still-winnable position into one that can no longer be won: the traps.</summary>
        public int LosingMoves { get; }

        /// <summary>No move can lose the level: any sequence of taps wins (decision D-107).</summary>
        public bool IsTrivial => Complete && LosingMoves == 0;

        internal LevelAnalysis(bool complete, int reachable, int wins, int deadlocks, int decisions, int losingMoves)
        {
            Complete = complete;
            ReachableStates = reachable;
            WinStates = wins;
            DeadlockStates = deadlocks;
            DecisionStates = decisions;
            LosingMoves = losingMoves;
        }
    }

    /// <summary>
    /// Exhaustive exploration of every reachable position, then backward propagation of "can still win".
    /// Used for the difficulty report (P2-07), not at runtime.
    /// </summary>
    public static class LevelAnalyzer
    {
        public static LevelAnalysis Analyze(LevelDefinition level, int maxStates = LevelSolver.DefaultMaxStates)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));

            var index = new Dictionary<string, int>();
            var states = new List<GameState>();
            var successors = new List<List<int>>();
            GameState start = GameState.CreateInitial(level);
            index[start.ComputePositionKey()] = 0;
            states.Add(start);
            successors.Add(null);

            bool complete = true;
            for (int i = 0; i < states.Count; i++)
            {
                var next = new List<int>(level.Lanes.Count);
                successors[i] = next;
                GameState state = states[i];
                for (int lane = 0; lane < level.Lanes.Count; lane++)
                {
                    if (!TurnResolver.CanSelect(level, state, lane)) continue;
                    GameState child = TurnResolver.SelectLane(level, state, lane).State;
                    string key = child.ComputePositionKey();
                    if (!index.TryGetValue(key, out int id))
                    {
                        if (states.Count >= maxStates) { complete = false; continue; }
                        id = states.Count;
                        index[key] = id;
                        states.Add(child);
                        successors.Add(null);
                    }
                    next.Add(id);
                }
            }

            int n = states.Count;
            var predecessors = new List<int>[n];
            for (int i = 0; i < n; i++) predecessors[i] = new List<int>();
            for (int i = 0; i < n; i++)
                foreach (int j in successors[i]) predecessors[j].Add(i);

            var winnable = new bool[n];
            var queue = new Queue<int>();
            int wins = 0, deadlocks = 0, decisions = 0;
            for (int i = 0; i < n; i++)
            {
                if (states[i].IsWon) { wins++; winnable[i] = true; queue.Enqueue(i); }
                else if (successors[i].Count == 0) deadlocks++;
                else if (successors[i].Count >= 2) decisions++;
            }
            while (queue.Count > 0)
            {
                int j = queue.Dequeue();
                foreach (int i in predecessors[j])
                {
                    if (winnable[i]) continue;
                    winnable[i] = true;
                    queue.Enqueue(i);
                }
            }

            int losing = 0;
            for (int i = 0; i < n; i++)
            {
                if (!winnable[i]) continue;
                foreach (int j in successors[i]) if (!winnable[j]) losing++;
            }
            return new LevelAnalysis(complete, n, wins, deadlocks, decisions, losing);
        }
    }
}
