using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using game;
using UnityEngine.UI;
using Defines;


namespace manager
{
    public class GameManager : MonoBehaviour, IDragHandler,IBeginDragHandler, IEndDragHandler
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

        [SerializeField] private RectTransform levelRoot;
        [SerializeField] private GameObject stagePrefab;
        [SerializeField] private TextAsset levelConfigText;
        [SerializeField] private TextAsset iconConfigText;

        public LevelConfigArray LevelConfigs { get; private set; }
        public IconConfig[] IconConfigs { get; private set; }
        private GameObject _holdingGood;
        private Vector2 _dragOffset;

        public GameObject HoldingGood
        {
            get => _holdingGood;
            set => _holdingGood = value;
        }

        private Goods _pickedGoods;

        public Goods PickedGoods
        {
            get => _pickedGoods;
            set => _pickedGoods = value;
        }

        private List<ShelveBase> _shelves;

        public List<ShelveBase> Shelves
        {
            get => _shelves;
            set => _shelves = value;
        }

        private int _currentLevel;

        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = value;
                SaveCurrentLevel();
            }
        }

        public List<int> GoodsArray { get; set; }
        public Dictionary<int, Dictionary<int, List<int>>> GoodsData { get; set; }

        private void Awake()
        {
            _instance = this;

            _shelves = new List<ShelveBase>();
            GoodsArray = new List<int>();
            GoodsData = new Dictionary<int, Dictionary<int, List<int>>>();
            // Deserialize level config and icon config
            LevelConfigs = JsonConvert.DeserializeObject<LevelConfigArray>(levelConfigText.text);
            IconConfigs = JsonConvert.DeserializeObject<IconConfig[]>(iconConfigText.text);
            LoadEventRegister();

            // Load current level from local storage
            LoadCurrentLevel();
        }

        private void Start()
        {
            // remove all child of level Root
            foreach (Transform child in levelRoot.transform)
            {
                Destroy(child.gameObject);
            }
            Reset();
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("KHOA TRAN - OnBeginDrag");
            if (_holdingGood == null) return;
            
            // Tính offset giữa vị trí click và vị trí holdingGood
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                levelRoot, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 localPoint
            );
            _dragOffset = (Vector2)_holdingGood.transform.localPosition - localPoint;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_holdingGood == null) return;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                levelRoot, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 localPos
            );
            
            // Áp dụng offset để giữ vị trí tương đối với chuột
            _holdingGood.transform.localPosition = localPos + _dragOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Instance._holdingGood == null) return;
            Instance.Release();
        }

        public void Pick(Goods goods, Vector2 pos)
        {
            if (IsPicking()) return;
            _pickedGoods = goods;
            _holdingGood = new GameObject();

            Image img = _holdingGood.AddComponent<Image>();
            img.sprite = goods.spriteImg.sprite;
            img.SetNativeSize();
            img.raycastTarget = false;

            // add to level root
            _holdingGood.transform.SetParent(levelRoot.transform, true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                levelRoot, // rectTransform cha
                pos, // screen pos (nếu pos đang ở tọa độ UI/screen)
                null, // Camera (null nếu Canvas = Screen Space Overlay)
                out var localPoint
            );
            RectTransform holdRect = _holdingGood.GetComponent<RectTransform>();
            holdRect.anchoredPosition = localPoint;
            holdRect.localScale = Vector3.one;
        }

        public void Release()
        {
            // var isSuccess = false;
            // var holdingGoodWorldPos = Instance._holdingGood.transform.position;
            var isSuccess = false;

            if (_holdingGood == null) return;

            var holdGoodsWorldPos = _holdingGood.transform.position;

            foreach (var shelve in _shelves)
            {
                var uiTrans = shelve.GetComponent<RectTransform>();
                var shelveWorldPos = shelve.transform.position;

                Vector2 goodsLocalPos = uiTrans.InverseTransformPoint(holdGoodsWorldPos);

                var disX = Mathf.Abs(shelveWorldPos.x - holdGoodsWorldPos.x);
                var disY = Mathf.Abs(shelveWorldPos.y - holdGoodsWorldPos.y);

                if (!(disX < uiTrans.rect.width / 2.3) || !(disY < uiTrans.rect.height / 2.3)) continue;
                var slotId = shelve.GetSlot(goodsLocalPos);

                if (slotId < 0 || shelve.IsSlotOccupied(slotId))
                    continue;

                var goodsId = _pickedGoods.Id;
                shelve.PlaceGoods(goodsId, slotId);
                isSuccess = true;
                break;
            }

            if (isSuccess)
            {
                EventManager.Instance.Emit(EventKey.PlaceGood);
            }
            else
            {
                _pickedGoods.Visible = true;
            }

            _pickedGoods = null;

            if (_holdingGood != null)
            {
                Destroy(_holdingGood.gameObject);
            }

            _holdingGood = null;
        }

        public bool IsPicking()
        {
            return _pickedGoods != null;
        }

        public void CreateStage()
        {
            // instantiate stage prefab
            var stage = Instantiate(stagePrefab, levelRoot.transform);
            stage.name = "stage";
        }

        private void LoadCurrentLevel()
        {
            int savedLevel = PlayerPrefs.GetInt("current_level", 0);

            if (savedLevel > 0)
            {
                _currentLevel = savedLevel;
            }
            else
            {
                _currentLevel = 1;
                SaveCurrentLevel();
            }
        }

        public void SaveCurrentLevel()
        {
            PlayerPrefs.SetInt("current_level", _currentLevel);
            PlayerPrefs.Save(); // Force save to disk
            Debug.Log($"Saved current level to local storage: {_currentLevel}");
        }

        public LevelConfig GetCurrentLevelConfig()
        {
            if (LevelConfigs == null) return null;

            foreach (var levelConfig in LevelConfigs.Levels)
            {
                if (levelConfig.Id == _currentLevel)
                {
                    return levelConfig;
                }
            }

            return null;
        }

        private void Reset()
        {
            _holdingGood = null;
            _shelves.Clear();
            GoodsArray.Clear();
            GoodsData.Clear();
        }
        
        private void LoadEventRegister()
        {
            EventManager.Instance.On(EventKey.StartGame, OnStartGame);
            EventManager.Instance.On(EventKey.OnStageComplete, OnStageComplete);
            EventManager.Instance.On(EventKey.NextStage, OnNextStage);
        }
        
        private void OnStartGame()
        {
            CreateStage();
        }
        
        private void OnStageComplete()
        {
            _currentLevel++;
            SaveCurrentLevel();
        }
        
        private void OnNextStage()
        {
            foreach (Transform child in levelRoot.transform)
            {
                Destroy(child.gameObject);
            }
            Reset();
            CreateStage();
        }
    }
}