using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
        
        public ComboVFX SetComboVFX(ComboVFXType type)
        {
            var font = type switch
            {
                ComboVFXType.Pink => pinkFont,
                ComboVFXType.Orange => orangeFont,
                ComboVFXType.Green => greenFont,
                ComboVFXType.Blue => blueFont,
                ComboVFXType.Violet => violetFont,
                ComboVFXType.Gradient => gradientFont
                };
            GetComponent<TMP_Text>().font = font;
            return this;
        }
        
        public ComboVFX SetText(string text)
        {
            GetComponent<TMP_Text>().text = text;
            return this;
        }

        public void Play()
        {
            // Text di chuyển nhẹ lên trên (1s) rồi opacity giảm dần về 0 (0.5s)
            var tmpText = GetComponent<TMP_Text>();
            var rectTransform = GetComponent<RectTransform>();
            
            var c = tmpText.color;
            c.a = 1f;
            tmpText.color = c;

            var sequence = DOTween.Sequence();

            sequence.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 25f, 1.5f)
                .SetEase(Ease.OutQuad));

            sequence.Append(tmpText.DOFade(0f, 0.5f));

            sequence.OnComplete(() =>
            {
                Debug.Log("Destroy ------->");
                Destroy(gameObject);
            });
        }
    }
}