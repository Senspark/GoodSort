using Cysharp.Threading.Tasks;
using Defines;
using manager.Interface;
using UnityEngine;

namespace manager
{
    public class DefaultLevelLoaderManager: ILevelLoaderManager
    {
        public UniTask<bool> Initialize()
        {
            return UniTask.FromResult(true);
        }
        
        public GameObject Load(int level)
        {
            var levelPrefab = Resources.Load<GameObject>("Level/LEVEL_" + level);
            return levelPrefab ? Object.Instantiate(levelPrefab) : // Return instantiated object
                null;
        }
    }
}