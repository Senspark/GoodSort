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
            var foundShelves = _container.GetComponentsInChildren<IShelf2>();
            var commonShelves = foundShelves.Where(e => e.Type == ShelfType.Common).ToArray();
            var singleShelves = foundShelves.Where(e => e.Type == ShelfType.Single).ToArray();

            var uiCommonAmount = commonShelves.Length;
            var uiSingleAmount = singleShelves.Length;

            var ipCommonAmount = Array.FindAll(input, e => e.Type == ShelfType.Common).Length;
            var ipSingleAmount = Array.FindAll(input, e => e.Type == ShelfType.Single).Length;

            if (uiCommonAmount < ipCommonAmount)
            {
                CleanLogger.Error($"Số lượng Common Shelves không khớp {uiCommonAmount} < {ipCommonAmount}");
                return null;
            }

            if (uiSingleAmount < ipSingleAmount)
            {
                CleanLogger.Error($"Số lượng Single Shelves không khớp {uiSingleAmount} < {ipSingleAmount}");
                return null;
            }

            var totalShelvesAmount = input.Length;
            var outputShelves = new IShelf2[totalShelvesAmount];
            var shelvesItems = new IShelfItem[totalShelvesAmount][][];
            var itemId = 0;
            Func<int> getItemId = () => itemId++;

            int commonShelfId = 0, singleShelfId = 0;
            for (var shelfId = 0; shelfId < totalShelvesAmount; shelfId++)
            {
                var shelfData = input[shelfId];
                IShelf2 shelf;
                switch (shelfData.Type)
                {
                    case ShelfType.Common:
                        shelf = commonShelves[commonShelfId++];
                        break;
                    case ShelfType.Single:
                        shelf = singleShelves[singleShelfId++];
                        break;
                    default:
                        CleanLogger.Error($"Chưa xử lý case này: ${shelfData.Type}");
                        return null;
                }

                shelf.Init(shelfId, shelfData.LockCount);
                shelvesItems[shelfId] = SpawnShelf(shelfId, shelf, shelfData, getItemId, onItemDestroy);
                outputShelves[shelfId] = shelf;
            }

            return new LevelData(shelvesItems, outputShelves);
        }

        private IShelfItem[][] SpawnShelf(
            int shelfId,
            IShelf2 shelf,
            ShelfPuzzleInputData input,
            Func<int> getItemIdFunc,
            Action<ShelfItemMeta> onItemDestroy
        )
        {
            var data = new IShelfItem[input.Data.Length][];
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
                        newItem = SpawnItem(meta, spawnContainer, spacingData, onItemDestroy, shelf);
                    }

                    data[layerId][slotId] = newItem;
                }
            }

            return data;
        }

        private ShelfItemBasic SpawnItem(
            ShelfItemMeta meta,
            Transform parent,
            ISpacingData spacingData,
            Action<ShelfItemMeta> onItemDestroy,
            IShelf2 shelf
        )
        {
            var spr = Resources.Load<Sprite>($"sprite/Items/Item{meta.TypeId}");
            if (!spr)
            {
                CleanLogger.Error($"Sprite không có: {meta.TypeId}");
                return null;
            }

            var obj = Object.Instantiate(_prefab, parent);
            obj.Init(meta, spacingData, FromLayerId(meta.LayerId), onItemDestroy, spr, shelf);
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
    }

    public class LevelData
    {
        public readonly IShelfItem[][][] ShelveItems;
        public readonly IShelf2[] Shelves;

        public LevelData(
            IShelfItem[][][] shelveItems,
            IShelf2[] shelves
        )
        {
            ShelveItems = shelveItems;
            Shelves = shelves;
        }
    }
}