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
using IAudioManager = manager.Interface.IAudioManager;
using IDataManager = manager.Interface.IDataManager;

namespace Game.UI
{
    public class SplashScene : MonoBehaviour
    {
        [SerializeField] private Transform loadingbar;
        [SerializeField] private Image progress;

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
            _ = initializer.InitializeAllAsync();
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
    }
}