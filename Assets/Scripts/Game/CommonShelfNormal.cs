using System.Linq;
using Core;
using manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class CommonShelfNormal : MonoBehaviour, IShelf2
    {
        [Required, SerializeField] public DropZone[] dropZone;
        [Required, SerializeField] private SpriteRenderer sprRenderer;
        [Required, SerializeField] private CommonShelfNormalSpacingData spacingData;

        public int Id { get; private set; }
        public ISpacingData SpacingData => spacingData;
        public IDropZone[] DropZones { get; private set; }

        private void Awake()
        {
            if (dropZone.Length != 3)
            {
                CleanLogger.Error("Phải có đúng 3 Drop zone");
                return;
            }

            DropZones = dropZone.Select(e => (IDropZone)e).ToArray();
        }

        public void Init(int shelfId)
        {
            Id = shelfId;
            name = $"{name}-{shelfId}";
        }
    }
}