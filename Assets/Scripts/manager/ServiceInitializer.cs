using System;
using Cysharp.Threading.Tasks;
using Defines;
using Factory;
using JetBrains.Annotations;
using manager.Interface;
using Senspark;
using IAudioManager = manager.Interface.IAudioManager;
using IDataManager = manager.Interface.IDataManager;

namespace manager
{
    public interface IServiceDeclaration
    {
        public IDataManager DataManager { get; set; }
        public IAudioManager AudioManager { get; set; }
        public ILevelStoreManager LevelStoreManager { get; set; }
        public ISceneLoader SceneLoader { get; set; }
        public IScoreManager ScoreManager { get; set; }
        public IEventManager EventManager { get; set; }
        public ILevelLoaderManager LevelLoaderManager { get; set; }
    }

    public class ServiceInitializer : IServiceDeclaration
    {
        private int TotalSteps { get; set; }
        private int CurrentStep { get; set; }

        #region MyRegion

        public IDataManager DataManager { get; set; }
        public IAudioManager AudioManager { get; set; }
        public ILevelStoreManager LevelStoreManager { get; set; }
        public ISceneLoader SceneLoader { get; set; }
        public IScoreManager ScoreManager { get; set; }
        public IEventManager EventManager { get; set; }
        public ILevelLoaderManager LevelLoaderManager { get; set; }

        #endregion

        public async UniTask InitializeAllAsync(
            ServiceInitializeData data,
            [CanBeNull] Action<int, int> onProgress,
            [CanBeNull] Action onCompleted
        )
        {
            DataManager = new DefaultDataManager(new LocalDataStorage());
            AudioManager = new DefaultAudioManager(DataManager);
            LevelStoreManager = new DefaultLevelStoreManager(DataManager);
            SceneLoader = new DefaultSceneLoader();
            ScoreManager = new DefaultScoreManager(DataManager);
            EventManager = new EventManager();
            LevelLoaderManager = new DefaultLevelLoaderManager();

            var configManager = new DefaultConfigManager();
            configManager.SetDefaultValue(ConfigKey.LevelConfig, data.LevelConfig);
            configManager.SetDefaultValue(ConfigKey.GoodsConfig, data.GoodsConfig);

            var services = new IService[]
            {
                DataManager,
                AudioManager,
                LevelStoreManager,
                SceneLoader,
                ScoreManager,
                EventManager,
                LevelLoaderManager,
                configManager
            };
            TotalSteps = services.Length;
            CurrentStep = 0;
            foreach (var service in services)
            {
                CurrentStep++;
                await service.Initialize();
                ServiceLocator.Instance.Provide(service);
                onProgress?.Invoke(CurrentStep, TotalSteps);
            }

            onCompleted?.Invoke();

            // Initialize factory
            var factoryInitializer = new FactoryInitializer();
            factoryInitializer.Initialize(this);
        }
    }
}

public class ServiceInitializeData
{
    public readonly PuzzleLevelConfig LevelConfig;
    public readonly GoodsConfig[] GoodsConfig;

    public ServiceInitializeData(PuzzleLevelConfig levelConfig, GoodsConfig[] goodsConfig)
    {
        LevelConfig = levelConfig;
        GoodsConfig = goodsConfig;
    }
}