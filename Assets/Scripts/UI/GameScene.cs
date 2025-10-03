using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Defines;
using Game;
using manager;
using manager.Interface;
using Senspark;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameScene : MonoBehaviour
    {
        [Header("Level Navigation")] //
        [SerializeField]
        private Button nextLevelButton;

        [SerializeField] private Button backLevelButton;
        [SerializeField] private Text currentLevelText;

        private IEventManager _eventManager;
        private ILevelLoaderManager _levelLoaderManager;
        private ILevelStoreManager _levelStoreManager;
        private IConfigManager _configManager;

        public GameObject holdingGoods;
        private Goods _pickedGoods;
        private LevelView _levelView;

        private const int MIN_LEVEL = 1;
        private const int MAX_LEVEL = 10;

        public int CurrentLevel { get; private set; }
        public GameStateType State { get; private set; } = GameStateType.UnInitialize;

        private void Awake()
        {
            try
            {
                GetServices();
            }
            catch (Exception e)
            {
                // missing service - re-initialize
                MockGameInitializer.Initialize()
                    .ContinueWith(() =>
                    {
                        GetServices();
                        CurrentLevel = 1;
                        Start();
                    })
                    .Forget();
            }

            return;

            void GetServices()
            {
                var services = ServiceLocator.Instance;
                _eventManager = services.Resolve<IEventManager>();
                _levelLoaderManager = services.Resolve<ILevelLoaderManager>();
                _levelStoreManager = services.Resolve<ILevelStoreManager>();
                _configManager = services.Resolve<IConfigManager>();
                State = GameStateType.Initialized;
            }
        }

        private void Start()
        {
            if (State != GameStateType.Initialized) return;
            
            SetupLevelNavigation();
            if (CurrentLevel > 0)
            {
                LoadLevel(CurrentLevel);
            }

            State = GameStateType.Loaded;
        }

        [Button]
        public void SetLevel(int level)
        {
            if (State <= GameStateType.Initialized)
            {
                CurrentLevel = level;
            }
            else
            {
                CurrentLevel = level;
                LoadLevel(level);
            }
        }

        // public void LoadLevel(int level)
        // {
        //     _currentLevel = level;
        //     StartCoroutine(LoadLevelAsync(level));
        // }

        private void SetupLevelNavigation()
        {
            // nextLevelButton.onClick.AddListener(NextLevel);
            // backLevelButton.onClick.AddListener(BackLevel);
        }

        // private void NextLevel()
        // {
        //     if (_currentLevel < MAX_LEVEL)
        //     {
        //         _currentLevel++;
        //         // set current level to local storage
        //         PlayerPrefs.SetInt("current_level", _currentLevel);
        //         PlayerPrefs.Save();
        //     }
        // }
        //
        // private void BackLevel()
        // {
        //     if (_currentLevel > MIN_LEVEL)
        //     {
        //         _currentLevel--;
        //         // set current level to local storage
        //         PlayerPrefs.SetInt("current_level", _currentLevel);
        //         PlayerPrefs.Save();
        //     }
        // }

        private void LoadLevel(int level)
        {
            CleanUp();
            var strategy = _configManager.GetValue<LevelConfig>(ConfigKey.LevelConfig).LevelStrategies[level - 1];
            var goodsConfig = _configManager.GetValue<GoodsConfig[]>(ConfigKey.GoodsConfig);
            var builder = new LevelConfigBuilder(_levelLoaderManager)
                .SetLevelStrategy(strategy)
                .SetGoodsArray(goodsConfig.ToList())
                .Build();

            var leveView = builder.LevelObject.GetComponent<LevelView>();
            leveView.Load(builder);
            leveView.transform.SetParent(transform, false);
            _levelView = leveView;
        }

        private void CleanUp()
        {
            if (!_levelView)
            {
                return;
            }

            Destroy(_levelView.gameObject);
            _levelView = null;
        }

        // public void OnPickGoods(Goods goods, Vector2 position)
        // {
        //     _pickedGoods = goods;
        //     // _pickedGoods.Visible = false;
        //
        //     holdingGoods = new GameObject("HoldingGoods");
        //     holdingGoods.transform.SetParent(transform, true);
        //     holdingGoods.transform.position = new Vector3(position.x, position.y, 0);
        //     var img = holdingGoods.AddComponent<SpriteRenderer>();
        //     img.sprite = goods.spriteIcon.sprite;
        //     img.sortingOrder = 100;
        //     
        //     var rd = holdingGoods.AddComponent<Rigidbody2D>();
        //     rd.isKinematic = true;
        //     
        //     var col = holdingGoods.AddComponent<BoxCollider2D>();
        //     col.isTrigger = true;
        // }
        //
        // public void OnMoveGoods(Vector2 position)
        // {
        //     holdingGoods.transform.position = position;
        // }

        // public void OnDropGoods()
        // {
        //     var isSuccess = false;
        //     if (!holdingGoods) return;
        //     
        //     foreach (var shelve in _levelView.Shelves)
        //     {
        //         if (!shelve.IsTargetTouched()) continue;
        //         var slotId = shelve.GetSlot(holdingGoods.transform.position);
        //
        //         if (slotId < 0 || shelve.IsSlotOccupied(slotId))
        //             continue;
        //
        //         var goodsId = _pickedGoods.Id;
        //         shelve.PlaceGoods(goodsId, slotId);
        //         isSuccess = true;
        //         break;
        //     }
        //     
        //     if(isSuccess)
        //     {
        //         ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.PlaceGood);
        //         if(_pickedGoods != null)
        //         {
        //             Destroy(_pickedGoods.gameObject);
        //             _pickedGoods = null;
        //         }
        //     }
        //     else
        //     {
        //         // _pickedGoods.Visible = true;
        //     }
        //
        //     Destroy(holdingGoods.gameObject);
        //     holdingGoods = null;
        // }
    }
}