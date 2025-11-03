using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utilities
{
    public class PrefabUtils
    {
        public static async UniTask<T> CreatePrefab<T>(string path) where T : MonoBehaviour
        {
            var prefab = await Resources.LoadAsync<GameObject>(path);
            var instance = Object.Instantiate(prefab as GameObject);
            return instance.GetComponent<T>();
        }
        
        public static async UniTask<GameObject> LoadPrefab(string path)
        {
            var prefab = await Resources.LoadAsync<GameObject>(path);
            var instance = Object.Instantiate(prefab as GameObject);
            return instance;
        }

    }
}