using Dialog.Controller;
using UnityEngine;

namespace Dialog
{
    public class SettingDialog : Dialog<SettingDialog>
    {
        [SerializeField] private GameObject soundOn;
        [SerializeField] private GameObject musicOn;
        [SerializeField] private GameObject vibrateOn;
        
        private ISettingDialogController _controller;
        
        public void OnToggleSound()
        {
            var isEnable = _controller.IsSoundEnabled();
            soundOn.SetActive(!isEnable);
            _controller.SetEnableSound(!isEnable);
        }
        
        public void OnToggleMusic()
        {
            var isEnable = _controller.IsMusicEnabled();
            musicOn.SetActive(!isEnable);
            _controller.SetEnableMusic(!isEnable);
        }
        
        public void OnToggleVibrate()
        {
            // TODO: Implement vibrate
            var isEnable = _controller.IsVibrateEnabled();
            vibrateOn.SetActive(!isEnable);
            _controller.SetEnableVibrate(!isEnable);
        }
        
        public void OnAboutUsButtonPressed()
        {
            _controller.OnAboutUsButtonPressed();
        }
        
        public void OnContactUsButtonPressed()
        {
            _controller.OnContactUsButtonPressed();
        }
        
        public override void Show(Canvas canvas)
        {
            OnWillShow(() =>
            {
                UpdateUI();
            });
            base.Show(canvas);
        }
        
        private void UpdateUI()
        {
            soundOn.SetActive(_controller.IsSoundEnabled());
            musicOn.SetActive(_controller.IsMusicEnabled());
            vibrateOn.SetActive(_controller.IsVibrateEnabled());
        }
    }
}