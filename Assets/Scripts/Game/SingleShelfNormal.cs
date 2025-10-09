using System;
using Core;
using Engine.ShelfPuzzle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    /**
     * Single Shelf chỉ có lấy item ra, không có đặt item vào
     */
    public class SingleShelfNormal : ShelfBase
    {
        [Required, SerializeField] private SpriteRenderer sprRenderer;
        [Required, SerializeField] private SingleShelfNormalSpacingData spacingData;

        public override int Id { get; protected set; }
        public override ShelfType Type => ShelfType.TakeOnly;
        public override ISpacingData SpacingData => spacingData;
        public override IDropZone[] DropZones { get; protected set; }

        private void Awake()
        {
            DropZones = Array.Empty<IDropZone>();
        }
        
        public override void Init(int shelfId)
        {
            Id = shelfId;
        }
    }
}