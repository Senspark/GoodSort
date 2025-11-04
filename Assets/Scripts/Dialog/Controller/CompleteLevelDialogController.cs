using Defines;
using manager.Interface;
using Senspark;
using UI;
using UnityEngine;
using Utilities;

namespace Dialog.Controller
{
    public interface ICompleteLevelDialogController
    {
        public void OnNextLevelButtonPressed();
        public void PlayEffect(AudioEnum audioEnum);
    }
    
    public class CompleteLevelDialogController : ICompleteLevelDialogController
    {
        private readonly IAudioManager _audioManager;
        private readonly ILevelManager _levelManager;
        private readonly ISceneLoader _sceneLoader;
        public CompleteLevelDialogController(IAudioManager audioManager, ILevelManager levelManager, ISceneLoader sceneLoader)
        {
            _audioManager = audioManager;
            _levelManager = levelManager;
            _sceneLoader = sceneLoader;
        }
        
        public void OnNextLevelButtonPressed()
        {
            _levelManager.GoToNextLevel();
            _ = _sceneLoader.LoadScene<MainMenu>(nameof(MainMenu)).Then(_ =>
            {

            });
        }
        
        public void PlayEffect(AudioEnum audioEnum)
        {
            _audioManager.PlaySound(audioEnum);
        }
    }
}