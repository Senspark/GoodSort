using Engine.ShelfPuzzle;

namespace Core
{
    public interface IShelf2
    {
        /* Shelf ID */
        int Id { get; }
        int LockCount { get; }
        ShelfType Type { get; }
        ISpacingData SpacingData { get; }
        IDropZone[]  DropZones { get; }
        void Init(int shelfId, int lockCount);
        void DecreaseLockCount();
    }
}