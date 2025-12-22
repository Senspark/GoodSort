using manager.Interface;

namespace Dialog.Controller
{
    public interface IProfileDialogController
    {
        int RegisterObserver(ProfileManagerObserver observer);
        void UnregisterObserver(int observerId);
        string GetName();
        string GetAvatarId();
        void SaveProfile(string name, string avatarId);
    }

    public class ProfileDialogController : IProfileDialogController
    {
        private readonly IProfileManager _profileManager;

        public ProfileDialogController(IProfileManager profileManager)
        {
            _profileManager = profileManager;
        }

        public int RegisterObserver(ProfileManagerObserver observer)
        {
            return _profileManager.AddObserver(observer);
        }

        public void UnregisterObserver(int observerId)
        {
            if (observerId >= 0)
                _profileManager.RemoveObserver(observerId);
        }

        public string GetName()
        {
            return _profileManager.GetName();
        }

        public string GetAvatarId()
        {
            return _profileManager.GetAvatarId();
        }

        public void SaveProfile(string name, string avatarId)
        {
            if (!string.IsNullOrEmpty(name))
                _profileManager.SetName(name);

            _profileManager.SetAvatarId(avatarId);
        }
    }
}