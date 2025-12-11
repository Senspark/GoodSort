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
            textRank.text = rank.ToString();
            textName.text = name;
            textLevel.text = level.ToString();
            imageAvatar.sprite = avatar;
        }
    }
}