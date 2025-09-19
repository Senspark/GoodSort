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
    public enum ShelveType
    {
        Empty,
        Common
    }

    [Serializable]
    public class IconConfig
    {
        public int Id { get; }
    
        public string Icon { get; }
        [JsonConstructor]
        public IconConfig(int id, string icon)
        {
            Id = id;
            Icon = icon;
        }
    }
}