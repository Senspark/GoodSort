using System;
using Constant;
using manager.Interface;
using Senspark;
using UnityEngine;

namespace Game
{
    public class LockGlass : MonoBehaviour
    {
        [SerializeField]
        private int unlockCount;

        private int UnlockCount
        {
            get => unlockCount;
            set
            {
                unlockCount = Math.Max(0, value);
                if (unlockCount <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void Start()
        {
            ServiceLocator.Instance.Resolve<IEventManager>().AddListener(EventKey.MergeGoods, OnMergeGoods);
        }
        
        private void OnMergeGoods()
        {
            Debug.Log("Merge goods on lock glass");
            UnlockCount--;
        }
        
        
        private void OnDestroy()
        {
            ServiceLocator.Instance.Resolve<IEventManager>().RemoveListener(EventKey.MergeGoods, OnMergeGoods);
        }
    }
}