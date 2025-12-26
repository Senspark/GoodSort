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
        private readonly ICurrencyManager _currencyManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly IAudioManager _audioManager;

        public OutOfLivesDialogController(
            IProfileManager profileManager,
            ICurrencyManager currencyManager,
            ISceneLoader sceneLoader,
            IAudioManager audioManager)
        {
            _profileManager = profileManager;
            _currencyManager = currencyManager;
            _sceneLoader = sceneLoader;
            _audioManager = audioManager;
        }

        public bool CanBuyLives()
        {
            return _currencyManager.GetCoins() >= LivesCost;
        }

        public bool BuyLivesAndRestart()
        {
            if (!CanBuyLives()) return false;
            _currencyManager.AddCoins(-LivesCost);
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

