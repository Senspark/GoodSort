using Cysharp.Threading.Tasks;
using Dialog.Controller;
using Utilities;

namespace Dialog
{
    public class SettingDialog : Dialog<SettingDialog>
    {
        private ISettingDialogController _controller;
        
        public void OnMuteSoundButtonPressed()
        {
            _controller.MuteSound();
        }
        
        public void OnUnmuteSoundButtonPressed()
        {
            _controller.UnmuteSound();
        }
        
        public void OnMuteMusicButtonPressed()
        {
            _controller.MuteMusic();
        }
        
        public void OnUnmuteMusicButtonPressed()
        {
            _controller.UnmuteMusic();
        }
        
        public void OnTurnOnVibrateButtonPressed()
        {
            _controller.TurnOnVibrate();
        }
        
        public void OnTurnOffVibrateButtonPressed()
        {
            _controller.TurnOffVibrate();
        }
        
        public void OnAboutUsButtonPressed()
        {
            _controller.OnAboutUsButtonPressed();
        }
        
        public void OnContactUsButtonPressed()
        {
            _controller.OnContactUsButtonPressed();
        }
    }
}