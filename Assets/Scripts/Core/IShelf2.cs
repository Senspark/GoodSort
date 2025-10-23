using System;
using Engine.ShelfPuzzle;
using JetBrains.Annotations;
using UnityEngine;

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
        void OnTopLayerCleared([CanBeNull] Action<Vector2> onCleared);
        
    }
}