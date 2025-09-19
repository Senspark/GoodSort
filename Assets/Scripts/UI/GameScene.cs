using System.Threading.Tasks;
using Defines;
using Game;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace UI
{
    public class GameScene : MonoBehaviour
    {
        private readonly IEventManager _eventManager = ServiceLocator.Instance.Resolve<IEventManager>();
        private readonly ILevelLoaderManager _levelLoaderManager = ServiceLocator.Instance.Resolve<ILevelLoaderManager>();
        private readonly ILevelStoreManager _levelStoreManager = ServiceLocator.Instance.Resolve<ILevelStoreManager>();
        private readonly IConfigManager _configManager = ServiceLocator.Instance.Resolve<IConfigManager>();
        
        public void LoadLevel(int level)
        {
            var strategy = _configManager.GetValue<LevelConfig>(ConfigKey.LevelConfig).levelStrategies[level - 1];
            var builder = new LevelConfigBuilder(_levelLoaderManager)
                .SetLevelStrategy(strategy)
                .Build();

            var leveView = builder.LevelObject.GetComponent<LevelView>();
            leveView.Load(builder, _eventManager);
            leveView.transform.SetParent(transform, false);
        }
    }
}