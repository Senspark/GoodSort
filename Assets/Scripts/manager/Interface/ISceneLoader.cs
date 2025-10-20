using Cysharp.Threading.Tasks;
using Senspark;
using UnityEngine;

namespace manager.Interface
{
    [Service(nameof(ISceneLoader))]
    public interface ISceneLoader : IService
    {
        public UniTask<T> LoadScene<T>(string sceneName) where T : MonoBehaviour;
    }
}