using System.Threading.Tasks;
using manager;
using Senspark;

namespace App
{
    public static class ServiceUtils
    {
        private static bool _initializedBaseServices;

        public static async Task InitializeBaseServices()
        {
            if (_initializedBaseServices)
            {
                return;
            }
            
            _initializedBaseServices = true;
            var dataManager = new DefaultDataManager(new LocalDataStorage());
            
            await logManager.Initialize();
            await sceneManager.Initialize();
            await dialogManager.Initialize();
            await languageManager.Initialize();
            await remoteConfigManager.Initialize();
            await levelManager.Initialize();
            
            ServiceLocator.Instance.Provide(logManager);
            ServiceLocator.Instance.Provide(sceneManager);
            ServiceLocator.Instance.Provide(dialogManager);
            ServiceLocator.Instance.Provide(languageManager);
            ServiceLocator.Instance.Provide(remoteConfigManager);
            ServiceLocator.Instance.Provide(levelManager);
        }
    }
}