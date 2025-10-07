using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Engine.ShelfPuzzle;
using Game;
using manager;
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
            var logger = new AppendLogger();
            for (var i = 0; i < solution.Count; i++)
            {
                if (i == 0)
                {
                    // ignore first move 
                    continue;
                }

                var node = solution[i];
                var move = node.LastMove;
                // Move Item id: {move.Item} từ Shelf {move.From} đến Shelf {move.To}
                var from = levelView.Shelves[move.From];
                var to = levelView.Shelves[move.To];
                var item = from.FindItem(move.Item);

                PrintState(i, node, logger);

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

                yield return levelView.StartCoroutine(MoveItemTo(item.Drag, drop));
                yield return new WaitForSeconds(1f);
            }
        }

        private static void PrintState(int step, ShelfPuzzleNode node, AppendLogger logger)
        {
            logger.Log($"Step {step}");
            
            if (node.ActiveShelves != null)
            {
                logger.Log("[");
                foreach (var shelf in node.ActiveShelves)
                {
                    logger.Log($"\t{string.Join(" ", shelf.Select(e => "[" + string.Join(",", e) + "]"))}");
                }

                logger.Log("]");
            }

            logger.Log($"Move #{node.LastMove.Item} from Shelf #{node.LastMove.From} to Shelf #{node.LastMove.To}");
            logger.PrintLogs();
        }

        private static IEnumerator MoveItemTo(DragDrop drag, DropZone drop)
        {
            var posFrom = drag.transform.position;
            var posTo = drop.transform.position;
            var distance = Vector2.Distance(posFrom, posTo);
            const int step = 60;
            var speed = distance / step;
            for (var i = 0; i < step; i++)
            {
                var newPos = Vector2.MoveTowards(posFrom, posTo, speed);
                drag.transform.position = newPos;
                posFrom = newPos;
                yield return null;
            }

            drop.AcceptItem(drag);
        }
    }
}