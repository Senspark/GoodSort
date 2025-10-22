using Dialog;
using Factory;
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
            // Resolve factory from ServiceLocator (initialized in SplashScene)
            var factory = ServiceLocator.Instance.Resolve<IUIControllerFactory>();

            // Instantiate dialog with automatic controller injection
            var dialog = factory.Instantiate<SelectLevelDialog>(selectLevelDialogPrefab);

            // Show dialog
            dialog.Show(canvasDialog);
        }
    }
}