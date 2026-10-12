using System;
using System.Collections.Generic;
using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;

namespace BloomGarden.Solvers
{
    public enum SolverStatus
    {
        /// <summary>A winning sequence was found (and is returned).</summary>
        Solvable = 0,
        /// <summary>The whole reachable space was explored without reaching a win.</summary>
        Unsolvable = 1,
        /// <summary>Exploration stopped at the state limit: nothing is known. Never treat as solvable.</summary>
        Unknown = 2,
    }

    public sealed class SolveResult
    {
        public SolverStatus Status { get; }

        /// <summary>Lane indices of a shortest solution; empty unless <see cref="Status"/> is Solvable.</summary>
        public IReadOnlyList<int> SolutionLanes { get; }

        /// <summary>Number of positions taken out of the BFS queue (same counting as solver_reference.py).</summary>
        public int StatesExplored { get; }

        /// <summary>Length of the shortest solution, or -1 when not solvable.</summary>
        public int ShortestMoves => Status == SolverStatus.Solvable ? SolutionLanes.Count : -1;

        public bool IsSolvable => Status == SolverStatus.Solvable;

        internal SolveResult(SolverStatus status, IReadOnlyList<int> solutionLanes, int statesExplored)
        {
            Status = status;
            SolutionLanes = solutionLanes;
            StatesExplored = statesExplored;
        }
    }

    /// <summary>
    /// Exhaustive breadth-first solver (GDD chapter 09). Runs the real <see cref="TurnResolver"/>, so it can
    /// never disagree with the game. Positions are deduplicated with <see cref="GameState.ComputePositionKey"/>;
    /// lanes are tried 0, 1, 2, so the returned solution is the same as Kit/tools/solver_reference.py.
    /// </summary>
    public static class LevelSolver
    {
        public const int DefaultMaxStates = 150000;

        public static SolveResult Solve(LevelDefinition level, int maxStates = DefaultMaxStates)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));
            if (maxStates < 1) throw new ArgumentOutOfRangeException(nameof(maxStates));

            GameState start = GameState.CreateInitial(level);
            string startKey = start.ComputePositionKey();
            var parents = new Dictionary<string, (string parent, int lane)> { [startKey] = (null, -1) };
            var queue = new Queue<(GameState state, string key)>();
            queue.Enqueue((start, startKey));
            int explored = 0;

            while (queue.Count > 0)
            {
                var (state, key) = queue.Dequeue();
                explored++;
                if (state.IsWon)
                    return new SolveResult(SolverStatus.Solvable, BuildPath(parents, key), explored);
                if (explored >= maxStates)
                    return new SolveResult(SolverStatus.Unknown, Array.Empty<int>(), explored);

                for (int lane = 0; lane < level.Lanes.Count; lane++)
                {
                    if (!TurnResolver.CanSelect(level, state, lane)) continue;
                    GameState next = TurnResolver.SelectLane(level, state, lane).State;
                    string nextKey = next.ComputePositionKey();
                    if (parents.ContainsKey(nextKey)) continue;
                    parents[nextKey] = (key, lane);
                    queue.Enqueue((next, nextKey));
                }
            }
            return new SolveResult(SolverStatus.Unsolvable, Array.Empty<int>(), explored);
        }

        private static IReadOnlyList<int> BuildPath(Dictionary<string, (string parent, int lane)> parents, string key)
        {
            var path = new List<int>();
            while (true)
            {
                var (parent, lane) = parents[key];
                if (parent == null) break;
                path.Add(lane);
                key = parent;
            }
            path.Reverse();
            return path.AsReadOnly();
        }

        /// <summary>Plays the lanes from the initial state; true when every move is legal and the level ends won.</summary>
        public static bool ReplayWins(LevelDefinition level, IEnumerable<int> lanes)
        {
            var session = new GameSession(level);
            return session.Replay(lanes) && session.State.IsWon;
        }
    }
}
