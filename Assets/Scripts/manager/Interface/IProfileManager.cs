using System;
using Senspark;

namespace manager.Interface
{
    public class ProfileManagerObserver
    {
        public Action<string> OnNameChanged { get; set; }
        public Action<string> OnAvatarIdChanged { get; set; }
        public Action<int> OnLivesChanged { get; set; }
    }

    [Service(typeof(IProfileManager))]
    public interface IProfileManager : IObserverManager<ProfileManagerObserver>, IService
    {
        void SetName(string name);
        void SetAvatarId(string avatarId);
        void SetLives(int lives);
        bool UseLive();

        string GetName();
        string GetAvatarId();
        int GetLives();
    }
}