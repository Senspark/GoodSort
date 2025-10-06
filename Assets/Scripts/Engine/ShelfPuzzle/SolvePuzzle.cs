using System.Collections.Generic;
using System.Linq;
using Engine.AStar;

namespace Engine.ShelfPuzzle
{
    public class PuzzleSolver
    {
        private readonly IEngineLogger _logger;

        public PuzzleSolver(IEngineLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Solve a shelf puzzle with one line of code
        /// </summary>
        /// <param name="puzzle">The puzzle configuration as ShelfPuzzleInputData array</param>
        /// <returns>Array of solution steps, or null if no solution</returns>
        public List<ShelfPuzzleNode>? SolvePuzzle(ShelfPuzzleInputData[] puzzle)
        {
            var puzzleInstance = new ShelfPuzzle(puzzle);
            var startNode = puzzleInstance.GetInitialState(puzzle);
            return new AStarSolver<ShelfPuzzleNode>(puzzleInstance).Solve(startNode);
        }

        /// <summary>
        /// Solve and print solution steps
        /// </summary>
        public void SolvePuzzleAndPrint(ShelfPuzzleInputData[] puzzle)
        {
            var solution = SolvePuzzle(puzzle);

            if (solution != null)
            {
                _logger.Log($"Solution found in {solution.Count - 1} moves!");
                for (int i = 0; i < solution.Count; i++)
                {
                    var step = solution[i];
                    var moveInfo = step.LastMove != null ? $" (moved {step.LastMove.Item})" : "";
                    _logger.Log($"Step {i}: Score {step.CalculateTotalScore()}{moveInfo}");
                }
            }
            else
            {
                _logger.Log("No solution found!");
            }
        }

        /// <summary>
        /// Solve and print detailed state changes (from -> to)
        /// </summary>
        public void SolvePuzzleWithStateChanges(ShelfPuzzleInputData[] puzzle)
        {
            var solution = SolvePuzzle(puzzle);

            if (solution == null)
            {
                _logger.Log("No solution found!");
                return;
            }

            _logger.Log($"\n=== Solution found in {solution.Count - 1} moves ===");

            for (int i = 0; i < solution.Count; i++)
            {
                var currentState = solution[i];

                if (i == 0)
                {
                    _logger.Log("\n🏁 INITIAL STATE:");
                    PrintState(currentState);
                }
                else
                {
                    var prevState = solution[i - 1];
                    _logger.Log(
                        $"\n📍 MOVE {i}: {currentState.LastMove?.Item} from shelf {currentState.LastMove?.From} to shelf {currentState.LastMove?.To}");

                    _logger.Log("\n  ⬅️  BEFORE:");
                    PrintState(prevState, "    ");

                    _logger.Log("\n  ➡️  AFTER:");
                    PrintState(currentState, "    ");

                    // Show what was trimmed
                    if (currentState.CompletedScore > prevState.CompletedScore)
                    {
                        var scoreIncrease = currentState.CompletedScore - prevState.CompletedScore;
                        _logger.Log($"    🎉 Layer completed and trimmed! +{scoreIncrease} points");
                    }
                }
            }

            var finalState = solution[solution.Count - 1];
            _logger.Log($"\n🏆 GOAL REACHED! Final score: {finalState.CalculateTotalScore()}");
        }

        private void PrintState(ShelfPuzzleNode state, string indent = "")
        {
            var activeScore = state.CalculateTotalScore() - state.CompletedScore;
            _logger.Log(
                $"{indent}Score: {state.CalculateTotalScore()} ({state.CompletedScore} completed + {activeScore} active)");

            var remainingItems = string.Join(", ",
                state.RemainingItemCounts.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
            _logger.Log($"{indent}Remaining items: {(string.IsNullOrEmpty(remainingItems) ? "none" : remainingItems)}");
            _logger.Log($"{indent}Active shelves:");

            for (int shelfIndex = 0; shelfIndex < state.ActiveShelves.Length; shelfIndex++)
            {
                var shelf = state.ActiveShelves[shelfIndex];
                _logger.Log($"{indent}  Shelf {shelfIndex}:");

                if (shelf.Length == 1 && shelf[0].All(item => item == 0))
                {
                    _logger.Log($"{indent}    [EMPTY BUFFER]");
                }
                else
                {
                    for (int layerIndex = 0; layerIndex < shelf.Length; layerIndex++)
                    {
                        var layer = shelf[layerIndex];
                        var layerStr = string.Join(" ", layer.Select(item => item == 0 ? "_" : item.ToString()));
                        _logger.Log($"{indent}    Layer {layerIndex}: [{layerStr}]");
                    }
                }
            }
        }
    }
}