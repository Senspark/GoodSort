using System;
using Strategy.Level;

namespace Defines
{
    [Serializable]
    public class LevelConfig
    {
        public LevelStrategy[] LevelStrategies;
    }

    [Serializable]
    public class GoodsConfig
    {
        public int Id;
        public string Icon;

        // toString
        public override string ToString()
        {
            return $"GoodsConfig Id: {Id}, Icon: {Icon}";
        }
    }
}