using System.Collections;
using Cysharp.Threading.Tasks;
using Defines;
using manager;
using manager.Interface;
using Newtonsoft.Json;
using Senspark;
using Tutorial;
using UnityEngine;

namespace UI
{
    public class SplashScene : MonoBehaviour
    {
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
                (_, _) => {},
                () => { StartCoroutine(OnAppLoaded()); }
            );
        }

        private IEnumerator OnAppLoaded()
        {
            yield return new WaitForSeconds(1f);
            ServiceLocator.Instance.Resolve<IAudioManager>().PlayMusic(AudioEnum.MenuMusic);
            // Nếu current level là 1 và chưa finish tutorial là chuyển sang tutorial scene
            if (ServiceLocator.Instance.Resolve<ILevelManager>().GetCurrentLevel() == 1 &&
                !ServiceLocator.Instance.Resolve<ITutorialManager>().IsTutorialFinished(TutorialType.Onboarding))
            {
                GoToTutorialScene();
            }
            else
            {
                GoToMenuScene();
            }
        }

        private void GoToMenuScene()
        {
            ServiceLocator.Instance
                .Resolve<ISceneLoader>()
                .LoadScene<MainMenu>(nameof(MainMenu))
                .Forget();
        }
        
        private void GoToTutorialScene()
        {
            ServiceLocator.Instance
                .Resolve<ISceneLoader>()
                .LoadScene<TutorialGameScene>("Tutorial")
                .Forget();
        }
    }
}