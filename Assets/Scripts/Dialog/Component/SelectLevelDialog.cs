using Dialog.Controller;
using TMPro;
using UnityEngine;

namespace  Dialog
{
    public class SelectLevelDialog : Dialog<SelectLevelDialog>
    {
        [SerializeField] private TextMeshProUGUI textLevel;
        private ISelectLevelDialogController _controller;

        private void Start()
        {
            UpdatePopupContent();
        }

        public void OnClickLevelButton()
        {
            _controller.OpenLevel(_controller.GetLevel());
        }

        private void UpdatePopupContent()
        {
            textLevel.text = $"Level {_controller.GetLevel()}";
        }
        
    }

}
