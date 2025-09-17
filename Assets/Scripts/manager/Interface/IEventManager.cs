using System;
using Constant;
using Senspark;

namespace manager.Interface
{
    [Service(typeof(IEventManager))]
    public interface IEventManager : IService
    {
        public void AddListener(EventKey key, Action callback);
        public void AddListener(EventKey key, Action<object> callback);
        public void RemoveListener(EventKey key, Action callback);
        public void RemoveListener(EventKey key, Action<object> callback);
        public void Invoke(EventKey key);
        public void Invoke(EventKey key, object param);
    }
}