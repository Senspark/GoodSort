using Core;
using Engine.ShelfPuzzle;
using manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class CommonShelfNormal : ShelfBase
    {
        [Required, SerializeField] public DropZone2[] dropZone;
        [Required, SerializeField] private SpriteRenderer sprRenderer;
        [Required, SerializeField] private GameObject lockView;
        [Required, SerializeField] private CommonShelfNormalSpacingData spacingData;

        public override int Id { get; protected set; }
        public override int LockCount { get; protected set; }
        public override ShelfType Type => ShelfType.Common;
        public override ISpacingData SpacingData => spacingData;
        public override IDropZone[] DropZones { get; protected set; }

        private void Awake()
        {
            if (dropZone.Length != 3)
            {
                CleanLogger.Error("Phải có đúng 3 Drop zone");
            }
        }

        public override void Init(int shelfId, int lockCount)
        {
            Id = shelfId;
            LockCount = lockCount;
            name = $"{name}-{shelfId}";
            
            DropZones = new IDropZone[dropZone.Length];
            for (var slotId = 0; slotId < dropZone.Length; slotId++)
            {
                var zone = dropZone[slotId];
                zone.Init(Id, slotId);
                DropZones[slotId] = zone;
            }

            lockView.gameObject.SetActive(LockCount > 0);
        }
        
        public override void DecreaseLockCount()
        {
            LockCount = Mathf.Max(0, LockCount - 1);
            lockView.gameObject.SetActive(LockCount > 0);
        }
    }
}