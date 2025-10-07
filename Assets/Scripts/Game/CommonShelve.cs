using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using Constant;
using Core;
using manager.Interface;
using Senspark;
using Utilities;


namespace Game
{
    public class CommonShelve : ShelveBase
    {
        [SerializeField] private GameObject layerFrontContainer;
        [SerializeField] private GameObject layerBackContainer;

        [SerializeField] private DropZone layerFrontZone;
        [SerializeField] private DropZone layerBackZone;
        
        [SerializeField] private GameObject goodsPrefab;
        // [SerializeField] private DropZone[] slot;
        // private bool _mergeInProgress;
        private Dictionary<int, Goods> _layerFrontGoodsMap = new();
        private Dictionary<int, Goods> _layerBackGoodsMap = new();

        public override bool IsEmpty()
        {
            return layerFrontContainer.transform.childCount <= 0 && layerBackContainer.transform.childCount <= 0;
        }

        protected override void Start()
        {
            base.Start();
            InitializeZones();
            StartCoroutine(OnTrySupplyGoods());
        }

        private void InitializeZones()
        {
            layerFrontZone.SetActive(true);
            layerBackZone.SetActive(false);
            // for (var i = 0; i < slot.Length; i++)
            // {
            //     var slotIndex = i;
            //     slot[i].OnGoodsDropped += (zone, item) =>
            //     {
            //         HandleGoodsDropped(slotIndex, zone, item);
            //     };
            // }
        }

        // private void HandleGoodsDropped(int slotId, DropZone zone, DragDrop item)
        // {
        //     var layer0 = layers[0];
        //     item.transform.SetParent(layer0.transform, false);
        //     item.transform.localPosition = zone.transform.localPosition;
        //     
        //     item.CurrentZone.Free();
        //     item.CurrentZone = zone;
        //     ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.PlaceGood);
        // }
        
        private IEnumerator OnTrySupplyGoods()
        {
            while (true)
            {
                TrySupplyGoods();
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        private void CheckMerge()
        {
            // if (_mergeInProgress) 
            // {
            //     return;
            // }
            
            // var layer0 = layers[0];
            // var layer0GoodsArr = layer0.GetComponentsInChildren<Goods>();
            // if (layer0GoodsArr.Length < 3) return;
            //
            // var isAllSame = layer0GoodsArr.All(goods => goods.Id == layer0GoodsArr[0].Id);
            // if (!isAllSame) return;
            //
            // // _mergeInProgress = true;
            //
            // for (var i = 0; i < layer0GoodsArr.Length; i++)
            // {
            //     var goods = layer0GoodsArr[i];
            //     goods.Remove();
            // }
            var layerFrontGoods = layerFrontContainer.GetComponentsInChildren<Goods>();
            if (layerFrontGoods.Length < 3) return;
            var isAllSame = layerFrontGoods.All(goods => goods.Id == layerFrontGoods[0].Id);
            if(!isAllSame) return;
            foreach (var goods in layerFrontGoods)
            {
                goods.Remove();
            }

            ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.MergeGoods);
            
            // StartCoroutine(ResetMergeFlag());
        }

        // private IEnumerator ResetMergeFlag()
        // {
        //     yield return new WaitForSeconds(0.1f);
        //     _mergeInProgress = false;
        // }

        public override Goods CreateGoods(int goodsId, int slotId, int layer)
        {
            var targetLayer = layer == 0 ? layerFrontContainer : layerBackContainer;
            var targetZone = layer == 0 ? layerFrontZone : layerBackZone;
            
            var goodsNode = Instantiate(goodsPrefab, targetLayer.transform);
            var goods = goodsNode.GetComponent<Goods>();
            var dragObject = goodsNode.GetComponent<DragObject>();
            
            goods.Id = goodsId;
            goods.Layer = layer;
            goods.Slot = slotId;
            
            dragObject.SetCurrentZone(targetZone);
            var slotPos = Vector3.zero;

            switch (layer)
            {
                case 0:
                    dragObject.SetDraggable(true);
                    slotPos = layerFrontZone.GetSnapPosition(slotId);
                    _layerFrontGoodsMap[slotId] = goods;
                    break;
                case 1:
                    dragObject.SetDraggable(false);
                    slotPos = layerBackZone.GetSnapPosition(slotId);
                    _layerBackGoodsMap[slotId] = goods;
                    break;
            }
            dragObject.UpdatePosition(slotPos);
            
            return goods;
        }
        
        
        public override int GetSlot(Vector3 goodsPos)
        {
            if (goodsPos.x < -0.5f)
                return -1;

            if (goodsPos.x > 0.5f)
                return 1;

            return 0;
        }

        
        public override bool IsSlotOccupied(int slotIndex)
        {
            if (_layerFrontGoodsMap.TryGetValue(slotIndex, out var goods))
            {
                if (goods != null)
                {
                    return true;
                }
            }

            return false;
        }
        
        public new bool IsAllSlotOccupied()
        {
            return _layerFrontGoodsMap.Count >= 3;
        }
        
        public override void PlaceGoods(int goodsId, int slotId)
        {
            var goods = CreateGoods(goodsId, slotId, 0);
            if(goods != null)
            {
                goods.Bounce();
            }
        }
        
        protected override void OnPlaceGood()
        {
            CheckMerge();
            // DO NOTHING
        }

        public override void Clear()
        {
            // if (slot != null)
            // {
            //     foreach (var s in slot)
            //     {
            //         s.isOccupied = false;
            //     }
            // }
            // for (var i = 0; i < layers.Length; i++)
            // {
            //     var layer = layers[i];
            //     if (layer)
            //     {
            //         // Xóa tất cả children của layer
            //         foreach (Transform child in layer.transform)
            //         {
            //             Destroy(child.gameObject);
            //         }
            //     }
            // }
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
            var randomSlotId = new[] { -1, 0, 1 };
            ArrayUtils.Shuffle(randomSlotId);
            for (var i = 0; i < goodsData.Count; i++)
            {
                CreateGoods(goodsData[i], randomSlotId[i], layerIndex);
            }
        }
        
        private void TrySupplyGoods()
        {
            if (layerFrontContainer.transform.childCount <= 0)
            {
                var goodsInLayerBack = layerBackContainer.GetComponentsInChildren<Goods>();
                _layerFrontGoodsMap[-1] = null;
                _layerFrontGoodsMap[0] = null;
                _layerFrontGoodsMap[1] = null;
                foreach (var goods in goodsInLayerBack)
                {
                    goods.Layer = 0;
                    goods.transform.SetParent(layerFrontContainer.transform);
                    var newSlot = goods.Slot;
                    var goodsDrag = goods.GetComponent<DragObject>();
                    _layerFrontGoodsMap[newSlot] = goods;
                    goodsDrag.SetCurrentZone(layerFrontZone);
                    goodsDrag.SetDraggable(true);
                    goodsDrag.UpdatePosition(layerFrontZone.GetSnapPosition(newSlot));
                    goods.Bounce();
                }
                _layerBackGoodsMap.Clear();
                if (_layerQueue.Count > 0)
                {
                    var nextLayerData = _layerQueue.Dequeue();
                    LoadLayerData(1, nextLayerData);
                }
            }
            // var layer0 = layers[0];
            // var layer1 = layers[1];
            // if (layer0.transform.childCount <= 0)
            // {
            //     var goodsInLayer1 = layer1.GetComponentsInChildren<Goods>();
            //     foreach (var t in slot)
            //     {
            //         t.isOccupied = false;
            //     }
            //     foreach (var goods in goodsInLayer1)
            //     {
            //         goods.Layer = 0;
            //         goods.transform.SetParent(layer0.transform);
            //         goods.transform.position -= new Vector3(0, 0.2f, 0);
            //         var newSlot = goods.Slot;
            //         var goodsDrag = goods.GetComponent<DragDrop>();
            //         if (newSlot >= 0 && newSlot < slot.Length)
            //         {
            //             slot[newSlot].isOccupied = true;
            //             goodsDrag.CurrentZone = slot[newSlot];
            //         }
            //         goods.Bounce();
            //     }
            //
            //     if (_layerQueue.Count > 0)
            //     {
            //         var nextLayerData = _layerQueue.Dequeue();
            //         LoadLayerData(1, nextLayerData);
            //     }
            // }
        }
        
    }
}

