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
        void RestartGame();
    }

    public class OutOfLivesDialogController : IOutOfLivesDialogController
    {
        private const int LivesCost = 800;
        private const int MaxLives = 5;

        private readonly IProfileManager _profileManager;
        private readonly IStoreManager _storeManager;

        public OutOfLivesDialogController(
            IProfileManager profileManager,
            IStoreManager storeManager)
        {
            _profileManager = profileManager;
            _storeManager = storeManager;
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
            SceneUtils.LoadScene("MainMenu");
        }
        
        public void RestartGame()
        {
            SceneUtils.LoadScene("GameScene");
        }
    }
}

