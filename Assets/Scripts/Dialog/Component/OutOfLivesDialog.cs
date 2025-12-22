using Dialog.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class OutOfLivesDialog : Dialog<OutOfLivesDialog>
    {
        [SerializeField] private Button btnShop;
        [SerializeField] private Button btnBuyLives;

        private IOutOfLivesDialogController _controller;

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
            _controller.GoToShop();
        }

        private void OnBuyLivesClicked()
        {
            _controller.BuyLivesAndRestart();
        }
    }
}

