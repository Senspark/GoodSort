using System;
using Senspark;

namespace manager.Interface
{
    [Service(typeof(ILivesManager))]
    public interface ILivesManager : IService
    {
        bool IsLiveActive();
        TimeSpan GetLiveTimeLeft();
        void RefreshLive();
        void CheckAndRestoreLives();
    }
}