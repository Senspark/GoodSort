using System;
using DG.Tweening;
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
        [SerializeField] private Booster.Booster boosterReward;
        [SerializeField] private RectTransform panelStarChest;
        [SerializeField] private RectTransform panelOpenChest;
        [SerializeField] private Button btnContinue;
        [SerializeField] private Button btnClaim;
        [SerializeField] private Button btnClaimX2;

        private IStarChestDialogController _controller;
        private Action _onClaimed;

        protected override void Awake()
        {
            base.Awake();
            btnContinue.onClick.AddListener(OnContinueClicked);
            btnClaim.onClick.AddListener(OnClaimClicked);
            btnClaimX2.onClick.AddListener(OnClaimX2Clicked);
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
                boosterReward.SetType(reward.Booster).SetQuantity(reward.BoosterQuantity);
            }
            panelStarChest.gameObject.SetActive(true);
            panelOpenChest.gameObject.SetActive(false);
            
            // if not enough stars to claim, disable continue button
            btnContinue.interactable = progress >= threshold;
        }

        private void OnClaimClicked()
        {
            _controller.ClaimChest();
            Hide();
            _onClaimed?.Invoke();
        }
        
        private void OnClaimX2Clicked()
        {
            _controller.ClaimChest(true);
            Hide();
            _onClaimed?.Invoke();
        }
        
        private void OnContinueClicked()
        {
            // immediately hide star chest panel and tween fade in panel open chest
            panelStarChest.gameObject.SetActive(false);
            panelOpenChest.gameObject.SetActive(true);
        }
    }
}

