using Defines;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace Dialog.Controller
{
    public interface ISettingDialogController
    {
        public void MuteSound();
        public void UnmuteSound();
        public void MuteMusic();
        public void UnmuteMusic();
        public void TurnOnVibrate();
        public void TurnOffVibrate();
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
        public void MuteSound()
        {
            _audioManager.IsSoundEnabled = false;
        }

        public void UnmuteSound()
        {
            _audioManager.IsSoundEnabled = true;
        }

        public void MuteMusic()
        {
            _audioManager.IsMusicEnabled = false;
        }

        public void UnmuteMusic()
        {
            _audioManager.IsMusicEnabled = true;
        }

        public void TurnOnVibrate()
        {
            Debug.Log("Turn on vibrate");
        }

        public void TurnOffVibrate()
        {
            Debug.Log("Turn off vibrate");
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