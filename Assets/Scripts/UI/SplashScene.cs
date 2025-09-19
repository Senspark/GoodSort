using System.Collections;
using Defines;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Senspark;
using manager;
using manager.Interface;
using Newtonsoft.Json;
using UI;
using Utilities;

namespace Game.UI
{
    public class SplashScene : MonoBehaviour
    {
        [SerializeField] private Transform loadingbar;
        [SerializeField] private Image progress;
        [SerializeField] private TextAsset levelConfigDefault;
        [SerializeField] private TextAsset iconConfigDefault;

        private bool isLoaded;

        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;

            var initializer = new ServiceInitializer();
            initializer.OnProgress += (current, total) =>
            {
                progress.fillAmount = (float)current / total;
            };
            initializer.OnCompleted += () =>
            {
                isLoaded = true;
                StartCoroutine(OnAppLoaded());
            };
            InitializeConfigManager();
            _ = initializer.InitializeAllAsync();
        }

        private void InitializeConfigManager()
        {
            var configManager = new DefaultConfigManager();
            configManager.SetDefaultValue(ConfigKey.LevelConfig, levelConfigDefault.text);
            configManager.SetDefaultValue(ConfigKey.IconConfig, iconConfigDefault.text);
            ServiceLocator.Instance.Provide(configManager);
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
                    Debug.Log("Do Something");
                });
        }
    }
}