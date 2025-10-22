using Cysharp.Threading.Tasks;
using manager.Interface;

namespace manager
{
    public class DefaultLevelManager : ILevelManager
    {
        public int CurrentLevel { get; private set; }

        public UniTask<bool> Initialize()
        {
            return UniTask.FromResult(true);
        }

        public void SetCurrentLevel(int level)
        {
            CurrentLevel = level;
        }
        
        public int GetCurrentLevel()
        {
            return 3;
        }
    }
}