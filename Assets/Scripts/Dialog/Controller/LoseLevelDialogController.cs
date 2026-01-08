using Cysharp.Threading.Tasks;
using Defines;
using manager.Interface;
using Senspark;
using UI;
using UnityEngine;
using Utilities;

namespace Dialog.Controller
{
    public interface ILoseLevelDialogController
    {
        bool HasLives();
        void UseLiveAndRestart();
    }

    public class LoseLevelDialogController : ILoseLevelDialogController
    {
        private readonly IProfileManager _profileManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly IAudioManager _audioManager;

        public LoseLevelDialogController(
            IProfileManager profileManager,
            ISceneLoader sceneLoader,
            IAudioManager audioManager)
        {
            _profileManager = profileManager;
            _sceneLoader = sceneLoader;
            _audioManager = audioManager;
        }

        public bool HasLives()
        {
            return _profileManager.GetLives() > 0;
        }

        public void UseLiveAndRestart()
        {
            if (_profileManager.UseLive())
            {
                SceneUtils.LoadScene("GameScene");
            }
        }
    }
}

