using Dialog.Controller;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class OutOfLivesDialog : Dialog<OutOfLivesDialog>
    {
        [SerializeField] private Button btnShop;
        [SerializeField] private Button btnBuyLives;

        private IOutOfLivesDialogController _controller;
        private bool _isProcessing;

        protected override void Awake()
        {
            base.Awake();
            btnShop?.onClick.AddListener(OnShopClicked);
            btnBuyLives?.onClick.AddListener(OnBuyLivesClicked);
        }

        public override void Show(Canvas canvas)
        {
            IgnoreOutsideClick = true;
            OnWillShow(UpdateUI);
            base.Show(canvas);
        }

        private void UpdateUI()
        {
            var canBuy = _controller.CanBuyLives();
            btnBuyLives.interactable = canBuy;
        }

        private void OnShopClicked()
        {
            if (_isProcessing) return;
            _isProcessing = true;
            OnDidHide(_controller.GoToShop);
            Hide();
        }

        private void OnBuyLivesClicked()
        {
            if (_isProcessing) return;
            _isProcessing = true;

            if (_controller.BuyLivesAndRestart())
            {
                OnDidHide(_controller.RestartGame);
                Hide();
            }
            else
            {
                _isProcessing = false;
            }
        }
    }
}

