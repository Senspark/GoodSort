using Defines;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace Dialog.Controller
{
    public interface ISettingDialogController
    {
        public void SetEnableSound(bool isEnable);
        public void SetEnableMusic(bool isEnable);
        public void SetEnableVibrate(bool isEnable);
        public bool IsSoundEnabled();
        public bool IsMusicEnabled();
        public bool IsVibrateEnabled();
        public void OnAboutUsButtonPressed();
        public void OnContactUsButtonPressed();
    }
    
    public class SettingDialogController : ISettingDialogController
    {
        private readonly IAudioManager _audioManager;
        
        public SettingDialogController(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        
      public void SetEnableSound(bool isEnable)
            {
            _audioManager.IsSoundEnabled = isEnable;
        }
        
        public void SetEnableMusic(bool isEnable)
        {
            _audioManager.IsMusicEnabled = isEnable;
        }
        
        public void SetEnableVibrate(bool isEnable)
        {
            // TODO: Implement vibrate
        }
        
        public bool IsSoundEnabled()
        {
            return _audioManager.IsSoundEnabled;
        }
        
        public bool IsMusicEnabled()
        {
            return _audioManager.IsMusicEnabled;
        }
        
        public bool IsVibrateEnabled()
        {
            return false;
        }
        

        public void OnAboutUsButtonPressed()
        {
            Application.OpenURL("https://senspark.com/");
        }

        public void OnContactUsButtonPressed()
        {
            Application.OpenURL("https://www.facebook.com/TeamSenspark");
        }
    }
}