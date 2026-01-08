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

        public void AddCoins(int coins);
    }

    public class CompleteLevelDialogController : ICompleteLevelDialogController
    {
        private readonly IAudioManager _audioManager;
        private readonly ILevelManager _levelManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly IStoreManager _storeManager;

        public CompleteLevelDialogController(
            IAudioManager audioManager,
            ILevelManager levelManager,
            ISceneLoader sceneLoader,
            IStoreManager storeManager
        )
        {
            _audioManager = audioManager;
            _levelManager = levelManager;
            _sceneLoader = sceneLoader;
            _storeManager = storeManager;
        }

        public void BackToMenuScene()
        {
            _levelManager.GoToNextLevel();
            if (_levelManager.GetCurrentLevel() == 3 || _levelManager.GetCurrentLevel() == 2)
            {
                SceneUtils.LoadScene("GameScene");
            }
            else
            {
                SceneUtils.LoadScene("MainMenu");
            }
        }

        public void PlayEffect(AudioEnum audioEnum)
        {
            _audioManager.PlaySound(audioEnum);
        }

        public void AddCoins(int coins)
        {
            _storeManager.AddCoins(coins);
        }
    }
}