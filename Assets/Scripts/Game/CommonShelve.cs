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
        private Queue<List<int>> _layerQueue = new();

        public override bool IsEmpty()
        {
            return layers.All(t => t.transform.childCount <= 0);
        }

        protected override void Start()
        {
            base.Start();
            Init();
            StartCoroutine(OnTrySupplyGoods());
            StartCoroutine(OnCheckMerge());
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

        // private void CreateLayerGood(int layerIndex, int bounceDelay = 0)
        // {
        //     if (!GameManager.Instance.GoodsData.TryGetValue(Id, out var shelveData))
        //         return;
        //
        //     foreach (var (slotId, goodsArr) in shelveData) // kvp.Key = slotId, kvp.Value = List<int>
        //     {
        //         if (goodsArr.Count <= 0)
        //             continue;
        //
        //         // shift(): lấy phần tử đầu và xoá
        //         var goodsId = goodsArr[0];
        //         goodsArr.RemoveAt(0);
        //
        //         if (goodsId == -1)
        //             continue;
        //
        //         var goods = CreateGoods(goodsId, slotId, layerIndex);
        //         goods.Bounce(bounceDelay);
        //     }
        // }
        
        private IEnumerator OnTrySupplyGoods()
        {
            while (true)
            {
                TrySupplyGoods();
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        private IEnumerator OnCheckMerge()
        {
            while (true)
            {
                CheckMerge();
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        private void CheckMerge()
        {
            // var goodsCount = _layer0GoodMap.Count;
            // if (goodsCount < 3) return;
            var layer0 = layers[0];
            var layer0GoodsArr = layer0.GetComponentsInChildren<Goods>();
            if (layer0GoodsArr.Length < 3) return;
            var isAllSame = layer0GoodsArr.All(goods => goods.Id == layer0GoodsArr[0].Id);
            if (!isAllSame) return;
            for (int i = 0; i < layer0GoodsArr.Length; i++)
            {
                var goods = layer0GoodsArr[i];
                goods.Remove();
            }
            // _layer0GoodMap.Clear();
            ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.MergeGoods);
        }

        public override Goods CreateGoods(int goodsId, int slotId, int layer)
        {
            var goodsNode = Instantiate(goodsPrefab, layers[layer].transform);
            var goods = goodsNode.GetComponent<Goods>();
            var goodsDrop = goodsNode.GetComponent<DragDrop>();
            goods.Id = goodsId;
            goods.Layer = layer;
            goods.Slot = slotId;
            if (layer == 0)
            {
                // _layer0GoodMap[slotId] = goodsId;
                slot[slotId].isOccupied = true;
                goodsDrop.CurrentZone = slot[slotId];
            }
            
            return goods;
        }
        
        public override int GetSlot(Vector3 goodsPos)
        {
           
            
            var localPos = transform.InverseTransformPoint(goodsPos);
            // x from -1.5 to -0.5 is slot 0
            // x from -0.5 to 0.5 is slot 1
            // x from 0.5 to 1.5 is slot 2
            if (localPos.x < -0.5f) return 0;
            if (localPos.x < 0.5f) return 1;
            if (localPos.x < 1.5f) return 2;
    
            return -1; // Không có slot trống
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
            // var layer0 = layers[0];
            // var layer0GoodsArr = layer0.GetComponentsInChildren<Goods>();
            // foreach (var goods in layer0GoodsArr)
            // {
            //     
            //     // _layer0GoodMap.Remove(goods.Slot);
            //     // slot[goods.Slot].isOccupied = false;
            // }
            // DO NOTHING
        }

        public List<int> GetGoodsIdArray()
        {
            var goodsIdArr = new List<int>();
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                var layerGoodsArr = layer.GetComponentsInChildren<Goods>();
                foreach (var goods in layerGoodsArr)
                {
                    goodsIdArr.Add(goods.Id);
                }
            }
            return goodsIdArr;
        }
        
        // public Dictionary<int, Dictionary<int, Goods>> GetGoodsInfo()
        // {
        //     var info = new Dictionary<int, Dictionary<int, Goods>>();
        //
        //     for (int i = 0; i < layers.Length; i++)
        //     {
        //         var layer = layers[i];
        //         if (layer == null) continue;
        //
        //         if (!info.ContainsKey(i))
        //         {
        //             info[i] = new Dictionary<int, Goods>();
        //         }
        //
        //         var layerGoodsArr = layer.GetComponentsInChildren<Goods>();
        //         for (var j = 0; j < layerGoodsArr.Length; j++)
        //         {
        //             var goods = layerGoodsArr[j];
        //             if (goods == null) continue;
        //             var goodsSlot = goods.Slot;
        //             info[i][goodsSlot] = goods;
        //         }
        //     }
        //
        //     return info;
        // }
        
        public override void Clear()
        {
            // _layer0GoodMap.Clear();
            if (slot != null)
            {
                foreach (var s in slot)
                {
                    s.isOccupied = false;
                }
            }
            for (int i = 0; i < layers.Length; i++)
            {
                GameObject layer = layers[i];
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
        
        public void RemoveGoods(Goods goods)
        {
            // if (goods == null) return;
            //
            // if (goods.Layer == 0)
            // {
            //     _layer0GoodMap.Remove(goods.Slot);
            // }
            // goods.Remove();
        }

        public void SetLayerQueue(Queue<List<int>> layerQueue)
        {
            _layerQueue = layerQueue;
        }

        public void LoadNextLayers()
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
            var randomSlotId = new[] { 0, 1, 2 };
            ArrayUtils.Shuffle(randomSlotId);
            for (var i = 0; i < goodsData.Count; i++)
            {
                CreateGoods(goodsData[i], randomSlotId[i], layerIndex);
            }
        }
        
        private void TrySupplyGoods()
        {
            Debug.Log("Try supply goods");
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

