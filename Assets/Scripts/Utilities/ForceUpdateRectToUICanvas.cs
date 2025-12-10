using System;
using UnityEngine;

namespace Utilities
{
    [ExecuteInEditMode]
    public class ForceUpdateRectToUICanvas : MonoBehaviour
    {
        [SerializeField] private RectTransform uiCanvas;
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            UpdateRect();
        }
        
        private void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        private void UpdateRect()
        {
            _rectTransform = GetComponent<RectTransform>();
            Debug.Log(uiCanvas.rect.width);
            Debug.Log(uiCanvas.rect.height);
            _rectTransform.sizeDelta = new Vector2(uiCanvas.rect.width, uiCanvas.rect.height);
        }
    }
}