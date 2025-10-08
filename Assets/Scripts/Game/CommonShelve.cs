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
        }

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
            var layerFrontGoods = layerFrontContainer.GetComponentsInChildren<Goods>();
            if (layerFrontGoods.Length < 3) return;
            var isAllSame = layerFrontGoods.All(goods => goods.Id == layerFrontGoods[0].Id);
            if(!isAllSame) return;
            foreach (var goods in layerFrontGoods)
            {
                goods.Remove();
            }

            ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.MergeGoods);
        }

        public override ShelfItem CreateGoods(int goodsId, int slotId, int layer)
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

            // Return ShelfItem with DragObject instead of DragDrop
            var item = new ShelfItem(goods, dragObject);
            return item;
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

        public override bool IsAllSlotOccupied()
        {
            return _layerFrontGoodsMap.Count >= 3;
        }

        public override void PlaceGoods(int goodsId, int slotId)
        {
            var goods = CreateGoods(goodsId, slotId, 0);
            if(goods != null)
            {
                goods.Goods.Bounce();
            }
        }

        protected override void OnPlaceGood()
        {
            CheckMerge();
        }

        public override void Clear()
        {
            _layerFrontGoodsMap.Clear();
            _layerBackGoodsMap.Clear();

            // Clear front layer
            foreach (Transform child in layerFrontContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Clear back layer
            foreach (Transform child in layerBackContainer.transform)
            {
                Destroy(child.gameObject);
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

        public DropZone FindAnyEmptyZone()
        {
            // Check if any slot in front layer is not occupied
            for (int slotId = -1; slotId <= 1; slotId++)
            {
                if (!IsSlotOccupied(slotId))
                {
                    return layerFrontZone;
                }
            }
            return null;
        }

        private void LoadLayerData(int layerIndex, List<int> goodsData)
        {
            var randomSlotId = new[] { -1, 0, 1 };
            ArrayUtils.Shuffle(randomSlotId);

            var items = new ShelfItem[goodsData.Count];
            for (var i = 0; i < goodsData.Count; i++)
            {
                var g = goodsData[i];
                if (g > 0)
                {
                    items[i] = CreateGoods(goodsData[i], randomSlotId[i], layerIndex);
                }
                else
                {
                    items[i] = null;
                }
            }

            VisibleItems ??= new List<ShelfItem>();
            VisibleItems.AddRange(items);
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
        }
    }

    public class ShelfItem
    {
        public ShelfItem(Goods goods, DragObject drag)
        {
            Goods = goods;
            Drag = drag;
        }

        public readonly Goods Goods;
        public readonly DragObject Drag;
    }
}
