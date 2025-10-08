using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using manager.Interface;

namespace manager
{
    public class DefaultConfigManager : IConfigManager
    {
        private readonly Dictionary<string, object> _data = new();

        public UniTask<bool> Initialize()
        {
            return UniTask.FromResult(true);
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
