using UnityEngine;
using System.Collections.Generic;
using Defines;
using System;

namespace manager
{
    public class EventManager : MonoBehaviour
    {
        private static EventManager _instance;
        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<EventManager>();
                }
                return _instance;
            }
        }

        private Dictionary<EventKey,Action> eventTable = new Dictionary<EventKey, Action>();

        private Dictionary<EventKey, Action<object>> eventTableWithParam = new Dictionary<EventKey, Action<object>>();

        private void Awake()
        {
            _instance = this;
        }

        public void On(EventKey key, Action callback)
        {
            if (!eventTable.ContainsKey(key))
                eventTable[key] = () => { };

            eventTable[key] += callback;
        }

        public void On(EventKey key, Action<object> callback)
        {
            if (!eventTableWithParam.ContainsKey(key))
                eventTableWithParam[key] = obj => { };

            eventTableWithParam[key] += callback;
        }

        public void Off(EventKey key, Action callback)
        {
            if (eventTable.ContainsKey(key))
                eventTable[key] -= callback;
        }

        public void Off(EventKey key, Action<object> callback)
        {
            if (eventTableWithParam.ContainsKey(key))
                eventTableWithParam[key] -= callback;
        }

        public void Emit(EventKey key)
        {
            if (eventTable.ContainsKey(key))
                eventTable[key]?.Invoke();
        }

        public void Emit(EventKey key, object param)
        {
            if (eventTableWithParam.ContainsKey(key))
                eventTableWithParam[key]?.Invoke(param);
        }
    }
}