using System.Threading.Tasks;
using manager.Interface;

namespace manager
{
    public class DefaultScoreManager : IScoreManager
    {
        public Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }
        
        // constructor
        public DefaultScoreManager(IDataManager dataManager)
        {
        }
    }
}