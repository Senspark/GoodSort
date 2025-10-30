using System;
using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace manager
{
    public class DefaultScoreManager : IScoreManager
    {
        private const string SaveKey = nameof(DefaultScoreManager);
        private readonly IDataManager _dataManager;

        public UniTask<bool> Initialize()
        {
            LoadData();
            return UniTask.FromResult(true);
        }
        
        // constructor
        public DefaultScoreManager(IDataManager dataManager)
        {
            _dataManager = dataManager;
        }
        
        public int HighestLevelPlayed { get; private set; }
        public int LastLevelPlayed { get; private set; }
        public int AmountLevelPlayed { get; private set; }
        
        public void PlayedLevel(int level) {
            HighestLevelPlayed = Mathf.Max(HighestLevelPlayed, level);
            LastLevelPlayed = level;
            AmountLevelPlayed++;
            SaveData();
        }

        private void LoadData() {
            try {
                var raw = _dataManager.Get(SaveKey, "{}");
                var data = JsonUtility.FromJson<InternalData>(raw);
                HighestLevelPlayed = data.highestLevelPlayed;
                LastLevelPlayed = data.lastLevelPlayed;
                AmountLevelPlayed = data.amountLevelPlayed;
            }
            catch (Exception) {
                // ignore
            }
        }
        
        private void SaveData() {
            var d = new InternalData {
                highestLevelPlayed = HighestLevelPlayed,
                lastLevelPlayed = LastLevelPlayed,
                amountLevelPlayed = AmountLevelPlayed
            };
            var json = JsonUtility.ToJson(d);
            _dataManager.Set(SaveKey, json);
        }

        [Serializable]
        private class InternalData {
            public int highestLevelPlayed;
            public int lastLevelPlayed;
            public int amountLevelPlayed;
        }
    }
}