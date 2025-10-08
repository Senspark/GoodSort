using System.Collections;
using System.Linq;
using Core;
using Engine.ShelfPuzzle;
using Game;
using manager;
using Sirenix.OdinInspector;
using Strategy.Level;
using UnityEngine;

namespace UI
{
    public class TestScene : MonoBehaviour
    {
        public DragDropGameManager dragDropGameManager;
        public GameObject container;
        public ShelfItemBasic shelfItemPrefab;

        private LevelDataManager _levelDataManager;

        [Button]
        public void CreateLevel()
        {
            var levelCreator = new LevelCreator(container, shelfItemPrefab);
            var inputData = new ShelfPuzzleInputData[]
            {
                new()
                {
                    Type = ShelfType.Common,
                    Data = new[]
                    {
                        new[] { 1, 1, 0 },
                        new[] { 2, 2, 0 }
                    }
                },
                new()
                {
                    Type = ShelfType.Common,
                    Data = new[]
                    {
                        new[] { 0, 2, 1 },
                    }
                }
            };
            var levelData = levelCreator.SpawnLevel(inputData);
            _levelDataManager = new LevelDataManager(levelData);
            foreach (var shelve in levelData.Shelves)
            {
                for (var slotId = 0; slotId < shelve.DropZones.Length; slotId++)
                {
                    var sId = slotId;
                    dragDropGameManager.RegisterDropZone(new DropZoneData
                    {
                        Zone = shelve.DropZones[slotId],
                        OnDropped = itemId => OnDropped(itemId, shelve.Id, sId)
                    });
                }
            }

            foreach (var drag in levelData.Drags)
            {
                dragDropGameManager.RegisterDragObject(drag);
            }
        }

        private void OnDropped(int itemId, int shelfId, int slotId)
        {
            CleanLogger.Log($"Dropped {itemId} at shelf {shelfId} slot {slotId}");
            var layerId = _levelDataManager.GetTopLayerOfShelf(shelfId);
            var slotData = _levelDataManager.GetItem(shelfId, layerId, slotId);

            if (slotData == null)
            {
                var item = (ShelfItemBasic)_levelDataManager.FindItem(itemId);
                var shelf = (CommonShelfNormal)_levelDataManager.GetShelf(shelfId);
                item.transform.parent = shelf.transform;

                _levelDataManager.SetItem(item.Meta.ShelfId, item.Meta.LayerId, item.Meta.SlotId, null);
                item.SetShelf(item.Meta.Change(shelfId, layerId, slotId), shelf.SpacingData, ShelfLayerDisplay.Top);
                item.ResetVisual();
                _levelDataManager.SetItem(shelfId, layerId, slotId, item);
                TryMergeLayer(shelfId, layerId);
            }
            else
            {
                if (slotData.Meta.Id != itemId)
                {
                    CleanLogger.Error("Slot đã có item, không thể place thêm vào");
                }
            }
        }

        private void TryMergeLayer(int shelfId, int layerId)
        {
            var items = _levelDataManager.GetLayer(shelfId, layerId);
            var first = items?[0];
            if (first == null) return;
            var allTheSame = items.All(e => e != null && e.Meta.TypeId == first.Meta.TypeId);
            if (!allTheSame) return;

            // Remove top layer
            foreach (var item in items)
            {
                _levelDataManager.SetItem(shelfId, layerId, item.Meta.SlotId, null);
                dragDropGameManager.UnregisterDragObject(item.DragObject);
                item.DestroyItem();
            }

            StartCoroutine(ShowNextLayer(shelfId, layerId));
        }
        
        private IEnumerator ShowNextLayer(int shelfId, int layerId)
        {
            yield return new WaitForSeconds(1.0f);
            
            // đưa layer dưới lên top
            var nextTopLayer = _levelDataManager.GetLayer(shelfId, layerId + 1);
            if (nextTopLayer == null) yield break;
            foreach (var item in nextTopLayer)
            {
                item?.SetDisplay(ShelfLayerDisplay.Top);
                item?.ResetVisual();
            }

            // đưa layer dưới lên seconds
            var nextSecondLayer = _levelDataManager.GetLayer(shelfId, layerId + 2);
            if (nextSecondLayer == null) yield break;
            foreach (var item in nextTopLayer)
            {
                item?.SetDisplay(ShelfLayerDisplay.Second);
                item?.ResetVisual();
            }
        }
    }
}