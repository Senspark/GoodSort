using Engine.ShelfPuzzle;
using Game;
using UnityEngine;

namespace Core
{
    public interface IShelf
    {
        /**
         * Shelf ID
         */
        int Id { get; }

        /**
         * [
         *      [slot_0, slot_1, slot_2], // layer_0
         *      [slot_0, slot_1, slot_2], // layer_1
         * ]
         */
        int[][] ExportData();

        void ImportData(int id, ShelfPuzzleInputData input);
    }

    public interface IShelf2
    {
        /* Shelf ID */
        int Id { get; }
        ISpacingData SpacingData { get; }
    }

    public interface IShelfItem
    {
        ShelfItemMeta Meta { get; }

        void SetPosition(Vector3 position);
        void SetShelf(ShelfItemMeta meta, ISpacingData spacingData);
        void ResetPosition();
    }

    public interface IDragObject
    {
        /**
         * Unique Id của Item trong Game Scene
         */
        int Id { get; }
    }
}