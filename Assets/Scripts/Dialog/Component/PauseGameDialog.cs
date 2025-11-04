using System;
using Dialog.Controller;
using UnityEngine;

namespace Dialog
{
    public class PauseGameDialog : Dialog<PauseGameDialog>
    {
        [SerializeField] private GameObject soundOn;
        [SerializeField] private GameObject musicOn;
        [SerializeField] private GameObject vibrateOn;
        
        private Action _onContinue;
        private Action _onQuit;
        private ISettingDialogController _controller;
        
        public void SetActions(Action onContinue, Action onQuit)
        {
            _onContinue = onContinue;
            _onQuit = onQuit;
        }
        
        public void OnClickContinueButton()
        {
            _onContinue?.Invoke();
        }
        
        public void OnClickQuitButton()
        {
            _onQuit?.Invoke();
        }

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