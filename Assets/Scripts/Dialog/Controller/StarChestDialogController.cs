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
        ChestReward ClaimChest();
        bool HasPendingChest();
    }

    public class StarChestDialogController : IStarChestDialogController
    {
        private const int ChestThreshold = 800;
        private readonly ICurrencyManager _currencyManager;

        public StarChestDialogController(ICurrencyManager currencyManager)
        {
            _currencyManager = currencyManager;
        }

        public int GetTotalStars()
        {
            return _currencyManager.GetTotalStars();
        }

        public int GetStarsToNextChest()
        {
            var total = _currencyManager.GetTotalStars();
            var remaining = total % ChestThreshold;
            return ChestThreshold - remaining;
        }

        public int GetChestThreshold()
        {
            return ChestThreshold;
        }

        public ChestReward GetNextReward()
        {
            return _currencyManager.GetNextChestReward();
        }

        public ChestReward ClaimChest()
        {
            return _currencyManager.ClaimChest();
        }

        public bool HasPendingChest()
        {
            return _currencyManager.GetPendingChests() > 0;
        }
    }
}

