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
            HandleSceneMusic<T>();
            Assert.IsTrue(item != null);
            return item;
        }
        
        private void HandleSceneMusic<T>()
        {
            // Lấy attribute từ class T
            var musicAttr = typeof(T).GetCustomAttribute<SceneMusicAttribute>();
        
            if (musicAttr != null)
            {
                _audioManager.PlayMusic(musicAttr.Music);
            }
        }
    }
}