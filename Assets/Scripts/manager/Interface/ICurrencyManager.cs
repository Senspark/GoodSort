using System;
using Booster;
using Senspark;

namespace manager.Interface
{
    public class CurrencyManagerObserver
    {
        public Action<int> OnCoinsChanged { get; set; }
        public Action<int> OnStarsChanged { get; set; }
    }
    
    public struct ChestReward
    {
        public int Coins;
        public BoosterType Booster;
        public int BoosterQuantity;
    }

    [Service(typeof(ICurrencyManager))]
    public interface ICurrencyManager : IObserverManager<CurrencyManagerObserver>, IService
    {
        void AddCoins(int amount);
        void SetCoins(int coins);
        int GetCoins();

        void AddStars(int amount);
        int GetTotalStars();
        int GetPendingChests();
        int GetChestClaimCount();
        ChestReward GetNextChestReward();
        ChestReward ClaimChest(bool withAds = false);
    }
}

