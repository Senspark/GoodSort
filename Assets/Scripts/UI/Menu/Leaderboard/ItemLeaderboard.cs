using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Leaderboard
{
    public class ItemLeaderboard : MonoBehaviour
    {
        [SerializeField] private TMP_Text textRank;
        [SerializeField] private TMP_Text textName;
        [SerializeField] private TMP_Text textLevel;
        [SerializeField] private Image imageAvatar;
        
        public void SetData(int rank, string name, int level, Sprite avatar)
        {
            // Display "100+" for ranks greater than 100
            textRank.text = rank > 100 ? "100+" : rank.ToString();
            textName.text = name;
            textLevel.text = level.ToString();
            imageAvatar.sprite = avatar;
        }
    }
}