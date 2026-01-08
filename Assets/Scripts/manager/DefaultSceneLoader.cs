using System.Reflection;
using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace manager
{
    public class DefaultSceneLoader : ISceneLoader
    {
        private readonly IAudioManager _audioManager;
        public DefaultSceneLoader(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        
        public UniTask<bool> Initialize()
        {
            return UniTask.FromResult(true);
        }

        public async UniTask<T> LoadScene<T>(string sceneName) where T : MonoBehaviour
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            var item = Object.FindFirstObjectByType<T>();
            Assert.IsTrue(item != null);
            return item;
        }
        
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
    }
}