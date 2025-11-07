using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public interface IClaimButton
    {
        void Setup(int reward, System.Action onClick);
        void SetMode(ClaimButton.Mode mode, float multiplier = 1f);
    }
    public class ClaimButton : MonoBehaviour, IClaimButton
    {
        public enum Mode { Normal, WithAds }

        [SerializeField] private Mode mode;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private TMP_Text rewardText;

        private int baseReward;
        private float multiplier = 1f;
        private System.Action onClickCallback;

        private void Awake()
        {
            button.onClick.AddListener(() => onClickCallback?.Invoke());
            RefreshUI();
        }

        public void Setup(int reward, System.Action onClick)
        {
            baseReward = reward;
            onClickCallback = onClick;
            RefreshUI();
        }

        public void SetMode(Mode newMode, float multi = 1f)
        {
            mode = newMode;
            multiplier = multi;
            RefreshUI();
        }

        private void RefreshUI()
        {
            var displayReward = mode == Mode.WithAds
                ? Mathf.RoundToInt(baseReward * multiplier)
                : baseReward;

            rewardText.text = displayReward.ToString();

            if (mode == Mode.WithAds)
            {
                labelText.text = $"Claim x{multiplier}";
            }
            else
            {
                labelText.text = "Claim";
            }
        }
    }
}