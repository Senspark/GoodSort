using System;
using System.Collections.Generic;
using manager;
using UnityEngine;
using System.Collections;
using System.Linq;
using Constant;
using manager.Interface;
using Senspark;
using Unity.Mathematics;
using Utilities;


namespace Game
{
    public class CommonShelve : ShelveBase
    {
        [SerializeField] private GameObject[] layers;
        [SerializeField] private GameObject goodsPrefab;
        [SerializeField] private DropZone[] slot;
        // private readonly Dictionary<int, int> _layer0GoodMap = new();
        // private Queue<List<int>> _layerQueue = new();
        private bool _mergeInProgress = false;

        public override bool IsEmpty()
        {
            return layers.All(t => t.transform.childCount <= 0);
        }

        protected override void Start()
        {
            base.Start();
            Init();
            StartCoroutine(OnTrySupplyGoods());
            // StartCoroutine(OnCheckMerge());
        }

        public override void Init()
        {
            // const int bounceDelay = 1;
            // CreateLayerGood(0, bounceDelay);
            // CreateLayerGood(1, bounceDelay);
            for (var i = 0; i < slot.Length; i++)
            {
                var slotIndex = i;
                slot[i].OnGoodsDropped += (zone, item) =>
                {
                    HandleGoodsDropped(slotIndex, zone, item);
                };
            }
        }

        private void HandleGoodsDropped(int slotId, DropZone zone, DragDrop item)
        {
            var layer0 = layers[0];
            item.transform.SetParent(layer0.transform, false);
            item.transform.localPosition = zone.transform.localPosition;
            
            item.CurrentZone.Free();
            item.CurrentZone = zone;
            ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.PlaceGood);
        }
        
        private IEnumerator OnTrySupplyGoods()
        {
            while (true)
            {
                TrySupplyGoods();
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        // private IEnumerator OnCheckMerge()
        // {
        //     while (true)
        //     {
        //         CheckMerge();
        //         yield return new WaitForSeconds(0.15f);
        //     }
        // }
        
        private void CheckMerge()
        {
            if (_mergeInProgress) 
            {
                return;
            }
            
            var layer0 = layers[0];
            var layer0GoodsArr = layer0.GetComponentsInChildren<Goods>();
            if (layer0GoodsArr.Length < 3) return;
            
            var isAllSame = layer0GoodsArr.All(goods => goods.Id == layer0GoodsArr[0].Id);
            if (!isAllSame) return;
            
            _mergeInProgress = true;
            
            for (int i = 0; i < layer0GoodsArr.Length; i++)
            {
                var goods = layer0GoodsArr[i];
                goods.Remove();
            }

            ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.MergeGoods);
            
            // Reset flag after a short delay
            StartCoroutine(ResetMergeFlag());
        }

        private IEnumerator ResetMergeFlag()
        {
            yield return new WaitForSeconds(0.1f);
            _mergeInProgress = false;
        }

        public override Goods CreateGoods(int goodsId, int slotId, int layer)
        {
            var goodsNode = Instantiate(goodsPrefab, layers[layer].transform);
            var goods = goodsNode.GetComponent<Goods>();
            var goodsDrop = goodsNode.GetComponent<DragDrop>();
            goods.Id = goodsId;
            goods.Layer = layer;
            goods.Slot = slotId;
            goods.StartPos = new Vector3(slotId-1, 0, 0);
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
            return slot[slotIndex].isOccupied;
        }
        
        public new bool IsAllSlotOccupied()
        {
            return slot.All(s => s.isOccupied);
        }
        
        public override void PlaceGoods(int goodsId, int slotId)
        {
            var goods = CreateGoods(goodsId, slotId, 0);
            goods.Bounce();
        }
        
        protected override void OnPlaceGood()
        {
            Debug.Log("OnPlaceGood - CommonShelve");
            CheckMerge();
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
            for (var i = 0; i < goodsData.Count; i++)
            {
                CreateGoods(goodsData[i], i, layerIndex);
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
                    goods.transform.position -= new Vector3(0, 0.2f, 0);
                    var newSlot = goods.Slot;
                    var goodsDrag = goods.GetComponent<DragDrop>();
                    if (newSlot >= 0 && newSlot < slot.Length)
                    {
                        slot[newSlot].isOccupied = true;
                        goodsDrag.CurrentZone = slot[newSlot];
                    }
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

