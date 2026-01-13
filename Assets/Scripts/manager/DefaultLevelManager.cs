using Constant;
using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace manager
{
    public class DefaultLevelManager : ILevelManager
    {
        private int CurrentLevel { get; set; }
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
            var currentLevel = _dataManager.Get("CurrentLevel", 1);
            if (currentLevel < 1)
            {
                currentLevel = 1;
            }
            else if (currentLevel > Globals.MaxLevel)
            {
                currentLevel = Globals.MaxLevel;
            }

            return currentLevel;
        }
        
        public void GoToNextLevel()
        {
            CurrentLevel = GetCurrentLevel() + 1;
            SetCurrentLevel(CurrentLevel);
        } 
    }
}