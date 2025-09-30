using System;
using Newtonsoft.Json;
using Strategy.Level;

namespace Defines
{
    [Serializable]
    public class LevelConfig
    {
        public LevelStrategy[] levelStrategies;

        [JsonConstructor]
        public LevelConfig(LevelStrategy[] data)
        {
            levelStrategies = data;
        }
    }

    [Serializable]
    public class GoodsConfig
    {
        public int Id { get; }
    
        public string Icon { get; }
        [JsonConstructor]
        public GoodsConfig(int id, string icon)
        {
            Id = id;
            Icon = icon;
        }
        
        // toString
        public override string ToString()
        {
            return $"GoodsConfig Id: {Id}, Icon: {Icon}";
        }
    }
}