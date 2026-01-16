using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public interface IShelfItem
    {
        ShelfItemMeta Meta { get; }
        IDragObject DragObject { get; }

        void SetPosition(Vector3 position);
        void SetShelf(ShelfItemMeta meta, ISpacingData spacingData, ShelfLayerDisplay display);
        void SetDisplay(ShelfLayerDisplay display);
        void ResetVisual();

        void FadeInVisual(float duration);
        void DestroyItem();
        public void Bounce([CanBeNull] Action onCompleted, float delay = 0f);

        public void Jiggly();
    }

    public enum ShelfLayerDisplay
    {
        Top, Second, Hidden
    }
}