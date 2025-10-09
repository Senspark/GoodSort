using System;
using Core;
using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class ShelfItemBasic : MonoBehaviour, IShelfItem
    {
        [Required, SerializeField] private SpriteRenderer spriteRenderer;
        [Required, SerializeField] public DragObject dragObject;

        public ShelfItemMeta Meta { get; private set; }
        public IDragObject DragObject => dragObject;

        private const float MaxZ = -1.0f;
        private ISpacingData _spacingData;
        private ShelfLayerDisplay _display;
        private Action<ShelfItemMeta> _onDestroy;

        public void Init(
            ShelfItemMeta meta,
            ISpacingData spacingData,
            ShelfLayerDisplay display,
            Action<ShelfItemMeta> onDestroy,
            Sprite sprite
        )
        {
            Meta = meta;
            _spacingData = spacingData;
            _display = display;
            _onDestroy = onDestroy;
            spriteRenderer.sprite = sprite;
            name = $"S{meta.ShelfId}-L{meta.LayerId}-T{meta.TypeId}-{meta.Id}";
            dragObject.Init(meta.Id);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetShelf(
            ShelfItemMeta meta,
            ISpacingData spacingData,
            ShelfLayerDisplay display
        )
        {
            Meta = meta;
            _spacingData = spacingData;
            _display = display;
        }

        public void SetDisplay(ShelfLayerDisplay display)
        {
            _display = display;
        }

        public void ResetVisual()
        {
            // Visible
            if (_display == ShelfLayerDisplay.Hidden)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            // Position
            var layerOrder = (int)_display;
            var offset = (Vector3)_spacingData.GetPosition(layerOrder, Meta.SlotId);
            offset.z = MaxZ + layerOrder * 0.01f; // z càng nhỏ thì càng hiển thị trên cùng
            if (Meta.SlotId == 1)
            {
                offset.z -= 0.001f; // cho item ở giữa hiển thị nổi bật hơn 
            }

            transform.localPosition = offset;

            // Color
            spriteRenderer.color = GetDisplayColor(_display);
        }

        public void DestroyItem()
        {
            _onDestroy.Invoke(Meta);
            Destroy(gameObject);
        }

        public void Bounce(Action onCompleted, float delay = 0f)
        {
            // Reset scale
            transform.localScale = Vector3.one;

            // Sequence tween
            var seq = DOTween.Sequence();
            if (delay > 0)
            {
                seq.AppendInterval(delay);
            }
            seq.Append(transform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
            seq.Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
            seq.Append(transform.DOScale(Vector3.one, 0.1f));
            seq.OnComplete(() => onCompleted?.Invoke());
        }

        private static Color GetDisplayColor(ShelfLayerDisplay display)
        {
            return display switch
            {
                ShelfLayerDisplay.Hidden => Color.clear,
                ShelfLayerDisplay.Second => Color.gray2,
                ShelfLayerDisplay.Top => Color.white,
                _ => Color.red
            };
        }
    }
}