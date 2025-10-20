using System.Collections.Generic;
using Core;
using Engine.ShelfPuzzle;

namespace Strategy.Level
{
    public interface ILevelDataManager
    {
        void Dispose();
        ShelfPuzzleInputData[] Export();
        void RemoveItem(int itemId);
        List<IShelfItem> GetItems();
        IShelfItem GetItem(int shelfId, int layerId, int slotId);
        IShelfItem FindItemInShelf(int shelfId, int itemTypeId);
        IShelfItem FindItem(int itemId);
        IShelf2 GetShelf(int shelfId);
        IShelf2[] GetShelves();
        List<IDragObject> GetDrags();
        int GetTopLayerOfShelf(int shelfId);
        IShelfItem[] GetTopLayer(int shelfId);
        IShelfItem[] GetLayer(int shelfId, int layerId);
        bool IsLayerEmpty(int shelfId, int layerId);
        void SetSlotData(int shelfId, int layerId, int slotId, IShelfItem item);
    }
}