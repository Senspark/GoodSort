using Core;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Menu
{
    /// <summary>
    /// NestedScroll - ScrollRect that supports nested scrolling.
    /// Detects scroll direction and forwards horizontal scrolls to parent PageView.
    /// </summary>
    public class NestedScroll : ScrollRect
    {
        private PageView _parentPageView;
        private bool _isParentScrolling;

        [Header("Debug")]
        public bool enableDebugLog = false;

        protected override void Start()
        {
            base.Start();

            // Find parent PageView
            _parentPageView = transform.parent.GetComponentInParent<PageView>();

            if (enableDebugLog)
            {
                Debug.Log($"[NestedScroll] Initialized. Parent PageView: {(_parentPageView != null ? _parentPageView.name : "NULL")}");
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            // Detect scroll direction: horizontal or vertical
            _isParentScrolling = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

            if (_isParentScrolling && _parentPageView != null)
            {
                // Horizontal scroll -> forward to parent PageView
                if (enableDebugLog)
                {
                    Debug.Log($"[NestedScroll] Horizontal scroll -> Forwarding to PageView");
                }
                _parentPageView.OnBeginDrag(eventData);
            }
            else
            {
                // Vertical scroll -> handle locally
                if (enableDebugLog)
                {
                    Debug.Log($"[NestedScroll] Vertical scroll -> Handling locally");
                }
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_isParentScrolling && _parentPageView != null)
            {
                // Forward to parent PageView
                _parentPageView.OnDrag(eventData);
            }
            else
            {
                // Handle locally
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_isParentScrolling && _parentPageView != null)
            {
                // Forward to parent PageView
                _parentPageView.OnEndDrag(eventData);
            }
            else
            {
                // Handle locally
                base.OnEndDrag(eventData);
            }
        }
    }
}
