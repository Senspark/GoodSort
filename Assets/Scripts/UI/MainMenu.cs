using Cysharp.Threading.Tasks;
using Defines;
using Dialog;
using Factory;
using UnityEngine;
using manager.Interface;
using Senspark;
using TMPro;
using Utilities;

namespace UI
{
    [SceneMusic(AudioEnum.MenuMusic)]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private ResourceBar coinBar;
        [SerializeField] private ResourceBar starBar;
        [SerializeField] private TMP_Text textCurrentLevel;

        private ILevelManager _levelManager;
        private IStoreManager _storeManager;

        private void Awake()
        {
            GetServices();
            RegisterObserver();
        }
        
        private void GetServices()
        {
            _levelManager = ServiceLocator.Instance.Resolve<ILevelManager>();
            _storeManager = ServiceLocator.Instance.Resolve<IStoreManager>();
        }
        
        private void RegisterObserver()
        {
            _storeManager.AddObserver(new StoreManagerObserver
            {
                OnCoinsChanged = UpdateCoinBar,
                OnStarsChanged = UpdateStarBar
            });
        }

        private void Start()
        {
            UpdateUI();
            CheckAndShowStarChest();
        }

        private void UpdateUI()
        {
            UpdateCoinBar();
            UpdateStarBar();
            UpdateLevel();
        }

        private void CheckAndShowStarChest()
        {
            if (_storeManager.GetPendingChests() > 0)
            {
                ShowStarChestDialog();
            }
        }

        public void ShowStarChestDialog()
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Dialog/StarChestDialog")
                .ContinueWith(prefab =>
                {
                    var dialog = UIControllerFactory.Instance.Instantiate<StarChestDialog>(prefab);
                    dialog.OnClaimed(UpdateCoinBar);
                    dialog.Show(canvasDialog);
                });
        }
        
        private void UpdateCoinBar()
        {
            var coin = _storeManager.GetCoins();
            coinBar.Value = coin;
        }
        
        private void UpdateStarBar()
        {
            var star = _storeManager.GetTotalStars();
            starBar.Value = star;
            starBar.MaxValue = 800;
        }
        
        private void UpdateLevel()
        {
            textCurrentLevel.text = $"Level {_levelManager.GetCurrentLevel()}";
        }
        

        public void OnClickPlayButton()
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Dialog/SelectLevelDialog")
                .ContinueWith(prefab =>
                {
                    var dialog = UIControllerFactory.Instance.Instantiate<SelectLevelDialog>(prefab);
                    dialog.SetCurrentLevel(_levelManager.GetCurrentLevel())
                        .Show(canvasDialog);
                });
        }
        
        public void OnClickSettingButton()
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Dialog/SettingDialog")
                .ContinueWith(prefab =>
                {
                    var dialog = UIControllerFactory.Instance.Instantiate<SettingDialog>(prefab);
                    dialog.Show(canvasDialog);
                });
        }
        
        public void OnClickShopButton()
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Dialog/ShopDialog")
                .ContinueWith(prefab =>
                {
                    var dialog = UIControllerFactory.Instance.Instantiate<ShopDialog>(prefab);
                    dialog.Show(canvasDialog);
                });
        }

        public void OnClickProfile()
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Dialog/ProfileDialog")
                .ContinueWith(prefab =>
                {
                    var dialog = UIControllerFactory.Instance.Instantiate<ProfileDialog>(prefab);
                    dialog.Show(canvasDialog);
                });
        }
    
    }
}