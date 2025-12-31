using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Defines;
using Factory;
using JetBrains.Annotations;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace manager
{
    public interface IServiceDeclaration
    {
        public IDataManager DataManager { get; set; }
        public IAudioManager AudioManager { get; set; }
        public ILevelManager LevelManager { get; set; }
        public ISceneLoader SceneLoader { get; set; }
        public IEventManager EventManager { get; set; }
        public ILevelLoaderManager LevelLoaderManager { get; set; }
        public IProfileManager ProfileManager { get; set; }
        public IStoreManager StoreManager { get; set; }
    }

    public class ServiceInitializer : IServiceDeclaration
    {
        private int TotalSteps { get; set; }
        private int CurrentStep { get; set; }

        #region MyRegion

        public IDataManager DataManager { get; set; }
        public IAudioManager AudioManager { get; set; }
        public ILevelManager LevelManager { get; set; }
        public ISceneLoader SceneLoader { get; set; }
        public IEventManager EventManager { get; set; }
        public ILevelLoaderManager LevelLoaderManager { get; set; }
        public IProfileManager ProfileManager { get; set; }
        public IStoreManager StoreManager { get; set; }

        #endregion

        public async UniTask InitializeAllAsync(
            ServiceInitializeData data,
            [CanBeNull] Action<int, int> onProgress,
            [CanBeNull] Action onCompleted
        )
        {
            DataManager = new DefaultDataManager(new LocalDataStorage());

            // Build audio infos from Resources/Audio
            var infos = new Dictionary<Enum, IAudioInfo>
            {
                { AudioEnum.MenuMusic, new AudioInfo("Audio/menu_bg_music", 1f) },
                { AudioEnum.GameMusic, new AudioInfo("Audio/gameplay_bg_music", 1f) },
                { AudioEnum.ClickButton, new AudioInfo("Audio/Pop Up 3", 1f) },
                { AudioEnum.CloseDialog, new AudioInfo("Audio/Pop Up 1", 1f) },
                { AudioEnum.CoinFly, new AudioInfo("Audio/pick-coin", 1f) },
                { AudioEnum.PutGoods, new AudioInfo("Audio/Pop 06", 1f) },
                { AudioEnum.Match, new AudioInfo("Audio/Score Multi", 1f) },
                { AudioEnum.LevelComplete, new AudioInfo("Audio/Level Up", 1f) },
                { AudioEnum.ClaimComplete, new AudioInfo("Audio/Collect Item", 1f) },
            };
            AudioManager = new AudioManager(DataManager, "AudioManager", infos);

            LevelManager = new DefaultLevelManager(DataManager);
            SceneLoader = new DefaultSceneLoader(AudioManager);
            EventManager = new EventManager();
            LevelLoaderManager = new DefaultLevelLoaderManager();
            ProfileManager = new DefaultProfileManager(DataManager);
            StoreManager = new DefaultStoreManager(DataManager);

            var configManager = new DefaultConfigManager();
            configManager.SetDefaultValue(ConfigKey.LevelConfig, data.LevelConfig);
            configManager.SetDefaultValue(ConfigKey.GoodsConfig, data.GoodsConfig);

            await DataManager.Initialize();
            ServiceLocator.Instance.Provide(DataManager);

            await AudioManager.Initialize();
            ServiceLocator.Instance.Provide(AudioManager);
            
            var services = new IService[]
            {
                LevelManager,
                SceneLoader,
                EventManager,
                LevelLoaderManager,
                ProfileManager,
                StoreManager,
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