using Cysharp.Threading.Tasks;
using manager.Interface;

namespace manager
{
    public class DefaultLevelStoreManager : ILevelStoreManager
    {
        public UniTask<bool> Initialize()
        {
            return UniTask.FromResult(true);
        }

        // constructor
        public DefaultLevelStoreManager(IDataManager dataManager)
        {
        }
    }
}