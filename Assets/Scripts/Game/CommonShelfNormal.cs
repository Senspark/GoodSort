using System;
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

        private void Awake()
        {
            if (dropZone.Length != 3)
            {
                CleanLogger.Error("Phải có đúng 3 Drop zone");
                return;
            }
        }

        public void Init(int shelfId)
        {
            Id = shelfId;
            name = $"{name}-{shelfId}";
        }
    }
}