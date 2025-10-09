using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Engine.ShelfPuzzle;
using Game;
using manager;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Strategy.Level
{
    public class LevelCreator
    {
        private readonly GameObject _container;
        private readonly ShelfItemBasic _prefab;

        public LevelCreator(
            GameObject container,
            ShelfItemBasic prefab
        )
        {
            _container = container;
            _prefab = prefab;
        }

        private const int NullItemTypeId = 0;

        public LevelData SpawnLevel(
            ShelfPuzzleInputData[] input,
            Action<ShelfItemMeta> onItemDestroy
        )
        {
            var shelves = _container.GetComponentsInChildren<IShelf2>();
            if (shelves.Length != input.Length)
            {
                CleanLogger.Error($"Số lượng Shelves không khớp {shelves.Length} != ${input.Length}");
                return null;
            }

            PreProcessData(ref shelves, ref input);

            var shelvesItems = new IShelfItem[input.Length][][];
            var drags = new List<IDragObject>();
            var itemId = 0;
            Func<int> getItemId = () => itemId++;

            for (var shelfId = 0; shelfId < input.Length; shelfId++)
            {
                var shelfData = input[shelfId];
                var shelf = shelves[shelfId];
                shelf.Init(shelfId);
                var (items, itemsDrags) = SpawnShelf(shelfId, shelf, shelfData, getItemId, onItemDestroy);
                shelvesItems[shelfId] = items;
                drags.AddRange(itemsDrags);
            }

            return new LevelData
            {
                ShelveItems = shelvesItems,
                Shelves = shelves,
                Drags = drags,
            };
        }

        private (IShelfItem[][], List<DragObject2>) SpawnShelf(
            int shelfId,
            IShelf2 shelf,
            ShelfPuzzleInputData input,
            Func<int> getItemIdFunc,
            Action<ShelfItemMeta> onItemDestroy
        )
        {
            var data = new IShelfItem[input.Data.Length][];
            var drags = new List<DragObject2>();
            var spawnContainer = ((ShelfBase)shelf).transform;
            var spacingData = shelf.SpacingData;

            for (var layerId = 0; layerId < input.Data.Length; layerId++)
            {
                var slotsAmount = input.Data[layerId].Length;
                data[layerId] = new IShelfItem[slotsAmount];
                for (var slotId = 0; slotId < slotsAmount; slotId++)
                {
                    var itemType = input.Data[layerId][slotId];
                    ShelfItemBasic newItem = null;

                    if (itemType > NullItemTypeId)
                    {
                        var meta = new ShelfItemMeta(shelfId, layerId, slotId, itemType, getItemIdFunc());
                        newItem = SpawnItem(meta, spawnContainer, spacingData, onItemDestroy);
                        if (newItem)
                        {
                            drags.Add(newItem.dragObject);
                        }
                    }

                    data[layerId][slotId] = newItem;
                }
            }

            return (data, drags);
        }

        private ShelfItemBasic SpawnItem(
            ShelfItemMeta meta,
            Transform parent,
            ISpacingData spacingData,
            Action<ShelfItemMeta> onItemDestroy
        )
        {
            var spr = Resources.Load<Sprite>($"sprite/Items/Item{meta.TypeId}");
            if (!spr)
            {
                CleanLogger.Error($"Sprite không có: {meta.TypeId}");
                return null;
            }

            var obj = Object.Instantiate(_prefab, parent);
            obj.Init(meta, spacingData, FromLayerId(meta.LayerId), onItemDestroy, spr);
            obj.ResetVisual();

            return obj;
        }

        private static ShelfLayerDisplay FromLayerId(int layerId)
        {
            return layerId switch
            {
                0 => ShelfLayerDisplay.Top,
                1 => ShelfLayerDisplay.Second,
                _ => ShelfLayerDisplay.Hidden
            };
        }

        /* Sắp xếp lại theo thứ tự:
         Common Shelve -> Single Shelve
         */
        private static void PreProcessData(ref IShelf2[] ui, ref ShelfPuzzleInputData[] input)
        {
            // Validate: Count shelf types in both arrays
            var uiCommonCount = ui.Count(shelf => shelf.Type == ShelfType.Common);
            var uiTakeOnlyCount = ui.Count(shelf => shelf.Type == ShelfType.TakeOnly);
            var inputCommonCount = input.Count(data => data.Type == ShelfType.Common);
            var inputTakeOnlyCount = input.Count(data => data.Type == ShelfType.TakeOnly);

            if (uiCommonCount != inputCommonCount)
            {
                CleanLogger.Error($"ShelfType.Common count mismatch: UI={uiCommonCount}, Input={inputCommonCount}");
                throw new InvalidOperationException(
                    $"ShelfType.Common count mismatch: UI={uiCommonCount}, Input={inputCommonCount}");
            }

            if (uiTakeOnlyCount != inputTakeOnlyCount)
            {
                CleanLogger.Error(
                    $"ShelfType.TakeOnly count mismatch: UI={uiTakeOnlyCount}, Input={inputTakeOnlyCount}");
                throw new InvalidOperationException(
                    $"ShelfType.TakeOnly count mismatch: UI={uiTakeOnlyCount}, Input={inputTakeOnlyCount}");
            }

            // Sort: Common first, TakeOnly second (stable sort preserves original order within each type)
            var sortedUi = ui.OrderBy(shelf => shelf.Type == ShelfType.Common ? 0 : 1).ToArray();
            var sortedInput = input.OrderBy(data => data.Type == ShelfType.Common ? 0 : 1).ToArray();

            // Copy sorted results back to original arrays
            Array.Copy(sortedUi, ui, ui.Length);
            Array.Copy(sortedInput, input, input.Length);
        }
    }

    public class LevelData
    {
        public IShelfItem[][][] ShelveItems;
        public IShelf2[] Shelves;
        public List<IDragObject> Drags;
    }
}