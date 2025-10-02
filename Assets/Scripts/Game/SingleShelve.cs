using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Game
{
    public class SingleShelve : ShelveBase
    {
        [SerializeField] private GameObject[] layers;
        [SerializeField] private GameObject goodsPrefab;
        [SerializeField] private DropZone[] slot;
        // private readonly Dictionary<int, int> _layer0GoodMap = new();
        // private Queue<List<int>> _layerQueue = new();

        public override bool IsEmpty()
        {
            return layers.All(layer => layer.transform.childCount <= 0);
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(OnTrySupplyGoods());
        }

        
        private IEnumerator OnTrySupplyGoods()
        {
            while (true)
            {
                TrySupplyGoods();
                yield return new WaitForSeconds(0.15f);
            }
        }

        public override Goods CreateGoods(int goodsId, int slotId, int layer)
        {
            var goodsNode = Instantiate(goodsPrefab, layers[layer].transform);
            var goods = goodsNode.GetComponent<Goods>();
            var goodsDrop = goodsNode.GetComponent<DragDrop>();
            goods.Id = goodsId;
            goods.Layer = layer;
            goods.Slot = slotId;
            goods.StartPos = new Vector3(0, 0, 0);
            if (layer == 0)
            {
                slot[slotId].isOccupied = true;
                goodsDrop.CurrentZone = slot[slotId];
            }
            
            return goods;
        }
        
        public override int GetSlot(Vector3 goodsPos)
        {
            Debug.Log("Get Slot not use");
            return -1;
        }
        
        public override bool IsSlotOccupied(int slotIndex)
        {
            return true;
        }
        
        public new bool IsAllSlotOccupied()
        {
            return true;
        }
        
        public override void PlaceGoods(int goodsId, int slotId)
        {
            var goods = CreateGoods(goodsId, 0, 0);
            goods.Bounce();
        }
        
        protected override void OnPlaceGood()
        {
            Debug.Log("OnPlaceGood - SingleShelve");
            // DO NOTHING
        }

        public override void Clear()
        {
            if (slot != null)
            {
                foreach (var s in slot)
                {
                    s.isOccupied = false;
                }
            }
            for (var i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (layer)
                {
                    // Xóa tất cả children của layer
                    foreach (Transform child in layer.transform)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }

        public override void LoadNextLayers()
        {
            if (_layerQueue.Count > 0)
            {
                var layer0Data = _layerQueue.Dequeue();
                LoadLayerData(0, layer0Data);
            }
            if (_layerQueue.Count > 0)
            {
                var layer1Data = _layerQueue.Dequeue();
                LoadLayerData(1, layer1Data);
            }
        }

        private void LoadLayerData(int layerIndex, List<int> goodsData)
        {
            // random range int
            foreach (var t in goodsData)
            {
                CreateGoods(t, 0, layerIndex);
            }
        }
        
        private void TrySupplyGoods()
        {
            var layer0 = layers[0];
            var layer1 = layers[1];
            if (layer0.transform.childCount <= 0)
            {
                var goodsInLayer1 = layer1.GetComponentsInChildren<Goods>();
                foreach (var t in slot)
                {
                    t.isOccupied = false;
                }
                foreach (var goods in goodsInLayer1)
                {
                    goods.Layer = 0;
                    goods.transform.SetParent(layer0.transform);
                    goods.transform.position -= new Vector3(0, 0.1f, 0);
                    var goodsDrag = goods.GetComponent<DragDrop>();
                    goodsDrag.CurrentZone = slot[0];
                    goods.Bounce();
                }

                if (_layerQueue.Count > 0)
                {
                    var nextLayerData = _layerQueue.Dequeue();
                    LoadLayerData(1, nextLayerData);
                }
            }
        }
    }
}