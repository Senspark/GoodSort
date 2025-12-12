using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Leaderboard
{
    public class LeaderboardLayer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private Sprite[] avatarList;

        [Header("Mock Data")]
        [SerializeField] private TextAsset mockDataFile;

        private void Start()
        {
            LoadLeaderboardData();
        }

        /// <summary>
        /// Load and populate leaderboard data from mock data file
        /// </summary>
        private void LoadLeaderboardData()
        {
            RemoveAllChildren();
            var entries = ParseMockData(mockDataFile.text);
            foreach (var entry in entries)
            {
                CreateLeaderboardItem(entry);
            }
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
        private void CreateLeaderboardItem(LeaderboardEntry entry)
        {
            var itemObj = Instantiate(itemPrefab, content);
            itemObj.name = $"Item_{entry.rank}_{entry.name}";
            var item = itemObj.GetComponent<ItemLeaderboard>();
            item.SetData(entry.rank, entry.name, entry.level, avatarList[Random.Range(0, avatarList.Length)]);
            ResizeItemWidth(itemObj);
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