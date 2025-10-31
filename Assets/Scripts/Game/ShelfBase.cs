using Core;
using Engine.ShelfPuzzle;
using UnityEngine;
using System;

namespace Game
{
    public abstract class ShelfBase : MonoBehaviour, IShelf2
    {
        public abstract int Id { get; protected set; }
        public abstract int LockCount { get; protected set; }
        public abstract ShelfType Type { get; }
        public abstract ISpacingData SpacingData { get; }
        public abstract IDropZone[] DropZones { get; protected set; }
        public abstract void Init(int shelfId, int lockCount);
        public abstract void DecreaseLockCount();
        public abstract void OnTopLayerCleared(Action<Vector2> onCleared);
    }
}