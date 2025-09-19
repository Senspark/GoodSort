using System.Collections.Generic;
using System.Threading.Tasks;
using manager.Interface;

namespace manager
{
    public class DefaultConfigManager : IConfigManager
    {
        private readonly Dictionary<string, object> _data = new();

        public Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }

        public void SetDefaultValue<T>(string key, T value)
        {
            _data[key] = value;
        }

        public T GetValue<T>(string key)
        {
            return _data.TryGetValue(key, out var value) ? (T)value : default;
        }

        public int GetTimeStamp()
        {
            return 0;
        }
    }
}
