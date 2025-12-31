using Booster;
using manager.Interface;

namespace Dialog.Controller
{
    public interface IStarChestDialogController
    {
        int GetTotalStars();
        int GetStarsToNextChest();
        int GetChestThreshold();
        ChestReward GetNextReward();
        ChestReward ClaimChest(bool withAds = false);
        bool HasPendingChest();
    }

    public class StarChestDialogController : IStarChestDialogController
    {
        private const int ChestThreshold = 800;
        private readonly IStoreManager _storeManager;

        public StarChestDialogController(IStoreManager storeManager)
        {
            _storeManager = storeManager;
        }

        public int GetTotalStars()
        {
            return _storeManager.GetTotalStars();
        }

        public int GetStarsToNextChest()
        {
            var total = _storeManager.GetTotalStars();
            var remaining = total % ChestThreshold;
            return ChestThreshold - remaining;
        }

        public int GetChestThreshold()
        {
            return ChestThreshold;
        }

        public ChestReward GetNextReward()
        {
            return _storeManager.GetNextChestReward();
        }

        public ChestReward ClaimChest(bool withAds = false)
        {
            return _storeManager.ClaimChest(withAds);
        }

        public bool HasPendingChest()
        {
            return _storeManager.GetPendingChests() > 0;
        }
    }
}

