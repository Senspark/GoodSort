using Cysharp.Threading.Tasks;
using Defines;
using Dialog;
using Factory;
using UnityEngine;
using manager.Interface;
using Senspark;
using TMPro;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    [SceneMusic(AudioEnum.MenuMusic)]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private TMP_Text textCoin;
        [SerializeField] private TMP_Text textCurrentLevel;
        
        [Header("Cheat")]
        [SerializeField] private TMP_InputField cheatLevelInputField;
        [SerializeField] private TMP_InputField cheatCoinInputField;
        
        private void Start()
        {
            UpdateUI();
            CheckAndShowStarChest();
        }

        private void UpdateUI()
        {
            UpdateCoinBar();
            UpdateLevel();
        }

        private void CheckAndShowStarChest()
        {
            var currencyManager = ServiceLocator.Instance.Resolve<ICurrencyManager>();
            if (currencyManager.GetPendingChests() > 0)
            {
                ShowStarChestDialog();
            }
        }

        private void ShowStarChestDialog()
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Dialog/StarChestDialog")
                .ContinueWith(prefab =>
                {
                    var dialog = UIControllerFactory.Instance.Instantiate<StarChestDialog>(prefab);
                    dialog.OnClaimed(() =>
                    {
                        UpdateCoinBar();
                        CheckAndShowStarChest();
                    });
                    dialog.Show(canvasDialog);
                });
        }
        
        private void UpdateCoinBar()
        {
            var currencyManager = ServiceLocator.Instance.Resolve<ICurrencyManager>();
            var coin = currencyManager.GetCoins();
            textCoin.text = $"{coin}";
        }
        
        private void UpdateLevel()
        {
            var levelManager = ServiceLocator.Instance.Resolve<ILevelManager>();
            textCurrentLevel.text = $"Level {levelManager.GetCurrentLevel()}";
        }
        

        public void OnClickPlayButton()
        {
            var levelManager = ServiceLocator.Instance.Resolve<ILevelManager>();
            _ = PrefabUtils.LoadPrefab("Prefabs/Dialog/SelectLevelDialog")
                .ContinueWith(prefab =>
                {
                    var dialog = UIControllerFactory.Instance.Instantiate<SelectLevelDialog>(prefab);
                    dialog.SetCurrentLevel(levelManager.GetCurrentLevel())
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
        
        public void OnCheatLevelPressed()
        {
            int level;
            var text = cheatLevelInputField.text;
            if (string.IsNullOrWhiteSpace(text) || !int.TryParse(text, out var parsed))
            {
                level = 1;
            }
            else
            {
                level = Mathf.Clamp(parsed, 1, 71);
            }

            var dataManager = ServiceLocator.Instance.Resolve<IDataManager>();
            dataManager.Set("CurrentLevel", level);
            UpdateLevel();
        }
        
        public void OnCheatCoinPressed()
        {
            int coin;
            var text = cheatCoinInputField.text;
            if (string.IsNullOrWhiteSpace(text) || !int.TryParse(text, out var parsed))
            {
                coin = 0;
            }
            else
            {
                coin = Mathf.Clamp(parsed, 0, int.MaxValue);
            }

            var currencyManager = ServiceLocator.Instance.Resolve<ICurrencyManager>();
            currencyManager.AddCoins(coin);
            UpdateCoinBar();
        }
    }
}