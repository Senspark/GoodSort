using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public interface ICurrencyBar
    {
        void SetValue(int value);
        Image GetIcon();
        UniTask NumberTo(float duration, int targetValue);
    }
    public class CurrencyBar : MonoBehaviour, ICurrencyBar
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text valueText;
        private int currentValue;

        public void SetValue(int value)
        {
            currentValue = value;
            if (valueText) valueText.text = value.ToString();
        }

        public Image GetIcon()
        {
            return icon;
        }
        
        public async UniTask NumberTo(float duration, int targetValue)
        {
            var startValue = currentValue;
            var time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                var t = Mathf.Clamp01(time / duration);
                var newValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));
                if (newValue != currentValue)
                {
                    currentValue = newValue;
                    valueText.text = newValue.ToString();
                }
                await UniTask.Yield(); // chờ 1 frame
            }

            SetValue(targetValue);
        }
    }
}