using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Constant;
using Defines;
using Game;
using manager.Interface;
using Senspark;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameScene : MonoBehaviour
    {
        private static GameScene _sharedInstance = null;
        [Header("Level Navigation")]
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button backLevelButton;
        [SerializeField] private Text currentLevelText;
        
        private IEventManager _eventManager;
        private ILevelLoaderManager _levelLoaderManager;
        private ILevelStoreManager _levelStoreManager;
        private IConfigManager _configManager;

        public GameObject holdingGoods;
        private Goods _pickedGoods;
        private LevelView _levelView;
        
        private int _currentLevel = 1;
        private const int MIN_LEVEL = 1;
        private const int MAX_LEVEL = 10;
        
        private void Awake()
        {
            _sharedInstance = this;
            var services = ServiceLocator.Instance;
            _eventManager = services.Resolve<IEventManager>();
            _levelLoaderManager = services.Resolve<ILevelLoaderManager>();
            _levelStoreManager = services.Resolve<ILevelStoreManager>();
            _configManager = services.Resolve<IConfigManager>();
        }
        
        public static GameScene Instance
        {
            get
            {
                if (_sharedInstance == null)
                {
                   throw new System.Exception("GameScene is null");
                }
                return _sharedInstance;
            }
        }
        
        private void Start()
        {
            SetupLevelNavigation();
            StartCoroutine(LoadLevelAsync(4));
        }
        
        public void LoadLevel(int level)
        {
            _currentLevel = level;
            StartCoroutine(LoadLevelAsync(level));
        }
        
        private void SetupLevelNavigation()
        {
            nextLevelButton.onClick.AddListener(NextLevel);
            backLevelButton.onClick.AddListener(BackLevel);
        }
        
        private void NextLevel()
        {
            if (_currentLevel < MAX_LEVEL)
            {
                _currentLevel++;
                // set current level to local storage
                PlayerPrefs.SetInt("current_level", _currentLevel);
                PlayerPrefs.Save();
            }
        }
    
        private void BackLevel()
        {
            if (_currentLevel > MIN_LEVEL)
            {
                _currentLevel--;
                // set current level to local storage
                PlayerPrefs.SetInt("current_level", _currentLevel);
                PlayerPrefs.Save();
            }
        }

        private IEnumerator LoadLevelAsync(int level)
        {
            yield return new WaitForEndOfFrame();
            var levelLoaderManager = ServiceLocator.Instance.Resolve<ILevelLoaderManager>();
            var configManager = ServiceLocator.Instance.Resolve<IConfigManager>();
            var strategy = configManager.GetValue<LevelConfig>(ConfigKey.LevelConfig).levelStrategies[level - 1];
            var goodsConfig = configManager.GetValue<GoodsConfig[]>(ConfigKey.GoodsConfig);
            var builder = new LevelConfigBuilder(levelLoaderManager)
                .SetLevelStrategy(strategy)
                .SetGoodsArray(goodsConfig.ToList())
                .Build();

            var leveView = builder.LevelObject.GetComponent<LevelView>();
            leveView.Load(builder);
            leveView.transform.SetParent(transform, false);
            _levelView = leveView;
        }
        
        public void OnPickGoods(Goods goods, Vector2 position)
        {
            _pickedGoods = goods;
            _pickedGoods.Visible = false;

            holdingGoods = new GameObject("HoldingGoods");
            holdingGoods.transform.SetParent(transform, true);
            holdingGoods.transform.position = new Vector3(position.x, position.y, 0);
            var img = holdingGoods.AddComponent<SpriteRenderer>();
            img.sprite = goods.spriteIcon.sprite;
            img.sortingOrder = 100;
            
            var rd = holdingGoods.AddComponent<Rigidbody2D>();
            rd.isKinematic = true;
            
            var col = holdingGoods.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
        
        public void OnMoveGoods(Vector2 position)
        {
            holdingGoods.transform.position = position;
        }
        
        public void OnDropGoods()
        {
            var isSuccess = false;
            if (!holdingGoods) return;
            
            foreach (var shelve in _levelView.Shelves)
            {
                if (!shelve.IsTargetTouched()) continue;
                var slotId = shelve.GetSlot(holdingGoods.transform.position);

                if (slotId < 0 || shelve.IsSlotOccupied(slotId))
                    continue;

                var goodsId = _pickedGoods.Id;
                shelve.PlaceGoods(goodsId, slotId);
                isSuccess = true;
                break;
            }
            
            if(isSuccess)
            {
                ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.PlaceGood);
                if(_pickedGoods != null)
                {
                    Destroy(_pickedGoods.gameObject);
                    _pickedGoods = null;
                }
            }
            else
            {
                _pickedGoods.Visible = true;
            }

            Destroy(holdingGoods.gameObject);
            holdingGoods = null;
        }
        
    }
}