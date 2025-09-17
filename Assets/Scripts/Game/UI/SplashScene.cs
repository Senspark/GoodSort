using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Senspark;
using Senspark.Internal;
using manager;
using manager.Interface;
using UI;
using Utilities;

namespace Game.UI
{
    public class SplashScene : MonoBehaviour
    {
        private static int _totalInitializationStep;
        private static int _currentInitializationStep;

        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private Transform loadingbar;
        [SerializeField] private Image progress;
        [SerializeField] private TextAsset remoteConfigText;

        private Dictionary<string, object> _defaultRemoteConfig;
        private bool isLoaded;

        private void Awake()
        {
            _currentInitializationStep = 0;
            _defaultRemoteConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(remoteConfigText.text);
        }

        private async void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;
            
            await ServiceSDKInitialize();
            await ServiceInitialize();
            
            // Initialize thêm mấy cái khác nữa nhưng bây giờ không cần
            StartCoroutine(OnAppLoaded());
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
            var services = new IService[]
            {
                dataManager,
                audioManager,
                levelStoreManager,
                sceneLoader,
                scoreManager,
                eventManager
            };
            _totalInitializationStep = services.Length;
            foreach (var service in services)
            {
                _currentInitializationStep++;
                await service.Initialize();
                ServiceLocator.Instance.Provide(service);
            }
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

        private IEnumerator OnAppLoaded()
        {
            yield return new WaitForSeconds(1f);
            progress.fillAmount = 1f;
            isLoaded = true;
            DisappearLoadingBar();
        }
        
        // Disappear loading bar after 0.5 seconds, using tween to move down it
        private void DisappearLoadingBar()
        {
            loadingbar.DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() => {
                    loadingbar.gameObject.SetActive(false);
                    GoToMenuScene();
                });
        }

        private void GoToMenuScene()
        {
            ServiceLocator.Instance
                .Resolve<ISceneLoader>()
                .LoadScene<MainMenu>(nameof(MainMenu))
                .Then(mainMenu =>
                {
                    mainMenu.OnStartButtonPressed();
                });
        }

        private void Update()
        {
            if(!isLoaded) {
                progress.fillAmount = (float)_currentInitializationStep / _totalInitializationStep;
            }
        }
    }
}