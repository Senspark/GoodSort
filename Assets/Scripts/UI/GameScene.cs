using System.Collections;
using System.Linq;
using System.Threading.Tasks;
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
        [Header("Level Navigation")]
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button backLevelButton;
        [SerializeField] private Text currentLevelText;
        
        private readonly IEventManager _eventManager = ServiceLocator.Instance.Resolve<IEventManager>();
        private readonly ILevelLoaderManager _levelLoaderManager = ServiceLocator.Instance.Resolve<ILevelLoaderManager>();
        private readonly ILevelStoreManager _levelStoreManager = ServiceLocator.Instance.Resolve<ILevelStoreManager>();
        private readonly IConfigManager _configManager = ServiceLocator.Instance.Resolve<IConfigManager>();

        public GameObject holdingGoods;
        private Goods pickedGoods;
        private LevelView _levelView;
        
        private int _currentLevel = 1;
        private const int MIN_LEVEL = 1;
        private const int MAX_LEVEL = 10;
        
        private void Start()
        {
            // SetupLevelNavigation();
            // // get current level from local storage
            // _currentLevel = PlayerPrefs.GetInt("current_level", 1);
            // Debug.Log($"Current level: {_currentLevel}");
            // StartCoroutine(LoadLevelAsync(1));
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
            var strategy = _configManager.GetValue<LevelConfig>(ConfigKey.LevelConfig).levelStrategies[level - 1];
            var goodsConfig = _configManager.GetValue<GoodsConfig[]>(ConfigKey.GoodsConfig);
            var builder = new LevelConfigBuilder(_levelLoaderManager)
                .SetLevelStrategy(strategy)
                .SetGoodsArray(goodsConfig.ToList())
                .Build();

            var leveView = builder.LevelObject.GetComponent<LevelView>();
            var levelCallback = new LevelViewController
            {
                OnPickGoods = OnPickGoods,
                OnMoveGoods = OnMoveGoods,
                OnDropGoods = OnDropGoods
            };
            leveView.Load(builder, levelCallback);
            leveView.transform.SetParent(transform, false);
            _levelView = leveView;
        }
        
        private void OnPickGoods(Goods goods, Vector2 position)
        {
            pickedGoods = goods;
            pickedGoods.Visible = false;

            holdingGoods = new GameObject("HoldingGoods" + goods.Id);
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
        
        private void OnMoveGoods(Vector2 position)
        {
            if (holdingGoods)
            {
                Debug.LogWarning("KHOA TRAN - OnMoveGoods: holdingGoods is null");
                return;
            }
            holdingGoods.transform.position = position;
        }
        
        private void OnDropGoods()
        {
            var isSuccess = false;
            if (!holdingGoods) return;
            
            foreach (var shelve in _levelView.Shelves)
            {
                if (!shelve.IsTargetTouched()) continue;
                var slotId = shelve.GetSlot(holdingGoods.transform.position);

                if (slotId < 0 || shelve.IsSlotOccupied(slotId))
                    continue;

                var goodsId = pickedGoods.Id;
                shelve.PlaceGoods(goodsId, slotId);
                isSuccess = true;
                break;
            }
            
            if(isSuccess)
            {
                Debug.Log("KHOA TRAN - OnDropGoods: isSuccess");
                if(pickedGoods != null)
                {
                    Destroy(pickedGoods.gameObject);
                    pickedGoods = null;
                }
            }
            else
            {
                pickedGoods.Visible = true;
            }

            Destroy(holdingGoods.gameObject);
            holdingGoods = null;
        }
        
    }
}