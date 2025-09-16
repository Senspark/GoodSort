using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Senspark;
using Senspark.Internal;
using manager;

namespace Game.UI
{
    public class SplashScene : MonoBehaviour
    {
        private static int _totalInitializationStep;
        private static int _currentInitializationStep;

        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private Transform loadingbar;
        [SerializeField] private Text loadText;
        [SerializeField] private Image progress;
        [SerializeField] private Text versionText;
        [SerializeField] private TextAsset remoteConfigText;

        private Dictionary<string, object> _defaultRemoteConfig;
        private bool isLoaded;

        private void Awake()
        {
            _defaultRemoteConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(remoteConfigText.text);
        }

        private async void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;
            versionText.text = $"Version {Application.version}";
            
            await ServiceSDKInitialize();
            await ServiceInitialize();
        }

        private async Task ServiceSDKInitialize()
        {
            await InitRemoteConfig();
            ServiceLocator.Instance.Resolve<BaseRemoteConfigManager>();
        }
        
        private async Task ServiceInitialize()
        {
            var dataManager = new DefaultDataManager(new LocalDataStorage());
            var audioManager = new DefaultAudioManager(dataManager);
            var levelStoreManager = new DefaultLevelStoreManager(dataManager);
            var sceneLoader = new DefaultSceneLoader();
            var scoreManager = new DefaultScoreManager(dataManager);
            var eventManager = new EventManager();

        }
        
        private async UniTask InitRemoteConfig() {
            var builder = new LocalRemoteConfigManager.Builder
            {
                Defaults = _defaultRemoteConfig
            };
            var remoteConfig = builder.Build();
            await remoteConfig.Initialize(2);
            ServiceLocator.Instance.Provide(remoteConfig);
        }
    }
}