using System.Collections;
using Cysharp.Threading.Tasks;
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

        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;

            var levelConfig = JsonConvert.DeserializeObject<PuzzleLevelConfig>(levelConfigDefault.text);
            var goodsConfig = JsonConvert.DeserializeObject<GoodsConfig[]>(iconConfigDefault.text);
            var initializeData = new ServiceInitializeData(levelConfig, goodsConfig);

            var initializer = new ServiceInitializer();
            _ = initializer.InitializeAllAsync(
                initializeData,
                (current, total) => { progress.fillAmount = (float)current / total; },
                () => { StartCoroutine(OnAppLoaded()); }
            );
        }

        private IEnumerator OnAppLoaded()
        {
            yield return new WaitForSeconds(1f);
            progress.fillAmount = 1f;
            DisappearLoadingBar();
        }

        // Disappear loading bar after 0.5 seconds, using tween to move down it
        private void DisappearLoadingBar()
        {
            loadingbar.DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    loadingbar.gameObject.SetActive(false);
                    GoToMenuScene();
                });
        }

        private void GoToMenuScene()
        {
            ServiceLocator.Instance
                .Resolve<ISceneLoader>()
                .LoadScene<MainMenu>(nameof(MainMenu))
                .Forget();
        }
    }
}