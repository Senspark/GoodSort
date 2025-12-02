using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class BottomBar : MonoBehaviour
    {
        [SerializeField] private Button tabShop;
        [SerializeField] private Button tabHome;
        [SerializeField] private Button tabLeaderboard;
        
        private void Awake()
        {
            tabShop.onClick.AddListener(() =>
            {
                Debug.Log("Shop");
            });
            tabHome.onClick.AddListener(() =>
            {
                Debug.Log("Home");
            });
            tabLeaderboard.onClick.AddListener(() =>
            {
                Debug.Log("Leaderboard");
            });
        }
    }
}