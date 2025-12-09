using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core
{
    public class PageView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        private ScrollRect _rect;
        private float _targetHorizontal;
        private bool _isDrag;
        private readonly List<float> _posList = new(); 
        private int _currentPageIndex = -1;
        public Action<int> OnPageChanged;
        public RectTransform content;
        private bool _stopMove = true;
        public float smooth = 4;
        public float swipeThreshold = 0.05f; 
        private float _startTime;

        private float _startDragHorizontal;

        private void Start()
        {
            _rect = transform.GetComponent<ScrollRect>();
            var rectWidth = GetComponent<RectTransform>();
            var tempWidth = content.transform.childCount * rectWidth.rect.width;
            content.sizeDelta = new Vector2(tempWidth, rectWidth.rect.height);
            var horizontalLength = content.rect.width - rectWidth.rect.width;
            for (var i = 0; i < _rect.content.transform.childCount; i++)
            {
                _posList.Add(rectWidth.rect.width * i / horizontalLength);
            }
            // for (var i = 0; i < content.childCount; i++)
            // {
            //     content.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(rectWidth.rect.width, rectWidth.rect.height);
            // }
        }

        private void Update()
        {
            if (!_isDrag && !_stopMove)
            {
                _rect.horizontalNormalizedPosition = Mathf.Lerp(
                    _rect.horizontalNormalizedPosition,
                    _targetHorizontal,
                    Time.deltaTime * smooth
                );

                if (Mathf.Abs(_rect.horizontalNormalizedPosition - _targetHorizontal) < 0.0001f)
                {
                    _rect.horizontalNormalizedPosition = _targetHorizontal;
                    _stopMove = true;
                }
            }
        }

        public void PageTo(int index)
        {
            if (index >= 0 && index < _posList.Count)
            {
                _rect.horizontalNormalizedPosition = _posList[index];
                SetPageIndex(index);
            }
        }

        private void SetPageIndex(int index)
        {
            if (_currentPageIndex != index)
            {
                _currentPageIndex = index;
                if (OnPageChanged != null)
                    OnPageChanged(index);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDrag = true;
            _startDragHorizontal = _rect.horizontalNormalizedPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var posX = _rect.horizontalNormalizedPosition;
            var delta = posX - _startDragHorizontal;

            var index = _currentPageIndex;

            if (Mathf.Abs(delta) > swipeThreshold)
            {
                if (delta > 0 && index < _posList.Count - 1)
                    index++;
                else if (delta < 0 && index > 0)
                    index--;
            }
            else
            {
                index = _currentPageIndex;
            }

            SetPageIndex(index);
            _targetHorizontal = _posList[index];

            _isDrag = false;
            _stopMove = false;
        }
    }
}