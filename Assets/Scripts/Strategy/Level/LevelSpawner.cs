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

        private const int MaxItemForShelfCommon = 3;
        private const int NullItemTypeId = 0;

        public LevelData SpawnLevel(ShelfPuzzleInputData[] input)
        {
            var shelves = _container.GetComponentsInChildren<CommonShelfNormal>();
            if (shelves.Length != input.Length)
            {
                CleanLogger.Error($"Số lượng Shelves không khớp {shelves.Length} != ${input.Length}");
                return null;
            }

            var shelvesItems = new IShelfItem[input.Length][][];
            var drags = new List<IDragObject>();
            var itemId = 0;
            Func<int> getItemId = () => itemId++;

            for (var shelfId = 0; shelfId < input.Length; shelfId++)
            {
                var shelfData = input[shelfId];
                var shelf = shelves[shelfId];
                shelf.Init(shelfId);
                var (items, itemsDrags) = SpawnShelf(shelfId, shelf, shelfData, getItemId);
                shelvesItems[shelfId] = items;
                drags.AddRange(itemsDrags);
            }

            return new LevelData
            {
                ShelveItems = shelvesItems,
                Shelves = shelves.Select(e => (IShelf2)e).ToArray(),
                Drags = drags,
            };
        }

        private (IShelfItem[][], List<DragObject>) SpawnShelf(
            int shelfId,
            CommonShelfNormal shelf,
            ShelfPuzzleInputData input,
            Func<int> getItemIdFunc
        )
        {
            var data = new IShelfItem[input.Data.Length][];
            var drags = new List<DragObject>();
            var spawnContainer = shelf.transform;
            var spacingData = shelf.SpacingData;

            for (var layerId = 0; layerId < input.Data.Length; layerId++)
            {
                data[layerId] = new IShelfItem[MaxItemForShelfCommon];
                for (var slotId = 0; slotId < MaxItemForShelfCommon; slotId++)
                {
                    var itemType = input.Data[layerId][slotId];
                    ShelfItemBasic newItem = null;

                    if (itemType > NullItemTypeId)
                    {
                        var meta = new ShelfItemMeta(shelfId, layerId, slotId, itemType, getItemIdFunc());
                        newItem = SpawnItem(meta, spawnContainer, spacingData);
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
            ISpacingData spacingData
        )
        {
            var spr = Resources.Load<Sprite>($"sprite/Items/Item{meta.TypeId}");
            if (!spr)
            {
                CleanLogger.Error($"Sprite không có: {meta.TypeId}");
                return null;
            }

            var obj = Object.Instantiate(_prefab, parent);
            obj.Init(meta, spacingData, FromLayerId(meta.LayerId), spr);
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
        public IShelfItem[][][] ShelveItems;
        public IShelf2[] Shelves;
        public List<IDragObject> Drags;
    }
}