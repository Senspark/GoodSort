using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Menu
{
    public class NestedScroll : ScrollRect
    {
        private ScrollRect _parentScrollRect;
        private bool _isParentScrolling;
        protected override void Start()
        {
            _parentScrollRect = transform.parent.GetComponentInParent<ScrollRect>();
        }
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            _isParentScrolling = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);
            if (_isParentScrolling)
            {
                _parentScrollRect.OnBeginDrag(eventData);
            }
            else
            {
                base.OnBeginDrag(eventData);
            }
        }
        
        public override void OnDrag(PointerEventData eventData)
        {
            if (_isParentScrolling)
            {
                _parentScrollRect.OnDrag(eventData);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }
        
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_isParentScrolling)
            {
                _parentScrollRect.OnEndDrag(eventData);
            }
            else
            {
                base.OnEndDrag(eventData);
            }
        }
    }
}
