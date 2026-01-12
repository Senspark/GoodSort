using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Game;
using Sirenix.Utilities;
using UnityEngine;

namespace Strategy.Level
{
    public class LevelAnimationStepMergeLayer : ILevelAnimationStep
    {
        private readonly LevelAnimationSwitchStateControl _control;
        private readonly LevelDataManager _levelDataManager;
        private readonly InputData _inputData;
        private readonly List<Data> _processingData = new();
        // private readonly Action<Vector2> _onMergeCompleted;

        public LevelAnimationStepMergeLayer(
            LevelAnimationSwitchStateControl control,
            InputData inputData,
            LevelDataManager levelDataManager,
            IDragDropManager dragDropManager
            // Action<Vector2> onMergeCompleted
        )
        {
            _control = control;
            _inputData = inputData;
            _levelDataManager = levelDataManager;
            // _onMergeCompleted = onMergeCompleted;
        }

        public void Enter()
        {
            var canBeMerged1 = TryMergeLayer(_inputData.ShelfId, _inputData.LayerId);
            var canBeMerged2 = TryMovePreviousLayer(_inputData.PreviousShelfId, _inputData.PreviousLayerId);
            if (!canBeMerged1 && !canBeMerged2)
            {
                _control.ToDragDrop();
            }
        }

        public void Update(float dt)
        {
            if (_processingData.Count == 0) return;

            var done = 0;
            foreach (var pData in _processingData)
            {
                // Step 1: Chờ các items Bounce xong
                if (pData.AnimationDone < pData.Items.Length) return;

                // Step 2: Chờ các items bị destroy
                if (!pData.ItemDestroyed)
                {
                    pData.Items.ForEach(e => e.DestroyItem());
                    pData.ItemDestroyed = true;
                }
                else
                {
                    // Step 2: Hiển thị các layer bên dưới
                    ShowNextLayer(pData.ShelfId, pData.LayerId);
                    done++;
                }
            }

            // Callback Done
            if (done >= _processingData.Count)
            {
                _control.ToUnlockShelves(new LevelAnimationUnlockShelves.InputData(1));
            }
        }

        public void Exit()
        {
        }

        private bool TryMergeLayer(int shelfId, int layerId)
        {
            var items = _levelDataManager.GetLayer(shelfId, layerId);
            var first = items?[0];
            if (first == null) return false;
            var allTheSame = items.All(e => e != null && e.Meta.TypeId == first.Meta.TypeId);
            if (!allTheSame) return false;

            var processingData = new Data(shelfId, layerId, items.ToArray());

            // Remove top layer
            Action onAnimationCompleted = () => { processingData.AnimationDone++; };

            foreach (var item in items)
            {
                item.Bounce(onAnimationCompleted);
            }

            // var shelfBase = (ShelfBase)_levelDataManager.GetShelf(shelfId);
            // _onMergeCompleted?.Invoke(shelfBase.transform.position);

            _processingData.Add(processingData);

            return true;
        }

        private void ShowNextLayer(int shelfId, int layerId)
        {
            // đưa layer dưới lên top
            var nextTopLayer = _levelDataManager.GetLayer(shelfId, layerId + 1);
            if (nextTopLayer == null) return;
            foreach (var item in nextTopLayer)
            {
                item?.SetDisplay(ShelfLayerDisplay.Top);
                item?.FadeInVisual(0.2f);
                // item?.ResetVisual();
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

        /* Kiểm tra thử Shelve trước đó: nếu top-layer hết phần tử rồi thì dời second-layer lên */
        private bool TryMovePreviousLayer(int shelfId, int layerId)
        {
            if (!_levelDataManager.IsLayerEmpty(shelfId, layerId))
            {
                return false;
            }

            var processingData = new Data(shelfId, layerId, Array.Empty<IShelfItem>())
            {
                ItemDestroyed = true,
            };
            _processingData.Add(processingData);
            return true;
        }

        public class InputData
        {
            public readonly int PreviousShelfId;
            public readonly int PreviousLayerId;

            public readonly int ShelfId;
            public readonly int LayerId;

            public InputData(int previousShelfId, int previousLayerId, int shelfId, int layerId)
            {
                PreviousShelfId = previousShelfId;
                PreviousLayerId = previousLayerId;

                ShelfId = shelfId;
                LayerId = layerId;
            }
        }

        private class Data
        {
            public readonly int ShelfId;
            public readonly int LayerId;
            public readonly IShelfItem[] Items;

            public int AnimationDone;
            public bool ItemDestroyed;

            public Data(int shelfId, int layerId, IShelfItem[] items)
            {
                ShelfId = shelfId;
                LayerId = layerId;
                Items = items;
            }
        }
    }
}