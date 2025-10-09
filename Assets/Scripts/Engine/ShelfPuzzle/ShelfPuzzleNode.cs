using System;
using System.Collections.Generic;
using System.Linq;
using Engine.AStar;
using Newtonsoft.Json;

namespace Engine.ShelfPuzzle
{
    public enum ShelfType
    {
        Common, // Có 3 Slot chứa trong cùng 1 Layer
        Single, // Chỉ có thể lấy Item ra
    }

    public class ShelfPuzzleInputData
    {
        public ShelfType Type;
        /* Số lần khoá cho đến khi == 0 thì sẽ mở khoá */
        public int LockCount;
        public int[][] Data = Array.Empty<int[]>();
    }

    public class ShelfPuzzleNode : IAStarNode
    {
        public int[][][] ActiveShelves { get; set; } // Only incomplete layers
        public ShelfType[] ShelfTypes { get; set; } // Type of each shelf
        public int CompletedScore { get; set; } // Sum of completed layer scores
        public Dictionary<int, int> RemainingItemCounts { get; set; } // Track items still in play
        public int MoveCount { get; set; }
        public Move LastMove { get; set; }

        public ShelfPuzzleNode(
            int[][][] activeShelves,
            ShelfType[] shelfTypes,
            int completedScore,
            Dictionary<int, int> remainingItemCounts,
            int moveCount = 0,
            Move lastMove = null)
        {
            ActiveShelves = activeShelves;
            ShelfTypes = shelfTypes;
            CompletedScore = completedScore;
            RemainingItemCounts = remainingItemCounts;
            MoveCount = moveCount;
            LastMove = lastMove;
        }

        public string GetId()
        {
            // Must include completedScore in ID for proper state comparison
            // Format: "completedScore:shelf1|shelf2|..."
            // Example: "200:aa_,bb_|c__"
            var shelvesStr = string.Join("|",
                ActiveShelves.Select(shelf =>
                    string.Join(",", shelf.Select(layer =>
                        string.Join("", NormalizeLayer(layer))
                    ))
                )
            );
            return $"{CompletedScore}:{shelvesStr}";
        }

        public bool IsGoal()
        {
            // Goal when all items are completed (no active items remaining)
            return RemainingItemCounts.Count == 0 ||
                   RemainingItemCounts.Values.All(count => count == 0);
        }

        public IAStarNode Clone()
        {
            return new ShelfPuzzleNode(
                DeepCloneShelves(ActiveShelves),
                (ShelfType[])ShelfTypes.Clone(),
                CompletedScore,
                new Dictionary<int, int>(RemainingItemCounts),
                MoveCount,
                LastMove
            );
        }

        // Helper methods
        private int[] NormalizeLayer(int[] layer)
        {
            // Sort items for consistent comparison [a,0,a] === [a,a,0]
            return layer.OrderBy(x => x == 0 ? 1 : 0)
                .ThenBy(x => x)
                .ToArray();
        }

        private int[][][] DeepCloneShelves(int[][][] shelves)
        {
            return shelves.Select(shelf =>
                shelf.Select(layer => layer.ToArray()).ToArray()
            ).ToArray();
        }

        // Utility methods for checking layer states
        public bool IsLayerComplete(int[] layer)
        {
            var nonZero = layer.Where(x => x != 0).ToArray();
            return nonZero.Length == 3 && nonZero.All(x => x == nonZero[0]);
        }

        public bool IsLayerLocked(int[] layer)
        {
            // For interaction: ONLY layers with 3 matching items are locked (inaccessible)
            var nonZeroItems = layer.Where(item => item != 0).ToArray();

            if (nonZeroItems.Length == 3)
            {
                return nonZeroItems.All(item => item == nonZeroItems[0]);
            }

            return false;
        }

        public int? GetTopAccessibleLayerIndex(int shelfIndex)
        {
            var shelf = ActiveShelves[shelfIndex];
            for (int i = 0; i < shelf.Length; i++)
            {
                if (!IsLayerLocked(shelf[i]))
                {
                    return i;
                }
            }

            return null; // All layers are locked (all have 3 matching items)
        }

        public bool CanTakeFrom(int shelfIndex)
        {
            // Find the topmost layer that has items AND is not locked
            var shelf = ActiveShelves[shelfIndex];
            for (int i = 0; i < shelf.Length; i++)
            {
                var layer = shelf[i];
                if (!IsLayerLocked(layer) && layer.Any(item => item != 0))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanPlaceTo(int shelfIndex)
        {
            // Find the topmost layer that has empty slots AND is not locked
            var shelf = ActiveShelves[shelfIndex];
            for (int i = 0; i < shelf.Length; i++)
            {
                var layer = shelf[i];
                if (!IsLayerLocked(layer) && layer.Any(item => item == 0))
                {
                    return true;
                }
            }

            return false;
        }

        public int? GetTopAccessibleLayerForTaking(int shelfIndex)
        {
            // Find the topmost layer that has items AND is not locked
            var shelf = ActiveShelves[shelfIndex];
            for (int i = 0; i < shelf.Length; i++)
            {
                var layer = shelf[i];
                if (!IsLayerLocked(layer) && layer.Any(item => item != 0))
                {
                    return i;
                }
            }

            return null;
        }

        public int? GetTopAccessibleLayerForPlacing(int shelfIndex)
        {
            // Find the topmost layer that has empty slots AND is not locked
            var shelf = ActiveShelves[shelfIndex];
            for (int i = 0; i < shelf.Length; i++)
            {
                var layer = shelf[i];
                if (!IsLayerLocked(layer) && layer.Any(item => item == 0))
                {
                    return i;
                }
            }

            return null;
        }

        // Calculate total score including completed layers
        public int CalculateTotalScore()
        {
            int activeScore = 0;

            foreach (var shelf in ActiveShelves)
            {
                foreach (var layer in shelf)
                {
                    var nonZeroItems = layer.Where(item => item != 0).ToArray();

                    if (nonZeroItems.Length == 0)
                    {
                        // Empty layer: 0 points
                        activeScore += 0;
                    }
                    else if (nonZeroItems.Length == 3)
                    {
                        var allSame = nonZeroItems.All(item => item == nonZeroItems[0]);
                        if (allSame)
                        {
                            // 3 matching items: 100 points
                            activeScore += 100;
                        }
                        else
                        {
                            // 3 different items: 1 point
                            activeScore += 1;
                        }
                    }
                    else
                    {
                        // Check for matching pairs
                        var itemCounts = new Dictionary<int, int>();
                        foreach (var item in nonZeroItems)
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

                        var hasMatchingPair = itemCounts.Values.Any(count => count >= 2);
                        if (hasMatchingPair)
                        {
                            // 2 matching items: 10 points
                            activeScore += 10;
                        }
                        else
                        {
                            // Different items: 1 point
                            activeScore += 1;
                        }
                    }
                }
            }

            return CompletedScore + activeScore;
        }
    }

    public class Move
    {
        public int From { get; }
        public int To { get; }
        public int Item { get; }

        public Move(int from, int to, int item)
        {
            From = from;
            To = to;
            Item = item;
        }
    }
}