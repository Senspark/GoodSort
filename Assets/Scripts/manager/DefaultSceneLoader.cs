using System.Threading.Tasks;
using manager.Interface;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace manager
{
    public class DefaultSceneLoader : ISceneLoader
    {
        public Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }
        
        public async Task<T> LoadScene<T>(string sceneName) where T : MonoBehaviour
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            var item = Object.FindFirstObjectByType<T>();
            Assert.IsTrue(item != null);
            return item;
        }
    }
}