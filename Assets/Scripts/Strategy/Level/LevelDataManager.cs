using System.Collections.Generic;
using System.Linq;
using Core;

namespace Strategy.Level
{
    public class LevelDataManager
    {
        /**
         * [shelf] [layer] [slot]
         */
        private readonly IShelfItem[][][] _shelves;

        private readonly LevelData _levelData;
        private readonly Dictionary<int, IShelfItem> _items;

        public LevelDataManager(LevelData levelData)
        {
            _levelData = levelData;
            _shelves = levelData.ShelveItems;
            _items = new Dictionary<int, IShelfItem>();
            foreach (var layers in _shelves)
            {
                foreach (var layer in layers)
                {
                    foreach (var shelf in layer)
                    {
                        if (shelf != null)
                        {
                            _items[shelf.Meta.Id] = shelf;
                        }
                    }
                }
            }
        }

        public IShelfItem GetItem(int shelfId, int layerId, int slotId)
        {
            if (_shelves == null) return null;

            if (shelfId < 0 || shelfId >= _shelves.Length) return null;
            var shelf = _shelves[shelfId];

            if (layerId < 0 || layerId >= shelf.Length) return null;
            var layer = shelf[layerId];

            if (slotId < 0 || slotId >= layer.Length) return null;
            return layer[slotId];
        }

        public IShelfItem FindItemInShelf(int shelfId, int itemTypeId)
        {
            if (_shelves == null) return null;
            if (shelfId < 0 || shelfId >= _shelves.Length) return null;
            var shelf = _shelves[shelfId];
            var layerId = FindNotEmptyLayer(shelf);
            if (layerId < 0)
            {
                return null;
            }

            var layer = shelf[layerId];
            return FindItemTypeInLayer(layer, itemTypeId);
        }

        public IShelfItem FindItem(int itemId)
        {
            return _items[itemId];
        }

        public IShelf2 GetShelf(int shelfId)
        {
            return _levelData.Shelves.FirstOrDefault(e => e.Id == shelfId);
        }

        public int GetTopLayerOfShelf(int shelfId)
        {
            if (shelfId < 0 || shelfId >= _shelves.Length) return -1;
            var shelf = _shelves[shelfId];
            return FindNotEmptyLayer(shelf);
        }

        public IShelfItem[] GetTopLayer(int shelfId)
        {
            if (shelfId < 0 || shelfId >= _shelves.Length) return null;
            var shelf = _shelves[shelfId];
            foreach (var layer in shelf)
            {
                foreach (var slot in layer)
                {
                    if (slot != null)
                    {
                        return layer;
                    }
                }
            }

            return null;
        }

        public IShelfItem[] GetLayer(int shelfId, int layerId)
        {
            if (shelfId < 0 || shelfId >= _shelves.Length) return null;
            var shelf = _shelves[shelfId];
            if (layerId < 0 || layerId >= shelf.Length) return null;
            return shelf[layerId];
        }

        public void SetItem(int shelfId, int layerId, int slotId, IShelfItem item)
        {
            if (_shelves == null) return;

            if (shelfId < 0 || shelfId >= _shelves.Length) return;
            var shelf = _shelves[shelfId];

            if (layerId < 0 || layerId >= shelf.Length) return;
            var layer = shelf[layerId];

            if (slotId < 0 || slotId >= layer.Length) return;
            layer[slotId] = item;
        }

        private static int FindNotEmptyLayer(IShelfItem[][] shelf)
        {
            for (var layerId = 0; layerId < shelf.Length; layerId++)
            {
                var layer = shelf[layerId];
                if (layer.Any(t => t != null))
                {
                    return layerId;
                }
            }

            return -1;
        }

        private static IShelfItem FindItemTypeInLayer(IShelfItem[] layer, int itemTypeId)
        {
            return layer.FirstOrDefault(slot => slot != null && slot.Meta.TypeId == itemTypeId);
        }
    }
}