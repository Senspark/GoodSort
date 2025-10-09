using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Engine.ShelfPuzzle;
using manager;
using UnityEngine;

namespace Strategy.Level
{
    public class AutoPlay2 : MonoBehaviour
    {
        private LevelDataManager _levelDataManager;
        private IDragDropGameManager _dragDropManager;


        public void Play(
            LevelDataManager levelDataManager,
            IDragDropGameManager dragDropManager,
            List<ShelfPuzzleNode> solution
        )
        {
            _levelDataManager = levelDataManager;
            _dragDropManager = dragDropManager;
            StartCoroutine(_Play(solution));
        }

        private IEnumerator _Play(List<ShelfPuzzleNode> solution)
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
                var from = _levelDataManager.GetShelf(move.From);
                var to = _levelDataManager.GetShelf(move.To);
                var item = _levelDataManager.FindItemInShelf(move.From, move.Item);

                PrintState(i, node, logger);

                if (item == null)
                {
                    Debug.LogError($"Không tìm thấy Item #{move.Item} ở Shelf #{from.Id}");
                    yield break;
                }

                var emptySlot = Array.FindIndex(_levelDataManager.GetTopLayer(move.To), e => e == null);
                if (emptySlot < 0)
                {
                    Debug.LogError($"To #{to.Id} không có Drop Zone nào trống");
                    yield break;
                }

                var drop = to.DropZones[emptySlot];

                yield return StartCoroutine(MoveItemTo(_dragDropManager, (DragObject)item.DragObject, (DropZone)drop));
                yield return new WaitForSeconds(1f);
            }
        }
        
        private static IEnumerator MoveItemTo(IDragDropGameManager dragDropManager, DragObject drag, DropZone drop)
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

            dragDropManager.ManualDropInto(drag, drop);
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
    }
}