using System;
using Cysharp.Threading.Tasks;
using manager.Interface;

namespace manager
{
    public class DefaultLivesManager : ILivesManager
    {
        private const string Key = nameof(DefaultLivesManager);
        private const int LiveHours = 24;
        
        private readonly IDateTimeManager _dateTimeManager;

        public DefaultLivesManager(IDateTimeManager dateTimeManager)
        {
            _dateTimeManager = dateTimeManager;
        }

        public UniTask<bool> Initialize()
        {
            // Mở game lần đầu hoặc hết thời gian -> reset 24h
            if (!_dateTimeManager.IsExist(Key) || _dateTimeManager.IsTimeOut(Key))
            {
                RefreshLive();
            }
            return UniTask.FromResult(true);
        }

        public bool IsLiveActive()
        {
            return !_dateTimeManager.IsTimeOut(Key);
        }

        public TimeSpan GetLiveTimeLeft()
        {
            return _dateTimeManager.GetCountDownTimeLeft(Key);
        }

        public void RefreshLive()
        {
            _dateTimeManager.SetCountdownFromNow(Key, TimeSpan.FromHours(LiveHours));
        }

        public void CheckAndRefreshIfNeeded()
        {
            if (_dateTimeManager.IsTimeOut(Key))
            {
                RefreshLive();
            }
        }
    }
}