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
            var levelPrefab = Resources.Load<GameObject>("Level/LEVEL_" + level);
            return levelPrefab != null ? Object.Instantiate(levelPrefab) : // Return instantiated object
                null;
        }
    }
}