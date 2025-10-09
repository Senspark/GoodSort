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

            double totalCost = 0;

            // Track item locations for distribution analysis
            var itemLocations = new Dictionary<int, List<(int shelfIdx, int layerIdx)>>();

            // Collect all item positions
            for (int shelfIdx = 0; shelfIdx < node.ActiveShelves.Length; shelfIdx++)
            {
                var shelf = node.ActiveShelves[shelfIdx];
                for (int layerIdx = 0; layerIdx < shelf.Length; layerIdx++)
                {
                    var layer = shelf[layerIdx];
                    foreach (var item in layer)
                    {
                        if (item != 0)
                        {
                            if (!itemLocations.ContainsKey(item))
                                itemLocations[item] = new List<(int, int)>();
                            itemLocations[item].Add((shelfIdx, layerIdx));
                        }
                    }
                }
            }

            // Calculate cost for each item type
            foreach (var (item, count) in node.RemainingItemCounts)
            {
                if (count <= 0) continue;

                var completeGroups = count / 3;
                var remainder = count % 3;
                var locations = itemLocations.ContainsKey(item) ? itemLocations[item] : new List<(int, int)>();

                // 1. Base cost: moves needed to complete groups
                // Each complete group needs at least 2 moves (gather 3 items)
                totalCost += completeGroups * 2;

                // 2. Remainder penalty
                if (remainder > 0)
                {
                    // Need moves to either complete or disperse
                    totalCost += Math.Min(3 - remainder, remainder);
                }

                // 3. Layer depth penalty - items in lower layers need more moves
                foreach (var location in locations)
                {
                    // Items not in top layer need extraction
                    if (location.Item2 > 0)
                    {
                        totalCost += location.Item2 * 0.5; // Penalty for buried items
                    }
                }

                // 4. Distribution penalty - scattered items are harder
                if (locations.Count > 1)
                {
                    // Calculate unique shelves
                    var uniqueShelves = locations.Select(loc => loc.Item1).Distinct().Count();
                    if (uniqueShelves > 1)
                    {
                        // Items scattered across multiple shelves need consolidation
                        totalCost += (uniqueShelves - 1) * 0.3;
                    }
                }
            }

            // 5. Buffer availability penalty
            int availableBuffers = 0;
            for (int i = 0; i < node.ActiveShelves.Length; i++)
            {
                if (node.ShelfTypes[i] == ShelfType.Common)
                {
                    var shelf = node.ActiveShelves[i];
                    if (shelf.Length == 1 && shelf[0].All(x => x == 0))
                    {
                        availableBuffers++;
                    }
                }
            }

            // Fewer buffers = harder (need at least 1 for maneuvering)
            if (availableBuffers == 0 && !node.IsGoal())
            {
                totalCost += 5; // Significant penalty for no buffers
            }

            return Math.Max(1, totalCost);
        }

        public List<ShelfPuzzleNode> GetNeighbours(ShelfPuzzleNode node)
        {
            var neighbours = new List<(ShelfPuzzleNode node, int priority)>();

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
                    if (node.ShelfTypes[toIdx] == ShelfType.Single) continue;

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
                                    // Calculate move priority (higher = better)
                                    int priority = CalculateMovePriority(item, toLayer);
                                    neighbours.Add((newNode, priority));
                                }
                            }
                        }
                    }
                }
            }

            // Sort by priority (descending) and limit to top moves
            const int MAX_NEIGHBORS = 100; // Limit branching factor
            return neighbours
                .OrderByDescending(n => n.priority)
                .Take(MAX_NEIGHBORS)
                .Select(n => n.node)
                .ToList();
        }

        /// <summary>
        /// Calculate priority for a move. Higher priority = better move.
        /// </summary>
        private int CalculateMovePriority(int item, int[] destinationLayer)
        {
            int priority = 0;

            // Count matching items already in destination
            int matchCount = destinationLayer.Count(x => x == item);

            // Highest priority: Creates a complete set (2 matching + 1 new = 3)
            if (matchCount == 2)
            {
                priority += 1000; // Very high priority - completes a layer!
            }
            // High priority: Creates a pair (1 matching + 1 new = 2)
            else if (matchCount == 1)
            {
                priority += 100; // High priority - makes progress
            }
            // Medium priority: Move to empty buffer
            else if (destinationLayer.All(x => x == 0))
            {
                priority += 10; // Buffer moves are useful for maneuvering
            }
            // Low priority: Move to layer with different items (might cause blocking)
            else
            {
                priority += 1; // Low priority - might not help
            }

            return priority;
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
                        if (node.ShelfTypes[i] == ShelfType.Single && layer.Length == 1)
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