using System;
using System.Linq;
using Core;
using Sirenix.Utilities;

namespace Strategy.Level
{
    public class LevelAnimationStepMergeLayer : ILevelAnimationStep
    {
        private readonly LevelAnimationSwitchStateControl _control;
        private readonly LevelDataManager _levelDataManager;
        private readonly IDragDropManager _dragDropManager;
        private readonly InputData _inputData;
        private Data _processingData;

        public LevelAnimationStepMergeLayer(
            LevelAnimationSwitchStateControl control,
            InputData inputData,
            LevelDataManager levelDataManager,
            IDragDropManager dragDropManager
        )
        {
            _control = control;
            _inputData = inputData;
            _levelDataManager = levelDataManager;
            _dragDropManager = dragDropManager;
        }
        
        public void Enter()
        {
            TryMergeLayer(_inputData.ShelfId, _inputData.LayerId);
        }

        public void Update(float dt)
        {
            // Step 1: Chờ các items Bounce xong
            if (_processingData == null) return;
            if (_processingData.AnimationDone < _processingData.Items.Length) return;

            // Step 2: Chờ các items bị destroy
            if (!_processingData.ItemDestroyed)
            {
                _processingData.Items.ForEach(e => e.DestroyItem());
                _processingData.ItemDestroyed = true;
                return;
            }

            // Step 2: Hiển thị các layer bên dưới & Callback Done
            ShowNextLayer(_inputData.ShelfId, _inputData.LayerId);
            _control.ToDragDrop();
        }

        public void Exit()
        {
        }

        private void TryMergeLayer(int shelfId, int layerId)
        {
            var items = _levelDataManager.GetLayer(shelfId, layerId);
            var first = items?[0];
            if (first == null)
            {
                _control.ToDragDrop();
                return;
            }
            var allTheSame = items.All(e => e != null && e.Meta.TypeId == first.Meta.TypeId);
            if (!allTheSame)
            {
                _control.ToDragDrop();
                return;
            }

            _processingData = new Data
            {
                Items = items.ToArray(),
                AnimationDone = 0,
            };

            // Remove top layer
            Action onAnimationCompleted = () =>
            {
                _processingData.AnimationDone++;
            };

            foreach (var item in items)
            {
                item.Bounce(onAnimationCompleted);
            }
        }

        private void ShowNextLayer(int shelfId, int layerId)
        {
            // đưa layer dưới lên top
            var nextTopLayer = _levelDataManager.GetLayer(shelfId, layerId + 1);
            if (nextTopLayer == null) return;
            foreach (var item in nextTopLayer)
            {
                item?.SetDisplay(ShelfLayerDisplay.Top);
                item?.ResetVisual();
            }

            // đưa layer dưới lên seconds
            var nextSecondLayer = _levelDataManager.GetLayer(shelfId, layerId + 2);
            if (nextSecondLayer == null) return;
            foreach (var item in nextSecondLayer)
            {
                item?.SetDisplay(ShelfLayerDisplay.Second);
                item?.ResetVisual();
            }
        }

        public class InputData
        {
            public readonly int ShelfId;
            public readonly int LayerId;

            public InputData(int shelfId, int layerId)
            {
                ShelfId = shelfId;
                LayerId = layerId;
            }
        }

        private class Data
        {
            public IShelfItem[] Items;
            public int AnimationDone;
            public bool ItemDestroyed;
        }
    }
}