using System.Collections.Generic;
using manager;
using UnityEngine;

namespace game
{
    public class CommonShelve : ShelveBase
    {
        [SerializeField] private GameObject[] layers;
        private Dictionary<int, int> layer0GoodMap = new Dictionary<int, int>();

        public bool isEmpty()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].transform.childCount > 0) return false;
            }
            return true;
        }
    
        protected override void Awake()
        {
            base.Awake();
            GameManager.Instance.shelves.Add(this);
        }
    
        protected override void Start() {}

        protected new void Init(int bounceDelay = 0)
        {
            createLayerGood(0, bounceDelay);
            createLayerGood(1, bounceDelay);
        }
    
        public void createLayerGood(int layerIndex, int bounceDelay = 0)
        {
            for (int i = 0; i < layers[layerIndex].transform.childCount; i++)
            {
                var good = layers[layerIndex].transform.GetChild(i).GetComponent<Goods>();
                // good.Init(bounceDelay);
            }
        }
    }
}

