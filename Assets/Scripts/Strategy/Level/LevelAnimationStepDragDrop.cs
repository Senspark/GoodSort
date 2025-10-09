using Core;
using Game;
using manager;

namespace Strategy.Level
{
    public class LevelAnimationStepDragDrop : ILevelAnimationStep
    {
        private readonly LevelAnimationSwitchStateControl _control;
        private readonly LevelDataManager _levelDataManager;
        private readonly IDragDropGameManager _dragDropManager;

        public LevelAnimationStepDragDrop(
            LevelAnimationSwitchStateControl control,
            LevelDataManager levelDataManager,
            IDragDropGameManager dragDropManager
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
                    _dragDropManager.RegisterDropZone(new DropZoneData
                    {
                        Zone = shelve.DropZones[slotId],
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
            CleanLogger.Log($"Dropped {itemId} at shelf {shelfId} slot {slotId}");
            var layerId = _levelDataManager.GetTopLayerOfShelf(shelfId);
            var slotData = _levelDataManager.GetItem(shelfId, layerId, slotId);

            if (slotData == null)
            {
                var item = (ShelfItemBasic)_levelDataManager.FindItem(itemId);
                var shelf = (CommonShelfNormal)_levelDataManager.GetShelf(shelfId);
                item.transform.parent = shelf.transform;

                _levelDataManager.SetSlotData(item.Meta.ShelfId, item.Meta.LayerId, item.Meta.SlotId, null);
                item.SetShelf(item.Meta.Change(shelfId, layerId, slotId), shelf.SpacingData, ShelfLayerDisplay.Top);
                item.ResetVisual();
                _levelDataManager.SetSlotData(shelfId, layerId, slotId, item);
                _control.ToMergeLayer(shelfId, layerId);
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