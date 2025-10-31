using Cysharp.Threading.Tasks;
using Defines;
using Engine.ShelfPuzzle;
using manager.Interface;
using UnityEngine;
using Utilities;

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
            return levelPrefab ? Object.Instantiate(levelPrefab) : null;
        }
        
        public ShelfPuzzleInputData[] GetInputData(int level)
        {
            var levelFile = Resources.Load<TextAsset>("InputLevel/lv" + level);
            var (inputData, _) = LevelFileParser.ParseLevelFile(levelFile.text);
            return inputData;
        }
    }
}