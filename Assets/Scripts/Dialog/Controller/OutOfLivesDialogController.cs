using manager.Interface;
using Senspark;
using UI;

namespace Dialog.Controller
{
    public interface IOutOfLivesDialogController
    {
        bool CanBuyLives();
        bool BuyLivesAndRestart();
        void GoToShop();
        void RestartGame();
    }

    public class OutOfLivesDialogController : IOutOfLivesDialogController
    {
        private const int LivesCost = 800;
        private const int MaxLives = 5;

        private readonly IProfileManager _profileManager;
        private readonly IStoreManager _storeManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly IAudioManager _audioManager;

        public OutOfLivesDialogController(
            IProfileManager profileManager,
            IStoreManager storeManager,
            ISceneLoader sceneLoader,
            IAudioManager audioManager)
        {
            _profileManager = profileManager;
            _storeManager = storeManager;
            _sceneLoader = sceneLoader;
            _audioManager = audioManager;
        }

        public bool CanBuyLives()
        {
            return _storeManager.GetCoins() >= LivesCost;
        }

        public bool BuyLivesAndRestart()
        {
            if (!CanBuyLives()) return false;
            _storeManager.AddCoins(-LivesCost);
            _profileManager.SetLives(MaxLives);
            return true;
        }

        public void GoToShop()
        {
            _ = _sceneLoader.LoadScene<MainMenu>(nameof(MainMenu));
        }
        
        public void RestartGame()
        {
            _ = _sceneLoader.LoadScene<GameScene>(nameof(GameScene));
        }
    }
}

