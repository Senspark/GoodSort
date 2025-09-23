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
        private readonly Dictionary<int, int> _layer0GoodMap = new();
        private Queue<List<int>> _layerQueue = new();

        public override bool IsEmpty()
        {
            return layers.All(t => t.transform.childCount <= 0);
        }
        //
        // protected override void Awake()
        // {
        //     base.Awake();
        //     GameManager.Instance.Shelves.Add(this);
        // }

        protected override void Start()
        {
            base.Start();
            Init();
            StartCoroutine(OnTrySupplyGoods());
            StartCoroutine(OnCheckMerge());
        }

        public override void Init()
        {
            const int bounceDelay = 1;
            CreateLayerGood(0, bounceDelay);
            CreateLayerGood(1, bounceDelay);
        }

        private void CreateLayerGood(int layerIndex, int bounceDelay = 0)
        {
            if (!GameManager.Instance.GoodsData.TryGetValue(Id, out var shelveData))
                return;

            foreach (var (slotId, goodsArr) in shelveData) // kvp.Key = slotId, kvp.Value = List<int>
            {
                if (goodsArr.Count <= 0)
                    continue;

                // shift(): lấy phần tử đầu và xoá
                int goodsId = goodsArr[0];
                goodsArr.RemoveAt(0);

                if (goodsId == -1)
                    continue;

                var goods = CreateGoods(goodsId, slotId, layerIndex);
                goods.Bounce(bounceDelay);
            }
        }
        
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
            var goodsCount = _layer0GoodMap.Count;
            if (goodsCount < 3) return;
            var layer0 = layers[0];
            var layer0GoodsArr = layer0.GetComponentsInChildren<Goods>();
            var isAllSame = layer0GoodsArr.All(goods => goods.Id == layer0GoodsArr[0].Id);
            if (!isAllSame) return;
            for (int i = 0; i < layer0GoodsArr.Length; i++)
            {
                var goods = layer0GoodsArr[i];
                goods.Remove();
            }
            _layer0GoodMap.Clear();
            ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.MergeGoods, transform.position);
        }

        public override Goods CreateGoods(int goodsId, int slotId, int layer)
        {
            var goodsNode = Instantiate(goodsPrefab, layers[layer].transform);
            var goods = goodsNode.GetComponent<Goods>();
            goods.Id = goodsId;
            goods.Layer = layer;
            goods.Slot = slotId;
            goods.Shelve = this;
            if (layer == 0) _layer0GoodMap[slotId] = goodsId;
            
            return goods;
        }
        
        public override int GetSlot(Vector3 goodsPos)
        {
            // var availableSlots = new Vector3[]
            //     {
            //         new Vector3(-1, 0, 0),
            //         new Vector3(0, 0, 0),
            //         new Vector3(1, 0, 0)
            //     }
            //     .Select((pos, index) => (slotId: index, distance: Vector3.Distance(goodsPos, pos)))
            //     .Where(x => !IsSlotOccupied(x.slotId))
            //     .OrderBy(x => x.distance);
            //
            // if (availableSlots.Any())
            // {
            //     return availableSlots.First().slotId;
            // }
            // local pos of goods in shelve
            var localPos = transform.InverseTransformPoint(goodsPos);
            Debug.Log($"GetSlot localPos: {localPos}");
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
            Debug.Log($"///////");
            foreach (var goods in _layer0GoodMap)
            {
                Debug.Log($"IsSlotOccupied: {goods.Key}");
                Debug.Log($"IsSlotOccupied: {goods.Value}");
                Debug.Log($"-----");
            }
            
            return _layer0GoodMap.ContainsKey(slotIndex);
        }
        
        public new bool IsAllSlotOccupied()
        {
            return _layer0GoodMap.Count == 3;
        }
        
        public override void PlaceGoods(int goodsId, int slotId)
        {
            var goods = CreateGoods(goodsId, slotId, 0);
            goods.Bounce();
        }
        
        protected override void OnPlaceGood()
        {
            var layer0 = layers[0];
            var layer0GoodsArr = layer0.GetComponentsInChildren<Goods>();
            foreach (var goods in layer0GoodsArr)
            {
                if (goods.Visible) continue;
                goods.Remove();
                _layer0GoodMap.Remove(goods.Slot);
            }
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
        
        public Dictionary<int, Dictionary<int, Goods>> GetGoodsInfo()
        {
            Dictionary<int, Dictionary<int, Goods>> info = new Dictionary<int, Dictionary<int, Goods>>();
    
            for (int i = 0; i < layers.Length; i++)
            {
                GameObject layer = layers[i];
                if (layer == null) continue;
        
                if (!info.ContainsKey(i))
                {
                    info[i] = new Dictionary<int, Goods>();
                }
        
                Goods[] layerGoodsArr = layer.GetComponentsInChildren<Goods>();
                for (int j = 0; j < layerGoodsArr.Length; j++)
                {
                    Goods goods = layerGoodsArr[j];
                    if (goods == null) continue;
                    int slot = goods.Slot;
                    info[i][slot] = goods;
                }
            }
    
            return info;
        }
        
        public override void Clear()
        {
            _layer0GoodMap.Clear();
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
            var randomSlotId = new int[] { 0, 1, 2 };
            ArrayUtils.Shuffle(randomSlotId);
            for (var i = 0; i < goodsData.Count; i++)
            {
                CreateGoods(goodsData[i], randomSlotId[i], layerIndex);
            }
        }
        
        private void TrySupplyGoods()
        {
            var layer0 = layers[0];
            var layer1 = layers[1];
            if (layer0.transform.childCount <= 0)
            {
                var goodsInLayer1 = layer1.GetComponentsInChildren<Goods>();
                foreach (var goods in goodsInLayer1)
                {
                    goods.Layer = 0;
                    goods.transform.SetParent(layer0.transform);
                    goods.transform.position -= new Vector3(0, 0.1f, 0);
                    _layer0GoodMap[goods.Slot] = goods.Id;
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

