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

        public void AddStar(int star);
    }

    public class CompleteLevelDialogController : ICompleteLevelDialogController
    {
        private readonly IAudioManager _audioManager;
        private readonly ILevelManager _levelManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly ICurrencyManager _currencyManager;

        public CompleteLevelDialogController(
            IAudioManager audioManager,
            ILevelManager levelManager,
            ISceneLoader sceneLoader,
            ICurrencyManager currencyManager
        )
        {
            _audioManager = audioManager;
            _levelManager = levelManager;
            _sceneLoader = sceneLoader;
            _currencyManager = currencyManager;
        }

        public void BackToMenuScene()
        {
            _levelManager.GoToNextLevel();
            _ = _sceneLoader.LoadScene<MainMenu>(nameof(MainMenu));
        }

        public void PlayEffect(AudioEnum audioEnum)
        {
            _audioManager.PlaySound(audioEnum);
        }

        public void AddStar(int star)
        {
            _currencyManager.AddStars(star);
        }
    }
}