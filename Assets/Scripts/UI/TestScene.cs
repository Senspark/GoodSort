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
                item.SetShelf(item.Meta.Change(shelfId, layerId, slotId), shelf.SpacingData);
                item.ResetPosition();
                _levelDataManager.SetItem(shelfId, layerId, slotId, item);
            }
            else
            {
                if (slotData.Meta.Id != itemId)
                {
                    CleanLogger.Error("Slot đã có item, không thể place thêm vào");
                }
            }
        }
    }
    
}