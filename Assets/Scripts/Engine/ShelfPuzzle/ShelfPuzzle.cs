using System;
using System.Collections.Generic;
using System.Linq;
using Engine.AStar;

namespace Engine.ShelfPuzzle
{
    public class ShelfPuzzle : IAStarPuzzle<ShelfPuzzleNode>
    {
        private readonly int targetScore;

        public ShelfPuzzle(ShelfPuzzleInputData[] inputData)
        {
            // Preprocess and calculate target score
            var processed = PreprocessPuzzle(inputData);
            targetScore = CalculateTargetScore(processed);
        }

        public double CalculateG(ShelfPuzzleNode node, ShelfPuzzleNode parent)
        {
            return node.MoveCount;
        }

        public double CalculateH(ShelfPuzzleNode node)
        {
            if (node.IsGoal()) return 0;

            // Improved heuristic based on remaining items
            int estimatedMoves = 0;

            foreach (var (item, count) in node.RemainingItemCounts)
            {
                if (count > 0)
                {
                    // Each group of 3 needs to be completed
                    var completeGroups = count / 3;
                    var remainder = count % 3;

                    if (remainder > 0)
                    {
                        // Incomplete group needs moves to either:
                        // - Get to 3 (needs 3-remainder items)
                        // - Disperse to other groups
                        estimatedMoves += Math.Min(3 - remainder, remainder);
                    }
                }
            }

            return Math.Max(1, estimatedMoves);
        }

        public List<ShelfPuzzleNode> GetNeighbours(ShelfPuzzleNode node)
        {
            var neighbours = new List<ShelfPuzzleNode>();

            // For each shelf that can provide items
            for (int fromIdx = 0; fromIdx < node.ActiveShelves.Length; fromIdx++)
            {
                var fromShelf = node.ActiveShelves[fromIdx];
                if (fromShelf.Length == 0) continue;

                var fromLayer = fromShelf[0]; // Always top layer after trimming
                var availableItems = fromLayer.Where(item => item != 0).ToArray();

                if (availableItems.Length == 0) continue;

                // For each shelf that can receive items
                for (int toIdx = 0; toIdx < node.ActiveShelves.Length; toIdx++)
                {
                    if (fromIdx == toIdx) continue;

                    // Skip TakeOnly shelves as destinations
                    if (node.ShelfTypes[toIdx] == ShelfType.TakeOnly) continue;

                    var toShelf = node.ActiveShelves[toIdx];
                    if (toShelf.Length == 0) continue;

                    var toLayer = toShelf[0];
                    var emptySlots = toLayer.Count(item => item == 0);

                    if (emptySlots > 0)
                    {
                        // Create neighbour for each unique item type that can be moved
                        var uniqueItems = availableItems.Distinct().ToArray();

                        foreach (var item in uniqueItems)
                        {
                            if (item != 0)
                            {
                                var newNode = ExecuteMove(node, fromIdx, toIdx, item);
                                if (newNode != null)
                                {
                                    neighbours.Add(newNode);
                                }
                            }
                        }
                    }
                }
            }

            return neighbours;
        }

        private ShelfPuzzleNode? ExecuteMove(
            ShelfPuzzleNode node,
            int fromIdx,
            int toIdx,
            int item)
        {
            var newNode = (ShelfPuzzleNode)node.Clone();
            newNode.MoveCount++;
            newNode.LastMove = new Move(fromIdx, toIdx, item);

            // Remove item from source
            var fromLayer = newNode.ActiveShelves[fromIdx][0];
            var itemIndex = Array.IndexOf(fromLayer, item);
            if (itemIndex == -1) return null;
            fromLayer[itemIndex] = 0;

            // Add item to destination
            var toLayer = newNode.ActiveShelves[toIdx][0];
            var emptyIndex = Array.IndexOf(toLayer, 0);
            if (emptyIndex == -1) return null;
            toLayer[emptyIndex] = item;

            // CRITICAL: Trim completed layers
            TrimCompletedLayers(newNode);

            return newNode;
        }

        private void TrimCompletedLayers(ShelfPuzzleNode node)
        {
            for (int i = 0; i < node.ActiveShelves.Length; i++)
            {
                var shelf = node.ActiveShelves[i];
                var newShelf = new List<int[]>();

                foreach (var layer in shelf)
                {
                    if (IsLayerComplete(layer))
                    {
                        // Add score and update remaining items
                        node.CompletedScore += CalculateLayerScore(layer);

                        // Update remaining item counts
                        var item = layer.FirstOrDefault(x => x != 0);
                        if (item != 0 && node.RemainingItemCounts.ContainsKey(item))
                        {
                            var count = node.RemainingItemCounts[item] - 3;
                            if (count <= 0)
                            {
                                node.RemainingItemCounts.Remove(item);
                            }
                            else
                            {
                                node.RemainingItemCounts[item] = count;
                            }
                        }
                    }
                    else if (IsLayerEmpty(layer))
                    {
                        // Trim empty layers
                        // For TakeOnly shelves, also update item count (since 1-slot layers are consumed)
                        if (node.ShelfTypes[i] == ShelfType.TakeOnly && layer.Length == 1)
                        {
                            // Item was already taken, no count update needed (handled in source removal)
                        }
                        // No score change for empty layers
                    }
                    else
                    {
                        newShelf.Add(layer);
                    }
                }

                // Maintain shelf structure based on type
                if (newShelf.Count > 0)
                {
                    node.ActiveShelves[i] = newShelf.ToArray();
                }
                else
                {
                    // Only Common shelves become empty buffers when all layers trimmed
                    if (node.ShelfTypes[i] == ShelfType.Common)
                    {
                        node.ActiveShelves[i] = new[] { new int[] { 0, 0, 0 } };
                    }
                    else
                    {
                        // TakeOnly shelf with no layers left becomes empty array
                        node.ActiveShelves[i] = new int[][] { };
                    }
                }
            }
        }

        private bool IsLayerComplete(int[] layer)
        {
            var nonZero = layer.Where(x => x != 0).ToArray();
            return nonZero.Length == 3 && nonZero.All(x => x == nonZero[0]);
        }

        private bool IsLayerEmpty(int[] layer)
        {
            return layer.All(x => x == 0);
        }

        private int CalculateLayerScore(int[] layer)
        {
            var nonZero = layer.Where(x => x != 0).ToArray();
            if (nonZero.Length == 0) return 0;
            if (nonZero.Length == 3 && nonZero.All(x => x == nonZero[0])) return 100;
            // This shouldn't happen for completed layers, but included for completeness
            return 0;
        }

        private ShelfPuzzleNode PreprocessPuzzle(ShelfPuzzleInputData[] inputData)
        {
            var activeShelves = new List<int[][]>();
            var shelfTypes = new List<ShelfType>();
            int completedScore = 0;
            var itemCounts = new Dictionary<int, int>();

            foreach (var shelfInput in inputData)
            {
                var activeLayersForShelf = new List<int[]>();
                var shelfType = shelfInput.Type;

                foreach (var layer in shelfInput.Data)
                {
                    if (IsLayerComplete(layer))
                    {
                        // Pre-trim completed layers
                        completedScore += CalculateLayerScore(layer);
                    }
                    else
                    {
                        // Keep active layers and count items
                        activeLayersForShelf.Add(layer);
                        foreach (var item in layer)
                        {
                            if (item != 0)
                            {
                                if (!itemCounts.ContainsKey(item))
                                {
                                    itemCounts[item] = 0;
                                }
                                itemCounts[item]++;
                            }
                        }
                    }
                }

                // Maintain shelf structure based on type
                if (activeLayersForShelf.Count > 0)
                {
                    activeShelves.Add(activeLayersForShelf.ToArray());
                }
                else
                {
                    // Only Common shelves can become empty buffers
                    if (shelfType == ShelfType.Common)
                    {
                        activeShelves.Add(new[] { new int[] { 0, 0, 0 } });
                    }
                    else
                    {
                        // TakeOnly shelf with no items is completely removed
                        activeShelves.Add(new int[][] { });
                    }
                }

                shelfTypes.Add(shelfType);
            }

            return new ShelfPuzzleNode(activeShelves.ToArray(), shelfTypes.ToArray(), completedScore, itemCounts, 0);
        }

        private int CalculateTargetScore(ShelfPuzzleNode processed)
        {
            // Calculate maximum possible score based on remaining items
            int maxScore = processed.CompletedScore;

            foreach (var (item, count) in processed.RemainingItemCounts)
            {
                var completeGroups = count / 3;
                maxScore += completeGroups * 100;
            }

            return maxScore;
        }

        // Public method to get preprocessed initial state
        public ShelfPuzzleNode GetInitialState(ShelfPuzzleInputData[] inputData)
        {
            return PreprocessPuzzle(inputData);
        }
    }
}