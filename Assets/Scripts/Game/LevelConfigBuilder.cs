using System.Threading.Tasks;
using Defines;
using manager.Interface;
using Strategy.Level;
using UnityEngine;

namespace Game
{
    public class LevelConfigBuilder
    {
        private readonly ILevelLoaderManager _levelLoader;
        private LevelStrategy _levelStrategy;
        private GameObject _levelObject;
        
        public LevelConfigBuilder(ILevelLoaderManager levelLoader)
        {
            _levelLoader = levelLoader;
        }
        
        public GameObject LevelObject => _levelObject;
        
        public LevelConfigBuilder SetLevelStrategy(LevelStrategy levelStrategy)
        {
            _levelStrategy = levelStrategy;
            return this;
        }
        
        
        public LevelConfigBuilder Build()
        {
            _levelObject = _levelLoader.Create(_levelStrategy.Id);
            return this;
        }
    }
}