using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace manager
{
    public class DefaultDateTimeManager : IDateTimeManager
    {
        private readonly IDataManager _dataManager;
        private readonly InternalData _internalData;
        private List<Action> _newDayAction = new();
        private const string Key = nameof(DefaultDateTimeManager);
        private const string NewDayKey = "ndk";

        public DefaultDateTimeManager(IDataManager dataManager)
        {
            _dataManager = dataManager;
            _internalData = _dataManager.Get(Key, new InternalData());
        }
        
        public UniTask<bool> Initialize()
        {
            return UniTask.FromResult(true);
        }
        
        public void SetCountdownFromNow(string id, TimeSpan timeSpan)
        {
            var epoch = TimeUtils.ConvertDateTimeToEpochSeconds(DateTime.Now.Add(timeSpan));
            SetDataToId(id, epoch);
        }
        
        public void AppendCountdown(string id, TimeSpan timeSpan)
        {
            var epoch = GetEpochFromId(id);
            if (epoch > 0) {
                var past = TimeUtils.ConvertEpochSecondsToLocalDateTime(epoch);
                var newTime = past.Add(timeSpan);
                var newEpoch = TimeUtils.ConvertDateTimeToEpochSeconds(newTime);
                SetDataToId(id, newEpoch);
            }
            else {
                SetCountdownFromNow(id, timeSpan);
            }
        }
        
        public TimeSpan GetCountDownTimeLeft(string id) {
            var epoch = GetEpochFromId(id);
            if (epoch == 0) {
                return TimeSpan.Zero;
            }

            var value = TimeUtils.ConvertEpochSecondsToLocalDateTime(epoch);
            return value - DateTime.Now;
        }
        
        public bool IsTimeOut(string id) {
            var epoch = GetEpochFromId(id);
            if (epoch == 0) {
                Debug.LogWarning($"mylog: not contain id {id}");
                return true;
            }

            var endTime = TimeUtils.ConvertEpochSecondsToLocalDateTime(epoch);
            var now = DateTime.Now;
            var time = endTime - now;
            return time.TotalSeconds <= 0;
        }
        
        public bool IsExist(string id) {
            return _internalData.Data.ContainsKey(id);
        }
        
        public void Delete(string id) {
            if (IsExist(id)) {
                _internalData.Data.Remove(id);
            }
        }
        
        public void OnNewDayRegister(Action action) {
            if (action != null && !_newDayAction .Contains(action)) {
                _newDayAction.Add(action);
            }
        }
        
        public void CheckNewDay() {
            if (IsTimeOut(NewDayKey)) {
                foreach (var act in _newDayAction) {
                    act?.Invoke();
                }

                var now = DateTime.Now;
                var endOfDay = now.Date.AddDays(1).AddSeconds(-1); // 23:59:59 hôm nay
                var remaining = endOfDay - now;
                SetCountdownFromNow(NewDayKey, remaining);
            }
        }
        
        private long GetEpochFromId(string id) {
            return _internalData.Data.GetValueOrDefault(id, 0);
        }

        private void SetDataToId(string id, long epoch) {
            _internalData.Data[id] = epoch;
            _dataManager.Set(Key, _internalData);
        }

        [SerializeField]
        private class InternalData
        {
            public Dictionary<string, long> Data = new();
        }
    }
}