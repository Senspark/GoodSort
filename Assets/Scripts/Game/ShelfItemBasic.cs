using Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class ShelfItemBasic : MonoBehaviour, IShelfItem
    {
        [Required, SerializeField] private SpriteRenderer spriteRenderer;
        [Required, SerializeField] public DragObject dragObject;

        public ShelfItemMeta Meta { get; private set; }

        private const float MaxZ = -1.0f;
        private ISpacingData _spacingData;

        public void Init(
            ShelfItemMeta meta,
            ISpacingData spacingData,
            Sprite sprite
        )
        {
            Meta = meta;
            spriteRenderer.sprite = sprite;
            _spacingData = spacingData;
            name = $"S{meta.ShelfId}-L{meta.LayerId}-T{meta.TypeId}-{meta.Id}";
            dragObject.Init(meta.Id);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetShelf(
            ShelfItemMeta meta,
            ISpacingData spacingData
        )
        {
            Meta = meta;
            _spacingData = spacingData;
        }

        public void ResetPosition()
        {
            var offset = (Vector3)_spacingData.GetPosition(Meta.LayerId, Meta.SlotId);
            offset.z = MaxZ + Meta.LayerId * 0.01f; // z càng nhỏ thì càng hiển thị trên cùng
            if (Meta.SlotId == 1)
            {
                offset.z -= 0.001f; // cho item ở giữa hiển thị nổi bật hơn 
            }

            transform.localPosition = offset;
        }
    }
}