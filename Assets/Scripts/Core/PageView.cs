using System;
using System.Collections.Generic;
using DG.Tweening;
using UI.Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core
{
    public class PageView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("References")]
        public RectTransform content;

        [Tooltip("Optional: MenuTabIndicator to sync with page changes")]
        [SerializeField] private MenuTabIndicator menuTabIndicator;

        [Header("Settings")]
        [Tooltip("Duration of page transition animation")]
        public float transitionDuration = 0.3f;

        [Tooltip("Ease type for page transition")]
        public Ease transitionEase = Ease.OutQuad;

        [Tooltip("Minimum swipe distance (in pixels) to trigger page change")]
        public float swipeThreshold = 50f;

        [Tooltip("Minimum swipe velocity to trigger page change")]
        public float velocityThreshold = 500f;

        [Tooltip("Enable debug logging")]
        public bool enableDebugLog = false; // ✅ Disabled by default

        [Tooltip("Minimum drag distance to determine scroll direction")]
        public float directionDetectionThreshold = 10f;

        [Header("Runtime Info (Read Only)")]
        [SerializeField] private int _currentPageIndex = 0;
        [SerializeField] private int _totalPages = 0;

        // Events
        public Action<int> OnPageChanged;

        // Private fields
        private List<float> _pagePositions = new List<float>();
        private float _pageWidth;
        private bool _isDragging;
        private bool _isHorizontalDrag; // ✅ NEW: Track if this is horizontal drag
        private bool _directionLocked; // ✅ NEW: Lock direction once determined
        private Vector2 _dragStartPosition;
        private float _dragStartTime;
        private Tween _currentTween;
        
        private void Start()
        {
            Initialize();

            if (menuTabIndicator != null)
            {
                menuTabIndicator.OnChangeTabAction += GoToPage;
            }

            GoToPage(1);
        }

        private void Initialize()
        {
            if (content == null)
            {
                return;
            }
            
            var viewportRect = GetComponent<RectTransform>();
            _pageWidth = viewportRect.rect.width;
            
            _totalPages = content.childCount;
            
            if (_totalPages == 0)
            {
                return;
            }
            
            content.sizeDelta = new Vector2(_pageWidth * _totalPages, content.sizeDelta.y);
            
            _pagePositions.Clear();
            for (var i = 0; i < _totalPages; i++)
            {
                var xPos = -i * _pageWidth;
                _pagePositions.Add(xPos);
            }
            
            _currentPageIndex = 0;
            content.anchoredPosition = new Vector2(_pagePositions[0], content.anchoredPosition.y);
            
        }
        
        #region PUBLIC_METHODS
        
        /// <summary>
        /// Navigate to specific page with animation
        /// </summary>
        public void GoToPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= _totalPages) return;
            if (pageIndex == _currentPageIndex) return;

            _currentTween?.Kill();

            var oldPageIndex = _currentPageIndex;
            _currentPageIndex = pageIndex;

            // Reset old page background alpha
            FadeOutPageBackground(oldPageIndex);

            if (menuTabIndicator != null)
            {
                menuTabIndicator.SetTab(_currentPageIndex);
            }

            var targetPosition = new Vector2(_pagePositions[pageIndex], content.anchoredPosition.y);

            FadeInPageBackground(pageIndex);
            _currentTween = content.DOAnchorPos(targetPosition, transitionDuration)
                .SetEase(transitionEase)
                .OnComplete(() =>
                {
                    OnPageChanged?.Invoke(_currentPageIndex);
                });
        }
        
        /// <summary>
        /// Go to next page
        /// </summary>
        public void NextPage()
        {
            if (_currentPageIndex < _totalPages - 1)
            {
                GoToPage(_currentPageIndex + 1);
            }
        }
        
        /// <summary>
        /// Go to previous page
        /// </summary>
        public void PreviousPage()
        {
            if (_currentPageIndex > 0)
            {
                GoToPage(_currentPageIndex - 1);
            }
        }
        
        /// <summary>
        /// Get current page index
        /// </summary>
        public int GetCurrentPage()
        {
            return _currentPageIndex;
        }
        
        /// <summary>
        /// Get total number of pages
        /// </summary>
        public int GetTotalPages()
        {
            return _totalPages;
        }
        
        #endregion
        
        #region DRAG_HANDLERS
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            
            _currentTween?.Kill();

            _isDragging = true;
            _isHorizontalDrag = false; // ✅ Reset
            _directionLocked = false; // ✅ Reset
            _dragStartPosition = eventData.position;
            _dragStartTime = Time.time;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            // Calculate delta from start
            var delta = eventData.position - _dragStartPosition;

            if (!_directionLocked && delta.magnitude > directionDetectionThreshold)
            {
                var absDeltaX = Mathf.Abs(delta.x);
                var absDeltaY = Mathf.Abs(delta.y);

                _isHorizontalDrag = absDeltaX > absDeltaY;
                _directionLocked = true;
            }
            else if (!_directionLocked)
            {
            }

            if (!_isHorizontalDrag && _directionLocked)
            {
                return;
            }

            var newPosition = new Vector2(_pagePositions[_currentPageIndex] + delta.x, content.anchoredPosition.y);

            var minX = _pagePositions[_totalPages - 1]; // Last page (most negative)
            var maxX = _pagePositions[0]; // First page (0)
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

            content.anchoredPosition = newPosition;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            if (!_isHorizontalDrag && _directionLocked)
            {
                _isDragging = false;
                _directionLocked = false;
                return;
            }

            _isDragging = false;
            _directionLocked = false;

            var swipeDelta = eventData.position - _dragStartPosition;
            var swipeDistance = swipeDelta.x;
            var swipeTime = Time.time - _dragStartTime;
            var swipeVelocity = swipeTime > 0 ? swipeDistance / swipeTime : 0;

            var targetPage = _currentPageIndex;

            var isFastSwipe = Mathf.Abs(swipeVelocity) > velocityThreshold;
            var isLongSwipe = Mathf.Abs(swipeDistance) > swipeThreshold;

            if (isFastSwipe || isLongSwipe)
            {
                // Swipe right (positive delta) -> go to previous page
                if (swipeDistance > 0 && _currentPageIndex > 0)
                {
                    targetPage = _currentPageIndex - 1;
                }
                // Swipe left (negative delta) -> go to next page
                else if (swipeDistance < 0 && _currentPageIndex < _totalPages - 1)
                {
                    targetPage = _currentPageIndex + 1;
                }
            }
            else
            {
                targetPage = GetNearestPage();
            }
            GoToPage(targetPage);
        }
        
        #endregion
        
        #region PRIVATE_METHODS
        
        /// <summary>
        /// Find nearest page based on current content position
        /// </summary>
        private int GetNearestPage()
        {
            var currentX = content.anchoredPosition.x;
            var nearestPage = 0;
            var minDistance = float.MaxValue;
            
            for (var i = 0; i < _totalPages; i++)
            {
                var distance = Mathf.Abs(_pagePositions[i] - currentX);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPage = i;
                }
            }
            
            return nearestPage;
        }

        private void FadeInPageBackground(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= content.childCount) return;

            var page = content.GetChild(pageIndex);
            var image = page.GetComponent<Image>();

            if (image == null) return;

            image.DOFade(0.95f, 0.3f)
                .SetEase(Ease.OutQuad);
        }

        private void FadeOutPageBackground(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= content.childCount) return;

            var page = content.GetChild(pageIndex);
            var image = page.GetComponent<Image>();

            if (image == null) return;

            image.DOFade(0.0f, 0.1f)
                .SetEase(Ease.OutQuad);
        }

        #endregion

        private void OnDestroy()
        {
            // Clean up tween
            _currentTween?.Kill();
            if (menuTabIndicator != null)
            {
                menuTabIndicator.OnChangeTabAction -= GoToPage;
            }
        }
    }
}

