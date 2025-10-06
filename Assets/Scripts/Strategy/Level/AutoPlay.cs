using System.Collections;
using System.Collections.Generic;
using Engine.ShelfPuzzle;
using Game;
using UnityEngine;

namespace Strategy.Level
{
    public static class AutoPlay
    {
        public static void Play(LevelView levelView, List<ShelfPuzzleNode> solution)
        {
            levelView.StartCoroutine(Start(levelView, solution));
        }
        
        private static IEnumerator Start(LevelView levelView, List<ShelfPuzzleNode> solution)
        {
            for (var i = 0; i < solution.Count; i++)
            {
                if (i == 0)
                {
                    // ignore first move 
                    continue;
                }

                var state = solution[i];
                var move = state.LastMove;
                // Move Item id: {move.Item} từ Shelf {move.From} đến Shelf {move.To}
                var from = levelView.Shelves[move.From];
                var to = levelView.Shelves[move.To];
                var item = from.FindItem(move.Item);
                if (item == null)
                {
                    Debug.LogError($"Không tìm thấy Item #{move.Item} ở Shelf #{from.Id}");
                    yield break;
                }

                if (to is not CommonShelve toCm)
                {
                    Debug.LogError($"To #{to.Id} không phải là Common Shelf");
                    yield break;
                }

                var drop = toCm.FindAnyEmptyZone();
                if (!drop)
                {
                    Debug.LogError($"To #{to.Id} không có Drop Zone nào trống");
                    yield break;
                }

                if (!drop.CanAcceptItem(item.Drag))
                {
                    Debug.LogError($"To #{to.Id} không chấp nhận Drag #{item.Goods.Id}");
                    yield break;
                }

                drop.AcceptItem(item.Drag);

                yield return new WaitForSeconds(2f);
            }
        }
    }
}