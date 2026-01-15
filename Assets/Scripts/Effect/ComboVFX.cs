using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Effect
{
    public enum ComboVFXType
    {
        Pink,
        Orange,
        Green,
        Blue,
        Violet,
        Gradient,
    }
    public class ComboVFX : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset pinkFont;
        [SerializeField] private TMP_FontAsset orangeFont;
        [SerializeField] private TMP_FontAsset greenFont;
        [SerializeField] private TMP_FontAsset blueFont;
        [SerializeField] private TMP_FontAsset violetFont;
        [SerializeField] private TMP_FontAsset gradientFont;
        
        private TMP_Text _tmpText;
        private RectTransform _rectTransform;
        
        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            _rectTransform = GetComponent<RectTransform>();
        }
        
        public ComboVFX SetComboVFX(ComboVFXType type)
        {
            _tmpText.font = type switch
            {
                ComboVFXType.Pink => pinkFont,
                ComboVFXType.Orange => orangeFont,
                ComboVFXType.Green => greenFont,
                ComboVFXType.Blue => blueFont,
                ComboVFXType.Violet => violetFont,
                _ => gradientFont
            };
            return this;
        }
        
        public ComboVFX SetText(string text)
        {
            _tmpText.text = text;
            return this;
        }

        public void Play()
        {
            _tmpText.alpha = 1f;
            var sequence = DOTween.Sequence();
            sequence.Append(_rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + 15f, 1f)
                .SetEase(Ease.OutBack));
            sequence.Insert(0.5f, _tmpText.DOFade(0f, 0.5f));
            sequence.OnComplete(() =>
            {
                Destroy(gameObject);
            });
            sequence.SetTarget(this);
        }
    }
}