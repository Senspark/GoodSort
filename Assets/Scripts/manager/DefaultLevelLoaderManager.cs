using System.Threading.Tasks;
using Defines;
using manager.Interface;
using UnityEngine;

namespace manager
{
    public class DefaultLevelLoaderManager: ILevelLoaderManager
    {
        public Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }
        
        public GameObject Create(int level)
        {
            // TODO: Dynamite load level bằng cách hoành tráng hơn
            var levelPrefab = Resources.Load<GameObject>("Level/LEVEL_" + level);
            return levelPrefab;
        }
    }
}