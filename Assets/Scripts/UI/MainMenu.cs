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
            var dataManager = ServiceLocator.Instance.Resolve<IDataManager>();
            var coin = dataManager.Get("Coin", 0);
            textCoin.text = $"{coin}";
        }
        
        private void UpdateLevel()
        {
            var levelManager = ServiceLocator.Instance.Resolve<ILevelManager>();
            textCurrentLevel.text = $"Level {levelManager.GetCurrentLevel()}";
        }
        

        public void OnClickPlayButton()
        {
            OpenSelectLevelDialog().Forget();
        }
        
        public void OnClickSettingButton()
        {
            OpenSettingDialog().Forget();
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
                level = Mathf.Clamp(parsed, 1, 20);
            }

            var dataManager = ServiceLocator.Instance.Resolve<IDataManager>();
            dataManager.Set("CurrentLevel", level);
            UpdateLevel();
        }

        private async UniTaskVoid OpenSelectLevelDialog()
        {
            var levelManager = ServiceLocator.Instance.Resolve<ILevelManager>();
            levelManager.GetCurrentLevel();
            var selectLevelDialogPrefab = await PrefabUtils.LoadPrefab("Prefabs/Dialog/SelectLevelDialog");
            var dialog = UIControllerFactory.Instance.Instantiate<SelectLevelDialog>(selectLevelDialogPrefab);
            dialog.SetCurrentLevel(levelManager.GetCurrentLevel())
                .Show(canvasDialog);
        }

        private async UniTaskVoid OpenSettingDialog()
        {
            var dialogPrefab = await PrefabUtils.LoadPrefab("Prefabs/Dialog/SettingDialog");
            var dialog = UIControllerFactory.Instance.Instantiate<SettingDialog>(dialogPrefab);
            dialog.Show(canvasDialog);
        }
    }
}