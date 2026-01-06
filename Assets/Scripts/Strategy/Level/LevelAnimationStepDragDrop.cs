using Core;
using Defines;
using Game;
using manager;
using Senspark;
using UnityEngine;

namespace Strategy.Level
{
    public class LevelAnimationStepDragDrop : ILevelAnimationStep
    {
        private readonly LevelAnimationSwitchStateControl _control;
        private readonly LevelDataManager _levelDataManager;
        private readonly IDragDropManager _dragDropManager;

        public LevelAnimationStepDragDrop(
            LevelAnimationSwitchStateControl control,
            LevelDataManager levelDataManager,
            IDragDropManager dragDropManager
        )
        {
            _control = control;
            _levelDataManager = levelDataManager;
            _dragDropManager = dragDropManager;

            foreach (var shelve in _levelDataManager.GetShelves())
            {
                for (var slotId = 0; slotId < shelve.DropZones.Length; slotId++)
                {
                    var sId = slotId;
                    var dropZone = shelve.DropZones[slotId];

                    // Set dependencies cho DropZone2 để hỗ trợ thuật toán detect mới
                    if (dropZone is DropZone2 dropZone2)
                    {
                        dropZone2.SetDependencies(shelve, _levelDataManager);
                    }

                    _dragDropManager.RegisterDropZone(new DropZoneData
                    {
                        Zone = dropZone,
                        OnDropped = itemId => OnDropped(itemId, shelve.Id, sId)
                    });
                }
            }

            foreach (var drag in _levelDataManager.GetDrags())
            {
                _dragDropManager.RegisterDragObject(drag);
            }
        }

        public void Enter()
        {
            _dragDropManager.Unpause();
        }

        public void Update(float dt)
        {
        }

        public void Exit()
        {
            _dragDropManager.Pause();
        }

        private void OnDropped(int itemId, int shelfId, int slotId)
        {
            // CleanLogger.Log($"Dropped {itemId} at shelf {shelfId} slot {slotId}");
            var layerId = _levelDataManager.GetTopLayerOfShelf(shelfId);
            var slotData = _levelDataManager.GetItem(shelfId, layerId, slotId);

            if (slotData == null)
            {
                ServiceLocator.Instance.Resolve<IAudioManager>().PlaySound(AudioEnum.PutGoods);
                var item = (ShelfItemBasic)_levelDataManager.FindItem(itemId);
                var shelf = (ShelfBase)_levelDataManager.GetShelf(shelfId);

                // Debug.Log($"[DROP] === START DROP ===");
                // Debug.Log($"[DROP] Item: {item.name}, ItemID: {itemId}");
                // Debug.Log($"[DROP] Target Shelf: {shelfId}, Layer: {layerId}, Slot: {slotId}");
                // Debug.Log($"[DROP] BEFORE SetParent - Item WorldPos: {item.transform.position}, LocalPos: {item.transform.localPosition}");
                // Debug.Log($"[DROP] BEFORE SetParent - Item Parent: {(item.transform.parent ? item.transform.parent.name : "NULL")}");
                // Debug.Log($"[DROP] BEFORE SetParent - Shelf WorldPos: {shelf.transform.position}");

                item.gameObject.SetActive(false);
                // Debug.Log($"[DROP] SetActive(false) - Item hidden");

                item.transform.SetParent(shelf.transform, false);
                // Debug.Log($"[DROP] AFTER SetParent - Item WorldPos: {item.transform.position}, LocalPos: {item.transform.localPosition}");
                // Debug.Log($"[DROP] AFTER SetParent - Item Parent: {item.transform.parent.name}");

                var prevShelfId = item.Meta.ShelfId;
                var prevLayerId = item.Meta.LayerId;
                var prevSlotId = item.Meta.SlotId;
                _levelDataManager.SetSlotData(prevShelfId, prevLayerId, prevSlotId, null);
                item.SetShelf(item.Meta.Change(shelfId, layerId, slotId), shelf.SpacingData, ShelfLayerDisplay.Top);

                // Debug.Log($"[DROP] BEFORE ResetVisual - Item WorldPos: {item.transform.position}, LocalPos: {item.transform.localPosition}");
                item.ResetVisual();
                // Debug.Log($"[DROP] AFTER ResetVisual - Item WorldPos: {item.transform.position}, LocalPos: {item.transform.localPosition}");
                // Debug.Log($"[DROP] AFTER ResetVisual - Item Active: {item.gameObject.activeSelf}");
                // Debug.Log($"[DROP] === END DROP ===\n");

                _levelDataManager.SetSlotData(shelfId, layerId, slotId, item);

                _control.ToMergeLayer(
                    new LevelAnimationStepMergeLayer.InputData(prevShelfId, prevLayerId, shelfId, layerId));
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