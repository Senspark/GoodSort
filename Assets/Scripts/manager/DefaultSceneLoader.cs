using Cysharp.Threading.Tasks;
using manager.Interface;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace manager
{
    public class DefaultSceneLoader : ISceneLoader
    {
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
    }
}