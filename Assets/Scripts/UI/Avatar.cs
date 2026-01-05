using System.Collections.Generic;
using manager.Interface;
using Senspark;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Avatar : MonoBehaviour
    {
        [SerializeField] private Image avatarImage;

        private static readonly Dictionary<string, string> AvatarPathMap = new()
        {
            { "0", "Avatar/avatar_0" },
            { "1", "Avatar/avatar_1" },
            { "2", "Avatar/avatar_2" },
            { "3", "Avatar/avatar_3" },
            { "4", "Avatar/avatar_4" },
            { "5", "Avatar/avatar_5" },
            { "6", "Avatar/avatar_6" },
            { "7", "Avatar/avatar_7" },
            { "8", "Avatar/avatar_8" },
            { "9", "Avatar/avatar_9" },
            { "10", "Avatar/avatar_10" },
            { "11", "Avatar/avatar_11" }
        };

        private IProfileManager _profileManager;
        private int _observerId = -1;

        private void Start()
        {
            _profileManager = ServiceLocator.Instance.Resolve<IProfileManager>();
            RegisterObserver();
            LoadAvatar(_profileManager.GetAvatarId());
        }

        private void OnDestroy()
        {
            UnregisterObserver();
        }

        private void RegisterObserver()
        {
            _observerId = _profileManager.AddObserver(new ProfileManagerObserver
            {
                OnAvatarIdChanged = LoadAvatar
            });
        }

        private void UnregisterObserver()
        {
            if (_profileManager != null && _observerId >= 0)
                _profileManager.RemoveObserver(_observerId);
        }

        private void LoadAvatar(string avatarId)
        {
            if (!AvatarPathMap.TryGetValue(avatarId, out var path))
            {
                Debug.LogWarning($"[Avatar] Avatar ID '{avatarId}' not found in map");
                return;
            }

            var sprite = Resources.Load<Sprite>(path);
            if (sprite != null)
                avatarImage.sprite = sprite;
            else
                Debug.LogError($"[Avatar] Failed to load sprite at path: {path}");
        }
    }
}
