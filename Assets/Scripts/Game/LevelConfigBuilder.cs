using manager.Interface;
using UnityEngine;

namespace Game
{
    public class LevelConfigBuilder
    {
        private readonly ILevelLoaderManager _levelLoader;
        private int _levelId;
        private GameObject _levelObject;
        public GameObject LevelObject => _levelObject;
        
        public LevelConfigBuilder(ILevelLoaderManager levelLoader)
        {
            _levelLoader = levelLoader;
        }
        
        public LevelConfigBuilder SetLevel(int level)
        {
            _levelId = level;
            return this;
        }
        
        public LevelConfigBuilder Build()
        {
            _levelObject = _levelLoader.Load(_levelId);
            return this;
        }
    }
}