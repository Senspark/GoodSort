using System.Threading.Tasks;
using Senspark;
using UnityEngine;

namespace manager.Interface
{
    [Service(nameof(ISceneLoader))]
    public interface ISceneLoader : IService
    { 
        public Task<T> LoadScene<T>(string sceneName) where T : MonoBehaviour;
    }
}