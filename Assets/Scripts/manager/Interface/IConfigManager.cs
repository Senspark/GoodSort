using System.Collections.Generic;
using Defines;
using Senspark;

namespace manager.Interface
{
    public static class ConfigKey
    {
        public const string LevelConfig = "puzzle_level_config";
        public const string GoodsConfig = "goods_config";
    }
    
    [Service(nameof(IConfigManager))]
    public interface IConfigManager : IService
    {
        public void SetDefaultValue<T>(string key, T value);
        public T GetValue<T>(string key);
        public int GetTimeStamp();
    }
}