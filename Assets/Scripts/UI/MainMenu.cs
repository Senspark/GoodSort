using Dialog;
using Factory;
using manager.Interface;
using UnityEngine;
using Senspark;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private GameObject selectLevelDialogPrefab;

        public void OnPlayButtonPressed()
        {
            var level = ServiceLocator.Instance.Resolve<ILevelManager>().GetCurrentLevel();
            var dialog = UIControllerFactory.Instance.Instantiate<SelectLevelDialog>(selectLevelDialogPrefab);
            dialog.SetCurrentLevel(level)
                .Show(canvasDialog);
        }
    }
}