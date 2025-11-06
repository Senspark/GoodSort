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
        
        [Header("Cheat")]
        [SerializeField] private TMP_InputField cheatInputField;
        [SerializeField] private Button cheatButton;

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
                level = Mathf.Clamp(parsed, 1, 14);
            }

            var dataManager = ServiceLocator.Instance.Resolve<IDataManager>();
            dataManager.Set("CurrentLevel", level);
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