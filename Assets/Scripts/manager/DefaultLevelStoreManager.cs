using System.Threading.Tasks;
using manager.Interface;

namespace manager
{
    public class DefaultLevelStoreManager : ILevelStoreManager
    {
        public Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }
        
        // constructor
        public DefaultLevelStoreManager(IDataManager dataManager)
        {
        }
    }
}