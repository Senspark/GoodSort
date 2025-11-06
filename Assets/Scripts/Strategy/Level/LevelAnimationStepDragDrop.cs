using Core;
using Defines;
using Game;
using manager;
using Senspark;

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
                // ServiceLocator.Instance.Resolve<IAudioManager>().PlaySound(AudioEnum.PutGoods);
                var item = (ShelfItemBasic)_levelDataManager.FindItem(itemId);
                var shelf = (ShelfBase)_levelDataManager.GetShelf(shelfId);
                item.transform.parent = shelf.transform;

                var prevShelfId = item.Meta.ShelfId;
                var prevLayerId = item.Meta.LayerId;
                var prevSlotId = item.Meta.SlotId;
                _levelDataManager.SetSlotData(prevShelfId, prevLayerId, prevSlotId, null);
                item.SetShelf(item.Meta.Change(shelfId, layerId, slotId), shelf.SpacingData, ShelfLayerDisplay.Top);
                item.ResetVisual();
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