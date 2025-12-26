using System;
using Booster;
using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace manager
{
    public class DefaultCurrencyManager : ObserverManager<CurrencyManagerObserver>, ICurrencyManager
    {
        private const string SaveKey = nameof(DefaultCurrencyManager);
        private const int ChestThreshold = 800;

        private static readonly ChestReward[] ChestRewards =
        {
            new() { Coins = 100, Booster = BoosterType.MagicHat, BoosterQuantity = 0 },
            new() { Coins = 50, Booster = BoosterType.X2Star, BoosterQuantity = 1 },
            new() { Coins = 50, Booster = BoosterType.Magnet, BoosterQuantity = 1 },
            new() { Coins = 50, Booster = BoosterType.Freeze, BoosterQuantity = 1 },
            new() { Coins = 50, Booster = BoosterType.MagicStaff, BoosterQuantity = 1 },
        };

        private readonly IDataManager _dataManager;
        private int _coins;
        private int _totalStars;
        private int _chestClaimCount;

        public DefaultCurrencyManager(IDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public UniTask<bool> Initialize()
        {
            LoadData();
            return UniTask.FromResult(true);
        }

        public void AddCoins(int amount)
        {
            _coins += amount;
            SaveData();
            DispatchEvent(observer => observer.OnCoinsChanged?.Invoke(_coins));
        }

        public void SetCoins(int coins)
        {
            _coins = coins;
            SaveData();
            DispatchEvent(observer => observer.OnCoinsChanged?.Invoke(_coins));
        }

        public int GetCoins()
        {
            return _coins;
        }

        public void AddStars(int amount)
        {
            _totalStars += amount;
            SaveData();
            DispatchEvent(observer => observer.OnStarsChanged?.Invoke(_totalStars));
        }

        public int GetTotalStars()
        {
            return _totalStars;
        }

        public int GetPendingChests()
        {
            return _totalStars / ChestThreshold;
        }

        public int GetChestClaimCount()
        {
            return _chestClaimCount;
        }

        public ChestReward GetNextChestReward()
        {
            var index = _chestClaimCount % ChestRewards.Length;
            return ChestRewards[index];
        }

        public ChestReward ClaimChest()
        {
            if (GetPendingChests() <= 0)
            {
                return default;
            }

            var reward = GetNextChestReward();
            _totalStars -= ChestThreshold;
            _chestClaimCount++;
            _coins += reward.Coins;
            SaveData();
            DispatchEvent(observer => observer.OnCoinsChanged?.Invoke(_coins));
            DispatchEvent(observer => observer.OnStarsChanged?.Invoke(_totalStars));
            return reward;
        }

        private void LoadData()
        {
            try
            {
                var raw = _dataManager.Get(SaveKey, "{}");
                var data = JsonUtility.FromJson<CurrencyData>(raw);
                _coins = data.coins;
                _totalStars = data.totalStars;
                _chestClaimCount = data.chestClaimCount;
            }
            catch (Exception)
            {
                _coins = 0;
                _totalStars = 0;
                _chestClaimCount = 0;
            }
        }

        private void SaveData()
        {
            var data = new CurrencyData
            {
                coins = _coins,
                totalStars = _totalStars,
                chestClaimCount = _chestClaimCount
            };
            var json = JsonUtility.ToJson(data);
            _dataManager.Set(SaveKey, json);
        }

        [Serializable]
        private class CurrencyData
        {
            public int coins;
            public int totalStars;
            public int chestClaimCount;
        }
    }
}

