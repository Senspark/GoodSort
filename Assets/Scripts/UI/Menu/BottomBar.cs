using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Menu
{
    public class BottomBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // [SerializeField] private Button tabShop;
        // [SerializeField] private Button tabHome;
        // [SerializeField] private Button tabLeaderboard;
        //
        // private void Awake()
        // {
        //     tabShop.onClick.AddListener(() =>
        //     {
        //         Debug.Log("Shop");
        //     });
        //     tabHome.onClick.AddListener(() =>
        //     {
        //         Debug.Log("Home");
        //     });
        //     tabLeaderboard.onClick.AddListener(() =>
        //     {
        //         Debug.Log("Leaderboard");
        //     });
        public void OnBeginDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}