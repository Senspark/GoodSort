using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Constant;
using manager.Interface;

namespace manager
{
    public class EventManager : IEventManager
    {
        private Dictionary<EventKey,Action> eventTable = new();
        private Dictionary<EventKey, Action<object>> eventTableWithParam = new();
        
        public Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }
        
        
        public void AddListener(EventKey key, Action callback)
        {
            On(key, callback);
        }
        
        public void AddListener(EventKey key, Action<object> callback)
        {
            On(key, callback);
        }
        
        public void RemoveListener(EventKey key, Action callback)
        {
            Off(key, callback);
        }
        
        public void RemoveListener(EventKey key, Action<object> callback)
        {
            Off(key, callback);
        }
        
        public void Invoke(EventKey key)
        {
            Emit(key);
        }
        
        public void Invoke(EventKey key, object param)
        {
            Emit(key, param);
        }
        
        private void On(EventKey key, Action callback)
        {
            if (!eventTable.ContainsKey(key))
                eventTable[key] = () => { };

            eventTable[key] += callback;
        }

        private void On(EventKey key, Action<object> callback)
        {
            if (!eventTableWithParam.ContainsKey(key))
                eventTableWithParam[key] = obj => { };

            eventTableWithParam[key] += callback;
        }

        private void Off(EventKey key, Action callback)
        {
            if (eventTable.ContainsKey(key))
                eventTable[key] -= callback;
        }

        private void Off(EventKey key, Action<object> callback)
        {
            if (eventTableWithParam.ContainsKey(key))
                eventTableWithParam[key] -= callback;
        }

        private void Emit(EventKey key)
        {
            if (eventTable.ContainsKey(key))
                eventTable[key]?.Invoke();
        }

        private void Emit(EventKey key, object param)
        {
            if (eventTableWithParam.ContainsKey(key))
                eventTableWithParam[key]?.Invoke(param);
        }
    }
}