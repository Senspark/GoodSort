using System;
using System.Threading.Tasks;
using Factory;
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


        public event Action<int, int> OnProgress;
        public event Action OnCompleted;

        #region MyRegion
        public IDataManager DataManager { get; set; }
        public IAudioManager AudioManager { get; set; }
        public ILevelStoreManager LevelStoreManager { get; set; }
        public ISceneLoader SceneLoader { get; set; }
        public IScoreManager ScoreManager { get; set; }
        public IEventManager EventManager { get; set; }
        public ILevelLoaderManager LevelLoaderManager { get; set; }

        #endregion


        public async Task InitializeAllAsync()
        {
            DataManager = new DefaultDataManager(new LocalDataStorage());
            AudioManager = new DefaultAudioManager(DataManager);
            LevelStoreManager = new DefaultLevelStoreManager(DataManager);
            SceneLoader = new DefaultSceneLoader();
            ScoreManager = new DefaultScoreManager(DataManager);
            EventManager = new EventManager();
            LevelLoaderManager = new DefaultLevelLoaderManager();
            var services = new IService[]
            {
                DataManager,
                AudioManager,
                LevelStoreManager,
                SceneLoader,
                ScoreManager,
                EventManager,
                LevelLoaderManager
            };
            TotalSteps = services.Length;
            CurrentStep = 0;
            foreach (var service in services)
            {
                CurrentStep++;
                await service.Initialize();
                ServiceLocator.Instance.Provide(service);
                OnProgress?.Invoke(CurrentStep, TotalSteps);
            }
            OnCompleted?.Invoke();
            
            // Initialize factory
            var factoryInitializer = new FactoryInitializer();
            factoryInitializer.Initialize(this);
        }
    }
}