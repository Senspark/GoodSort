using Cysharp.Threading.Tasks;
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
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private TMP_Text textCoin;
        [SerializeField] private TMP_Text textCurrentLevel;
        
        [Header("Cheat")]
        [SerializeField] private TMP_InputField cheatInputField;
        [SerializeField] private Button cheatButton;
        
        private void Start()
        {
            // get coin from data manager
            UpdateUI();
        }

        private void UpdateUI()
        {
            // TODO: Refactor using observer
            UpdateCoinBar();
            UpdateLevel();
        }
        
        private void UpdateCoinBar()
        {
            var profileManager = ServiceLocator.Instance.Resolve<IProfileManager>();
            var coin = profileManager.GetCoins();
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
        
        public void OnCheatButtonPressed()
        {
            var text = cheatInputField != null ? cheatInputField.text : null;
            int level;
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
    }
}