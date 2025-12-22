using System;
using Senspark;

namespace manager.Interface
{
    public class ProfileManagerObserver
    {
        public Action<string> OnNameChanged { get; set; }

        public Action<string> OnAvatarIdChanged { get; set; }

        public Action<int> OnCoinsChanged { get; set; }
    }

    [Service(nameof(IProfileManager))]
    public interface IProfileManager : IObserverManager<ProfileManagerObserver>, IService
    {
        // Setters
        void SetName(string name);
        void SetAvatarId(string avatarId);
        void AddCoins(int amount);

        // Getters
        string GetName();
        string GetAvatarId();
        int GetCoins();
    }
}