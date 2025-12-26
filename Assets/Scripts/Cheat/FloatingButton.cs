using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Cheat
{
    public class DraggableFloatingButton : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private RectTransform panelContent;
        private readonly float _slideDuration = 0.3f;
        private readonly float _padding = 20f;

        private RectTransform _rectTransform;
        private Canvas _canvas;
        private bool _isOpen = false;
        private bool _isDragging = false;
 
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData) => _isDragging = true;

        public void OnDrag(PointerEventData eventData)
        {
            var targetPos = _rectTransform.position + (Vector3)(eventData.delta);
    
            _rectTransform.position = targetPos;
        
            ClampToScreen();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isDragging) return;

            _isOpen = !_isOpen;
            
            panelContent.DOScaleY(_isOpen ? 1 : 0, _slideDuration);
        }

        private void ClampToScreen()
        {
            var size = _rectTransform.rect.size * _canvas.scaleFactor;
    
            var minX = (_rectTransform.pivot.x * size.x) + _padding;
            var maxX = Screen.width - ((1f - _rectTransform.pivot.x) * size.x) - _padding;
    
            var minY = (_rectTransform.pivot.y * size.y) + _padding;
            var maxY = Screen.height - ((1f - _rectTransform.pivot.y) * size.y) - _padding;

            var currentPos = _rectTransform.position;

            currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
            currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);

            _rectTransform.position = currentPos;
        }
    }
}