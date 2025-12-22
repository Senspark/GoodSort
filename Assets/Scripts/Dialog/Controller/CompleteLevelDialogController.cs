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
        public void BackToMenuScene();
        public void PlayEffect(AudioEnum audioEnum);

        public void AddCoin(int newCoin);
    }

    public class CompleteLevelDialogController : ICompleteLevelDialogController
    {
        private readonly IAudioManager _audioManager;
        private readonly ILevelManager _levelManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly IProfileManager _profileManager;

        public CompleteLevelDialogController(
            IAudioManager audioManager,
            ILevelManager levelManager,
            ISceneLoader sceneLoader,
            IProfileManager profileManager
        )
        {
            _audioManager = audioManager;
            _levelManager = levelManager;
            _sceneLoader = sceneLoader;
            _profileManager = profileManager;
        }

        public void BackToMenuScene()
        {
            _levelManager.GoToNextLevel();
            Debug.Log($"Current Level: {_levelManager.GetCurrentLevel()}");
            _ = _sceneLoader.LoadScene<MainMenu>(nameof(MainMenu)).Then(_ =>
            {
                _audioManager.PlayMusic(AudioEnum.MenuMusic);
            });
        }

        public void PlayEffect(AudioEnum audioEnum)
        {
            _audioManager.PlaySound(audioEnum);
        }

        public void AddCoin(int newCoin)
        {
            // Use ProfileManager instead of DataManager
            // This will automatically notify all observers about the coin change
            _profileManager.AddCoins(newCoin);
        }
    }
}