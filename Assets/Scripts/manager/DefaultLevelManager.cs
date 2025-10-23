using Cysharp.Threading.Tasks;
using manager.Interface;

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
            _dataManager.SetInt("current_level", level);
        }
        
        public int GetCurrentLevel()
        {
            return _dataManager.GetInt("current_level", 1);
        }
        
        public void GoToNextLevel()
        {
            SetCurrentLevel(CurrentLevel + 1);
        }
    }
}