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
        private IDragDropManager _dragDropManager;
        
        public void Play(
            LevelDataManager levelDataManager,
            IDragDropManager dragDropManager,
            List<ShelfPuzzleNode> solution
        )
        {
            _levelDataManager = levelDataManager;
            _dragDropManager = dragDropManager;
            StartCoroutine(_Play(solution));
        }
        
        public void Play(
            LevelDataManager levelDataManager,
            IDragDropManager dragDropManager,
            Move[] moves
        )
        {
            _levelDataManager = levelDataManager;
            _dragDropManager = dragDropManager;
            StartCoroutine(_Play(moves));
        }

        private IEnumerator _Play(List<ShelfPuzzleNode> solution)
        {
            var logger = new AppendLogger();
            for (var i = 0; i < solution.Count; i++)
            {
                var node = solution[i];
                if (node == null)
                {
                    // ignore null move
                    continue;
                }

                var move = node.LastMove;
                // Move Item id: {move.Item} từ Shelf {move.From} đến Shelf {move.To}
                var from = _levelDataManager.GetShelf(move.From);
                var to = _levelDataManager.GetShelf(move.To);
                var item = _levelDataManager.FindItemInShelf(from.Id, move.Item);

                PrintState(i, node, logger);

                if (item == null)
                {
                    Debug.LogError($"Không tìm thấy Item #{move.Item} ở Shelf #{from.Id}");
                    yield break;
                }

                var layer = _levelDataManager.GetTopLayer(to.Id);
                if (layer == null)
                {
                    Debug.LogError($"To #{to.Id} null");
                    yield break;
                }

                IDropZone drop;
                if (layer.Length == 0)
                {
                    drop = to.DropZones[0];
                }
                else
                {
                    var emptySlot = Array.FindIndex(layer, e => e == null);
                    if (emptySlot < 0)
                    {
                        Debug.LogError($"To #{to.Id} không có Drop Zone nào trống");
                        yield break;
                    }

                    drop = to.DropZones[emptySlot];
                }

                yield return StartCoroutine(MoveItemTo(_dragDropManager, (DragObject2)item.DragObject, (DropZone2)drop));
                yield return new WaitForSeconds(1f);
            }
        }
        
        private IEnumerator _Play(Move[] moves)
        {
            var logger = new AppendLogger();
            for (var i = 0; i < moves.Length; i++)
            {
                var move = moves[i];
                // Move Item id: {move.Item} từ Shelf {move.From} đến Shelf {move.To}
                var from = _levelDataManager.GetShelf(move.From);
                var to = _levelDataManager.GetShelf(move.To);
                var item = _levelDataManager.FindItemInShelf(from.Id, move.Item);

                PrintState(i, move, logger);

                if (item == null)
                {
                    Debug.LogError($"Không tìm thấy Item #{move.Item} ở Shelf #{from.Id}");
                    yield break;
                }

                var layer = _levelDataManager.GetTopLayer(to.Id);
                if (layer == null)
                {
                    Debug.LogError($"To #{to.Id} null");
                    yield break;
                }

                IDropZone drop;
                if (layer.Length == 0)
                {
                    drop = to.DropZones[0];
                }
                else
                {
                    var emptySlot = Array.FindIndex(layer, e => e == null);
                    if (emptySlot < 0)
                    {
                        Debug.LogError($"To #{to.Id} không có Drop Zone nào trống");
                        yield break;
                    }

                    drop = to.DropZones[emptySlot];
                }

                yield return StartCoroutine(MoveItemTo(_dragDropManager, (DragObject2)item.DragObject, (DropZone2)drop));
                yield return new WaitForSeconds(1f);
            }
        }
        
        private static IEnumerator MoveItemTo(IDragDropManager dragDropManager, DragObject2 drag, DropZone2 drop)
        {
            var posFrom = drag.transform.position;
            var posTo = drop.transform.position;
            var distance = Vector2.Distance(posFrom, posTo);
            const int step = 60;
            var speed = distance / step;
            for (var i = 0; i < step; i++)
            {
                var newPos = Vector3.MoveTowards(posFrom, posTo, speed);
                newPos.z = -9;
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
        
        private static void PrintState(int step, Move move, AppendLogger logger)
        {
            logger.Log($"Step {step}");
            logger.Log($"Move #{move.Item} from Shelf #{move.From} to Shelf #{move.To}");
            logger.PrintLogs();
        }
    }
}