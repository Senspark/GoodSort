using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;
using Newtonsoft.Json;


namespace manager
{
    public class DefaultDataManager : IDataManager
    {
        private readonly IDataStorage _storage;

        public DefaultDataManager(IDataStorage storage)
        {
            _storage = storage;
        }

        public UniTask<bool> Initialize()
        {
            // clear all data
            return UniTask.FromResult(true);
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)_storage.GetInt(key, (int)(object)defaultValue);
            }
            if (typeof(T) == typeof(float))
            {
                return (T)(object)_storage.GetFloat(key, (float)(object)defaultValue);
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)_storage.GetString(key, (string)(object)defaultValue);
            }

            var str = _storage.GetString(key, string.Empty);
            return string.IsNullOrEmpty(str) ? defaultValue : JsonConvert.DeserializeObject<T>(str);
        }

        public void Set<T>(string key, T value)
        {
            if (value is int i)
            {
                _storage.SetInt(key, i);
                return;
            }
            if (value is float f)
            {
                _storage.SetFloat(key, f);
                return;
            }
            if (value is string s)
            {
                _storage.SetString(key, s);
                return;
            }

            var str = JsonConvert.SerializeObject(value);
            _storage.SetString(key, str);
        }

        Task IDataManager.Initialize()
        {
            return Task.FromResult(true);
        }
    }
}