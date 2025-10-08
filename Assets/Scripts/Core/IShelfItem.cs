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
        void DestroyItem();
    }

    public enum ShelfLayerDisplay
    {
        Top, Second, Hidden
    }
}