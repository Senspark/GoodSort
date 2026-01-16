using System;
using Core;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Utilities;

namespace Game
{
    public class ShelfItemBasic : MonoBehaviour, IShelfItem
    {
        [Required, SerializeField] private SpriteRenderer spriteRenderer;
        [Required, SerializeField] public DragObject2 dragObject;
        [Required, SerializeField] public TextMeshPro debugText;

        public ShelfItemMeta Meta { get; private set; }
        public IDragObject DragObject => dragObject;

        private const float MaxZ = -1.0f;
        private ISpacingData _spacingData;
        private ShelfLayerDisplay _display;
        private Action<ShelfItemMeta> _onDestroy;
        private IShelf2 _shelf;

        private void Awake()
        {
#if !UNITY_EDITOR
            Destroy(debugText.gameObject);
#endif
        }

        public void Init(
            ShelfItemMeta meta,
            ISpacingData spacingData,
            ShelfLayerDisplay display,
            Action<ShelfItemMeta> onDestroy,
            Sprite sprite,
            IShelf2 shelf
        )
        {
            Meta = meta;
            _spacingData = spacingData;
            _display = display;
            _onDestroy = onDestroy;
            _shelf = shelf;
            spriteRenderer.sprite = sprite;
            name = $"S{meta.ShelfId}-L{meta.LayerId}-T{meta.TypeId}-{meta.Id}";
            dragObject.Init(meta.Id, CanBeDragged);
#if UNITY_EDITOR
            debugText.text = meta.TypeId.ToString();
#endif
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

        private bool CanBeDragged()
        {
            // Check display layer
            if (_display != ShelfLayerDisplay.Top) return false;

            // Check shelf lock
            if (_shelf != null && _shelf.LockCount > 0)
            {
                return false;
            }

            return true;
        }

        public void ResetVisual()
        {
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
            spriteRenderer.color = GetDisplayColor(_display);
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        
        public void FadeInVisual(float duration)
        {
            if (_display == ShelfLayerDisplay.Top)
            {
                gameObject.SetActive(true);
                var layerOrder = (int)_display;
                var offset = (Vector3)_spacingData.GetPosition(layerOrder, Meta.SlotId);
                offset.z = MaxZ + layerOrder * 0.01f; // z càng nhỏ thì càng hiển thị trên cùng
                if (Meta.SlotId == 1)
                {
                    offset.z -= 0.001f; // cho item ở giữa hiển thị nổi bật hơn 
                }
                // transform.localPosition = offset;
                var targetColor = GetDisplayColor(_display);
                spriteRenderer.DOColor(targetColor, duration)
                    .SetEase(Ease.OutQuad);
                transform.DOLocalMove(offset, 0.2f).SetEase(Ease.OutQuad);
            }
        }

        public void DestroyItem()
        {
            var effectPosition = new Vector3(
                transform.position.x, 
                transform.position.y + 0.5f, 
                transform.position.z
            );
            EffectUtils.Blink(effectPosition);
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

        public void Jiggly()
        {
            transform.localScale = Vector3.one;
            var seq = DOTween.Sequence();
            seq.Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.15f));
            seq.Append(transform.DOScale(Vector3.one, 0.15f));
        }

        private static Color GetDisplayColor(ShelfLayerDisplay display)
        {
            return display switch
            {
                ShelfLayerDisplay.Hidden => Color.clear,
                ShelfLayerDisplay.Second => new Color(0.31f, 0.31f, 0.31f, 1f),
                ShelfLayerDisplay.Top => Color.white,
                _ => Color.red
            };
        }
    }
}