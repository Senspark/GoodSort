using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MenuTabIndicator : MonoBehaviour
    {
        [SerializeField] private RectTransform[] tabRects; // Các tab (RectTransform)
        [SerializeField] private RectTransform containRct; // Container chứa các tab
        [SerializeField] private RectTransform selectedImage; // Indicator của tab được chọn
        [SerializeField] private Transform[] iconTabs; // Icons của các tab
        [SerializeField] private Transform[] titleTabs;
        [SerializeField] private float expandRatio = 1.2f; // Tỷ lệ phóng to

        public Action<int> OnChangeTabAction;

        private int _selectedIndex = -1;
        private bool _isAnimating = false;
        private int _tweenRunning = 0;

        private void Awake()
        {
            if (tabRects.Length == 0) return;

            // Set width of containRect to match MenuTabIndicator width
            var menuTabIndicatorRect = GetComponent<RectTransform>();
            var width = menuTabIndicatorRect.rect.width;
            containRct.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

            // Initialize icon scales
            InitializeIconScales();

            for (var i = 0; i < tabRects.Length; i++)
            {
                var index = i;
                var button = tabRects[i].GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnTabClicked(index));
                }
            }
            SetTab(1);
        }

        private void InitializeIconScales()
        {
            if (iconTabs == null || iconTabs.Length == 0) return;

            // Set all icons to unselected scale initially
            for (var i = 0; i < iconTabs.Length; i++)
            {
                if (iconTabs[i] != null)
                {
                    iconTabs[i].localScale = Vector3.one * 0.8f;
                }
            }

            // Initialize title tabs - set all to inactive
            if (titleTabs != null && titleTabs.Length > 0)
            {
                for (var i = 0; i < titleTabs.Length; i++)
                {
                    if (titleTabs[i] != null)
                    {
                        titleTabs[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        private void OnTabClicked(int index)
        {
            if (_isAnimating) return;
            OnChangeTabAction?.Invoke(index);
        }

        public void SetTab(int index)
        {
            if (index == _selectedIndex || index < 0 || index >= tabRects.Length)
                return;

            _selectedIndex = index;
            AnimateToSelected(index);
        }

        private void AnimateToSelected(int selected)
        {
            var count = tabRects.Length;
            if (count == 0) return;

            var totalWidth = containRct.rect.width;
            var baseWidth = totalWidth / count;
            var expandedWidth = baseWidth * expandRatio;

            var reducedWidth = (count > 1) ? (totalWidth - expandedWidth) / (count - 1) : totalWidth;

            var widths = new float[count];
            for (var i = 0; i < count; i++)
                widths[i] = (i == selected) ? expandedWidth : reducedWidth;

            var groupWidth = 0f;
            for (var i = 0; i < count; i++) groupWidth += widths[i];

            var currentLeft = (totalWidth - groupWidth) / 2f;

            for (var i = 0; i < count; i++)
            {
                var rect = tabRects[i];
                var targetWidth = widths[i];

                var targetLeftX = currentLeft;

                _tweenRunning++;
                rect.DOSizeDelta(new Vector2(targetWidth, rect.sizeDelta.y), 0.3f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(CheckTweenDone).SetLink(gameObject);

                _tweenRunning++;
                rect.DOAnchorPosX(targetLeftX, 0.3f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(CheckTweenDone).SetLink(gameObject);

                currentLeft += targetWidth;
            }

            // Animate icon scales
            AnimateIconScales(selected);

            if (selectedImage != null)
            {
                var selectedImageLeft = (totalWidth - groupWidth) / 2f; 
                
                for (var i = 0; i < selected; i++) 
                    selectedImageLeft += widths[i];

                var targetWidth = widths[selected];
                
                var targetLeftX = selectedImageLeft;
                
                _tweenRunning++;
                selectedImage.DOSizeDelta(new Vector2(targetWidth, selectedImage.sizeDelta.y), 0.3f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(CheckTweenDone).SetLink(gameObject);

                _tweenRunning++;
                selectedImage.DOAnchorPosX(targetLeftX, 0.3f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(CheckTweenDone).SetLink(gameObject);
            }

            _isAnimating = true;
        }

        private void AnimateIconScales(int selected)
        {
            if (iconTabs == null || iconTabs.Length == 0) return;

            for (var i = 0; i < iconTabs.Length; i++)
            {
                if (iconTabs[i] == null) continue;

                var targetScale = (i == selected) ? 1.2f : 0.8f;

                _tweenRunning++;
                iconTabs[i].DOScale(targetScale, 0.3f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(CheckTweenDone)
                    .SetLink(gameObject);
            }

            // Update title tabs active state
            UpdateTitleTabs(selected);
        }

        private void UpdateTitleTabs(int selected)
        {
            if (titleTabs == null || titleTabs.Length == 0) return;

            for (var i = 0; i < titleTabs.Length; i++)
            {
                if (titleTabs[i] != null)
                {
                    titleTabs[i].gameObject.SetActive(i == selected);
                }
            }
        }

        private void CheckTweenDone()
        {
            _tweenRunning--;
            if (_tweenRunning <= 0)
            {
                _tweenRunning = 0;
                _isAnimating = false;
            }
        }
    }
}