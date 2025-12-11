using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace manager
{
    public class DefaultLevelManager : ILevelManager
    {
        public int CurrentLevel { get; private set; }
        private readonly IDataManager _dataManager;
        
        public DefaultLevelManager(IDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public UniTask<bool> Initialize()
        {
            CurrentLevel = 1;
            return UniTask.FromResult(true);
        }

        public void SetCurrentLevel(int level)
        {
            CurrentLevel = level;
            _dataManager.Set("CurrentLevel", level);
        }
        
        public int GetCurrentLevel()
        {
            return Mathf.Min(_dataManager.Get("CurrentLevel", 1), 43);
        }
        
        public void GoToNextLevel()
        {
            SetCurrentLevel(CurrentLevel + 1);
        }
    }
}