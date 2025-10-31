using Engine.ShelfPuzzle;
using Senspark;
using UnityEngine;

namespace manager.Interface
{
    [Service(nameof(ILevelLoaderManager))]
    public interface ILevelLoaderManager : IService
    {
        public GameObject Load(int level);
        public ShelfPuzzleInputData[] GetInputData(int level);
    }
}