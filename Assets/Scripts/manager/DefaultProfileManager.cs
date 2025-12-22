using System;
using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace manager
{
    public class DefaultProfileManager : ObserverManager<ProfileManagerObserver>, IProfileManager
    {
        private const string SaveKey = nameof(DefaultProfileManager);
        private const string DefaultName = "Player";
        private const string DefaultAvatarId = "avt_0";

        private readonly IDataManager _dataManager;

        private string _name;
        private string _avatarId;
        private int _coins;

        public DefaultProfileManager(IDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public UniTask<bool> Initialize()
        {
            LoadData();
            return UniTask.FromResult(true);
        }

        #region Setters

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("[ProfileManager] Cannot set empty name");
                return;
            }

            _name = name;
            SaveData();

            // Dispatch event to all observers
            DispatchEvent(observer => observer.OnNameChanged?.Invoke(_name));
        }

        public void SetAvatarId(string avatarId)
        {
            if (string.IsNullOrEmpty(avatarId))
            {
                Debug.LogWarning("[ProfileManager] Cannot set empty avatar ID");
                return;
            }

            _avatarId = avatarId;
            SaveData();

            // Dispatch event to all observers
            DispatchEvent(observer => observer.OnAvatarIdChanged?.Invoke(_avatarId));
        }

        public void SetCoins(int coins)
        {
            _coins = coins;
            SaveData();

            // Dispatch event to all observers
            DispatchEvent(observer => observer.OnCoinsChanged?.Invoke(_coins));
        }

        public void AddCoins(int amount)
        {
            _coins += amount;
            SaveData();

            // Dispatch event to all observers
            DispatchEvent(observer => observer.OnCoinsChanged?.Invoke(_coins));
        }

        #endregion

        #region Getters

        public string GetName()
        {
            return _name;
        }

        public string GetAvatarId()
        {
            return _avatarId;
        }

        public int GetCoins()
        {
            return _coins;
        }

        #endregion

        #region Data Persistence

        private void LoadData()
        {
            try
            {
                var raw = _dataManager.Get(SaveKey, "{}");
                var data = JsonUtility.FromJson<ProfileData>(raw);

                _name = string.IsNullOrEmpty(data.name) ? DefaultName : data.name;
                _avatarId = string.IsNullOrEmpty(data.avatarId) ? DefaultAvatarId : data.avatarId;
                _coins = data.coins;

                Debug.Log($"[ProfileManager] Loaded profile - Name: {_name}, Avatar: {_avatarId}, Coins: {_coins}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ProfileManager] Failed to load data: {e.Message}. Using defaults.");
                _name = DefaultName;
                _avatarId = DefaultAvatarId;
                _coins = 0;
            }
        }

        private void SaveData()
        {
            var data = new ProfileData
            {
                name = _name,
                avatarId = _avatarId,
                coins = _coins
            };

            var json = JsonUtility.ToJson(data);
            _dataManager.Set(SaveKey, json);
        }

        [Serializable]
        private class ProfileData
        {
            public string name;
            public string avatarId;
            public int coins;
        }

        #endregion
    }
}