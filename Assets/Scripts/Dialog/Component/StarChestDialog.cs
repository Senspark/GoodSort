using System;
using Dialog.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class StarChestDialog : Dialog<StarChestDialog>
    {
        [SerializeField] private Slider starProgress;
        [SerializeField] private TMP_Text textProgress;
        [SerializeField] private TMP_Text textRewardCoins;
        [SerializeField] private GameObject boosterRewardContainer;
        [SerializeField] private Booster.Booster boosterReward;
        [SerializeField] private Button btnClaim;

        private IStarChestDialogController _controller;
        private Action _onClaimed;

        protected override void Awake()
        {
            base.Awake();
            btnClaim.onClick.AddListener(OnClaimClicked);
        }

        public override void Show(Canvas canvas)
        {
            IgnoreOutsideClick = true;
            OnWillShow(UpdateUI);
            base.Show(canvas);
        }

        public StarChestDialog OnClaimed(Action callback)
        {
            _onClaimed = callback;
            return this;
        }

        private void UpdateUI()
        {
            var total = _controller.GetTotalStars();
            var threshold = _controller.GetChestThreshold();
            var progress = total % threshold;

            starProgress.maxValue = threshold;
            starProgress.value = progress;
            textProgress.text = $"{progress}/{threshold}";

            var reward = _controller.GetNextReward();
            textRewardCoins.text = $"+{reward.Coins}";

            if (reward.BoosterQuantity > 0)
            {
                boosterRewardContainer.SetActive(true);
                boosterReward.SetType(reward.Booster).SetQuantity(reward.BoosterQuantity);
            }
            else
            {
                boosterRewardContainer.SetActive(false);
            }
        }

        private void OnClaimClicked()
        {
            var reward = _controller.ClaimChest();
            Hide();
            _onClaimed?.Invoke();
        }
    }
}

