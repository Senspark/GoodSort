using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace manager
{
    public class GameManager : MonoBehaviour, IDragHandler, IEndDragHandler, ICancelHandler
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("GameManager is null");
                }
                return _instance;
            }
        }

        private readonly int pickOffset = 10;
        private GameObject _holdingGood = null;
        
        public List<ShelveBase> shelves;
        
        private void Awake()
        {
            _instance = this;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            var holdingGood = Instance._holdingGood;
            if (holdingGood == null) return;
            var uiPos = eventData.position;
            uiPos.y += pickOffset;
            Vector2 localPos;
            //rect transform is gameManager
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, uiPos, eventData.pressEventCamera, out localPos);
            holdingGood.transform.localPosition = localPos;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (Instance._holdingGood == null) return;
            Instance.release();
        }
        
        public void OnCancel(BaseEventData eventData)
        {
            
        }

        public void release()
        {
            var isSuccess = false;
            var holdingGoodWorldPos = Instance._holdingGood.transform.position;
            
        }
    }
}