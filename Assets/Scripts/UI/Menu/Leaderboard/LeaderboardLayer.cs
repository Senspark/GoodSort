using System.Collections.Generic;
using System.Linq;
using manager.Interface;
using Senspark;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Leaderboard
{
    public class LeaderboardLayer : MonoBehaviour
    {
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private Sprite[] avatarList;
        [SerializeField] private NestedScroll nestedScroll;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private ItemLeaderboard staticItem;

        [Header("Mock Data")] [SerializeField] private TextAsset mockDataFile;

        private RectTransform _myItemInContent;
        private int _myRank;
        private IProfileManager _profileManager;
        private ILevelManager _levelManager;
        private Sprite _myAvatarSprite;
        private int _observerId = -1;

        private void Start()
        {
            _profileManager = ServiceLocator.Instance.Resolve<IProfileManager>();
            _levelManager = ServiceLocator.Instance.Resolve<ILevelManager>();
            _myAvatarSprite = LoadAvatarSprite(_profileManager.GetAvatarId());
            RegisterObserver();
            LoadLeaderboardData();
            ResizeItemWidth(staticItem.gameObject);
            nestedScroll.onValueChanged.AddListener(OnScroll);
            UpdateStaticItemVisibility();
        }

        private void OnDestroy()
        {
            UnregisterObserver();
        }

        private void RegisterObserver()
        {
            _observerId = _profileManager.AddObserver(new ProfileManagerObserver
            {
                OnAvatarIdChanged = OnAvatarChanged,
                OnNameChanged = OnNameChanged
            });
        }

        private void UnregisterObserver()
        {
            if (_profileManager != null && _observerId >= 0)
                _profileManager.RemoveObserver(_observerId);
        }

        private void OnAvatarChanged(string avatarId)
        {
            _myAvatarSprite = LoadAvatarSprite(avatarId);
            // Reload leaderboard to update avatar for player's items
            LoadLeaderboardData();
            UpdateStaticItemVisibility();
        }

        private void OnNameChanged(string newName)
        {
            // Reload leaderboard to update name for player's items
            LoadLeaderboardData();
            UpdateStaticItemVisibility();
        }

        /// <summary>
        /// Load avatar sprite from Resources based on avatarId
        /// </summary>
        private Sprite LoadAvatarSprite(string avatarId)
        {
            var path = $"Avatar/avatar_{avatarId}";
            var sprite = Resources.Load<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogWarning($"[LeaderboardLayer] Failed to load avatar at path: {path}");
                // Fallback to first avatar in list
                return avatarList.Length > 0 ? avatarList[0] : null;
            }
            return sprite;
        }

        private void OnScroll(Vector2 pos)
        {
            UpdateStaticItemVisibility();
        }

        /// <summary>
        /// Update static item visibility based on player's rank and scroll position
        /// </summary>
        private void UpdateStaticItemVisibility()
        {
            if (_myRank > 100)
            {
                staticItem.gameObject.SetActive(true);
                return;
            }

            if (_myItemInContent != null)
            {
                bool isVisible = IsItemVisibleInViewport(_myItemInContent);
                staticItem.gameObject.SetActive(!isVisible);
            }
        }
        
        bool IsItemVisibleInViewport(RectTransform item)
        {
            Vector3[] itemCorners = new Vector3[4];
            Vector3[] viewportCorners = new Vector3[4];

            item.GetWorldCorners(itemCorners);
            viewport.GetWorldCorners(viewportCorners);

            Rect itemRect = new Rect(
                itemCorners[0],
                itemCorners[2] - itemCorners[0]
            );

            Rect viewportRect = new Rect(
                viewportCorners[0],
                viewportCorners[2] - viewportCorners[0]
            );

            return itemRect.Overlaps(viewportRect);
        }

        /// <summary>
        /// Load and populate leaderboard data from mock data file
        /// </summary>
        private void LoadLeaderboardData()
        {
            var myPlayerName = _profileManager.GetName();
            var myPlayerLevel = _levelManager.GetCurrentLevel();
            var newItem = new LeaderboardEntry
            {
                rank = 0,
                level = myPlayerLevel,
                name = myPlayerName
            };
            RemoveAllChildren();
            _myItemInContent = null;
            var entries = ParseMockData(mockDataFile.text);
            entries = AddItem(entries, newItem);

            // Find player's rank after sorting
            _myRank = entries.Find(e => e.name == myPlayerName).rank;

            // Setup static item with player's data and correct avatar from profile
            var myEntry = entries.Find(e => e.name == myPlayerName);
            staticItem.SetData(myEntry.rank, myEntry.name, myEntry.level, _myAvatarSprite);

            for(var i = 0; i < 100; i++)
            {
                var itemObj = CreateLeaderboardItem(entries[i], entries[i].name == myPlayerName);
                // Track player's item in content if within top 100
                if (entries[i].name == myPlayerName)
                {
                    _myItemInContent = itemObj.GetComponent<RectTransform>();
                }
            }
        }
        
        private List<LeaderboardEntry> AddItem(List<LeaderboardEntry> oldList, LeaderboardEntry newEntry)
        {
            newEntry.rank = 0;
            oldList.Add(newEntry);

            oldList = oldList
                .OrderByDescending(e => e.level)
                .ToList();

            for (var i = 0; i < oldList.Count; i++)
            {
                var entry = oldList[i];
                entry.rank = i + 1;
                oldList[i] = entry;
            }
            return oldList;
        }

        /// <summary>
        /// Remove all children from content
        /// </summary>
        private void RemoveAllChildren()
        {
            var childCount = content.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                var child = content.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Parse mock data from CSV format
        /// </summary>
        private List<LeaderboardEntry> ParseMockData(string csvText)
        {
            var entries = new List<LeaderboardEntry>();
            var lines = csvText.Split('\n');

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 3)
                {
                    Debug.LogWarning($"[LeaderboardLayer] Invalid line {i}: {line}");
                    continue;
                }

                var entry = new LeaderboardEntry
                {
                    rank = int.Parse(parts[0].Trim()),
                    level = int.Parse(parts[1].Trim()),
                    name = parts[2].Trim()
                };

                entries.Add(entry);
            }

            return entries;
        }

        /// <summary>
        /// Create a leaderboard item and add to content
        /// </summary>
        private GameObject CreateLeaderboardItem(LeaderboardEntry entry, bool isMyItem = false)
        {
            var itemObj = Instantiate(itemPrefab, content);
            itemObj.name = $"Item_{entry.rank}_{entry.name}";
            var item = itemObj.GetComponent<ItemLeaderboard>();
            var avatar = isMyItem ? _myAvatarSprite : avatarList[Random.Range(0, avatarList.Length)];
            item.SetData(entry.rank, entry.name, entry.level, avatar);
            ResizeItemWidth(itemObj);
            return itemObj;
        }

        /// <summary>
        /// Resize item to percentage of content width
        /// </summary>
        private void ResizeItemWidth(GameObject item)
        {
            var rectTransform = item.GetComponent<RectTransform>();
            var contentRect = content.GetComponent<RectTransform>();
            var contentWidth = contentRect.rect.width;
            var itemWidth = contentWidth * 0.9f;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemWidth);
        }

        /// <summary>
        /// Reload leaderboard data
        /// </summary>
        public void Reload()
        {
            LoadLeaderboardData();
            UpdateStaticItemVisibility();
        }
    }

    /// <summary>
    /// Leaderboard entry data structure
    /// </summary>
    [System.Serializable]
    public struct LeaderboardEntry
    {
        public int rank;
        public int level;
        public string name;
    }
}