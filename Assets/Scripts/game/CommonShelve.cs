using System.Collections.Generic;
using manager;
using UnityEngine;
using System.Collections;
using System.Linq;
using Defines;


namespace game
{
    public class CommonShelve : ShelveBase
    {
        [SerializeField] private GameObject[] layers;
        [SerializeField] private GameObject goodsPrefab;
        private Dictionary<int, int> layer0GoodMap = new Dictionary<int, int>();

        public override bool IsEmpty()
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
            GameManager.Instance.Shelves.Add(this);
        }

        protected override void Start()
        {
            base.Start();
            Init();
            StartCoroutine(OnTrySupplyGoods());
            StartCoroutine(OnCheckMerge());
        }

        public override void Init(int bounceDelay = 0)
        {
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
                yield return new WaitForSeconds(0.25f);
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
                    goods.transform.position -= new Vector3(0, 10, 0);
                    layer0GoodMap[goods.Slot] = goods.Id;
                    goods.Bounce();
                }
                CreateLayerGood(1);
            }
            if (layer1.transform.childCount <= 0)
            {
                CreateLayerGood(1);
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
            var goodsCount = layer0GoodMap.Count;
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
            layer0GoodMap.Clear();
            EventManager.Instance.Emit(EventKey.MergeGoods, transform.position);
            Debug.Log($"Merge goods at {transform.position}");
        }

        public override Goods CreateGoods(int goodsId, int slotId, int layer)
        {
            var goodsNode = Instantiate(goodsPrefab, layers[layer].transform);
            var goods = goodsNode.GetComponent<Goods>();
            var layerNode = layers[layer];
            var pos = new Vector3((slotId - 1) * (layerNode.GetComponent<RectTransform>().rect.width - 10) / 3, layer * 10, 0);
            goodsNode.transform.localPosition = pos;
            goods.Id = goodsId;
            goods.Layer = layer;
            goods.Slot = slotId;
            if (layer == 0) layer0GoodMap[slotId] = goodsId;
            return goods;
        }
        
        public override int GetSlot(Vector3 goodsPos)
        {
            var offsetX = 0.5f * GetComponent<RectTransform>().rect.width / 3f;
    
            return new Vector3[]
                {
                    new Vector3(-offsetX, 0, 0),
                    new Vector3(0, 0, 0),
                    new Vector3(offsetX, 0, 0)
                }
                .Select((pos, index) => (slotId: index, distance: Vector3.Distance(goodsPos, pos)))
                .OrderBy(x => x.distance)
                .FirstOrDefault(x => !IsSlotOccupied(x.slotId))
                .slotId;
        }
        
        public new bool IsSlotOccupied(int slotIndex)
        {
            return layer0GoodMap.ContainsKey(slotIndex);
        }
        
        public new bool IsAllSlotOccupied()
        {
            return layer0GoodMap.Count == 3;
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
            Debug.Log("KHOA TRAN ----------- OnPlaceGood" + Id);
            foreach (var goods in layer0GoodsArr)
            {
                Debug.Log("KHOA TRAN ----------- OnPlaceGood 1");
                if (goods.Visible) continue;
                Debug.Log("KHOA TRAN ----------- OnPlaceGood 2");
                goods.Remove();
                layer0GoodMap.Remove(goods.Slot);
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
            layer0GoodMap.Clear();
            for (int i = 0; i < layers.Length; i++)
            {
                GameObject layer = layers[i];
                if (layer != null)
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
            if (goods == null) return;
    
            int layer = goods.Layer;
            int slot = goods.Slot;
    
            if (layer == 0)
            {
                layer0GoodMap.Remove(slot);
            }
            goods.Remove();
        }
        
    }
}

