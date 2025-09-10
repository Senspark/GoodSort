using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using game;


namespace manager
{
    public class GameManager : MonoBehaviour, IDragHandler, IEndDragHandler, ICancelHandler
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

        [SerializeField] private GameObject levelRoot;
        [SerializeField] private GameObject stagePrefab;
        [SerializeField] private TextAsset levelConfigText;
        [SerializeField] private TextAsset iconConfigText;
        
        public LevelConfigArray LevelConfigs { get; private set; }
        public IconConfig[] IconConfigs { get; private set; }
        private GameObject _holdingGood;
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
            CreateStage();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            var holdingGood = Instance._holdingGood;
            if (holdingGood == null) return;
            var uiPos = eventData.position;
            uiPos.y += 10;
            Vector2 localPos;
            //rect transform is gameManager
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, uiPos, eventData.pressEventCamera, out localPos);
            holdingGood.transform.localPosition = localPos;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (Instance._holdingGood == null) return;
            Instance.Release();
        }
        
        public void OnCancel(BaseEventData eventData)
        {
            
        }
        
        public void Pick(Goods goods, Vector2 pos)
        {
            _pickedGoods = goods;
        }

        public void Release()
        {
            // var isSuccess = false;
            // var holdingGoodWorldPos = Instance._holdingGood.transform.position;
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
    }
}