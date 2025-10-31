using Defines;
using Dialog;
using Factory;
using UnityEngine;
using manager.Interface;
using Senspark;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private GameObject selectLevelDialogPrefab;
        
        [Header("Cheat")]
        [SerializeField] private TMP_InputField cheatInputField;
        [SerializeField] private Button cheatButton;

        public void OnPlayButtonPressed()
        {
            var levelManager = ServiceLocator.Instance.Resolve<ILevelManager>();
            levelManager.GetCurrentLevel();
            var dialog = UIControllerFactory.Instance.Instantiate<SelectLevelDialog>(selectLevelDialogPrefab);
            dialog.SetCurrentLevel(levelManager.GetCurrentLevel())
                .Show(canvasDialog);
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
    }
}