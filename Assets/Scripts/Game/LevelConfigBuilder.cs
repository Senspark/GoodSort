using System.Collections.Generic;
using Defines;
using manager.Interface;
using Strategy.Level;
using UnityEngine;
using Utilities;

namespace Game
{
    public class LevelConfigBuilder
    {
        private readonly ILevelLoaderManager _levelLoader;
        private LevelStrategy _levelStrategy;
        private List<GoodsConfig> _goodsArray;
        private GameObject _levelObject;
        
        public LevelConfigBuilder(ILevelLoaderManager levelLoader)
        {
            _levelLoader = levelLoader;
        }
        
        public GameObject LevelObject => _levelObject;
        public LevelStrategy LevelStrategy => _levelStrategy;
        public List<GoodsConfig> GoodsArray => _goodsArray;
        
        public LevelConfigBuilder SetLevelStrategy(LevelStrategy levelStrategy)
        {
            _levelStrategy = levelStrategy;
            return this;
        }
        
        public LevelConfigBuilder SetGoodsArray(List<GoodsConfig> goodsArray)
        {
            var sourceCnt = _levelStrategy.Group;
            _goodsArray = ArrayUtils.GenerateRandomList(goodsArray, sourceCnt);
            return this;
        }
        
        public LevelConfigBuilder Build()
        {
            _levelObject = _levelLoader.Create(_levelStrategy.Id);
            return this;
        }
    }
}