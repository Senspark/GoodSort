using Defines;
using manager.Interface;
using Senspark;
using UI;
using Utilities;

namespace Dialog.Controller
{
    public interface IOutOfLivesDialogController
    {
        bool CanBuyLives();
        bool BuyLivesAndRestart();
        void GoToShop();
    }

    public class OutOfLivesDialogController : IOutOfLivesDialogController
    {
        private const int LivesCost = 800;
        private const int MaxLives = 5;

        private readonly IProfileManager _profileManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly IAudioManager _audioManager;

        public OutOfLivesDialogController(
            IProfileManager profileManager,
            ISceneLoader sceneLoader,
            IAudioManager audioManager)
        {
            _profileManager = profileManager;
            _sceneLoader = sceneLoader;
            _audioManager = audioManager;
        }

        public bool CanBuyLives()
        {
            return _profileManager.GetCoins() >= LivesCost;
        }

        public bool BuyLivesAndRestart()
        {
            if (!CanBuyLives()) return false;
            _profileManager.AddCoins(-LivesCost);
            _profileManager.SetLives(MaxLives);
            _ = _sceneLoader.LoadScene<GameScene>(nameof(GameScene));
            return true;
        }

        public void GoToShop()
        {
            _ = _sceneLoader.LoadScene<MainMenu>(nameof(MainMenu));
        }
    }
}

