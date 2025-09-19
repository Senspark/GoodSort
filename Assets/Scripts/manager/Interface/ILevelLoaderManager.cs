using Senspark;
using UnityEngine;

namespace manager.Interface
{
    [Service(nameof(ILevelLoaderManager))]
    public interface ILevelLoaderManager : IService
    {
        public GameObject Create(int level);
    }
}