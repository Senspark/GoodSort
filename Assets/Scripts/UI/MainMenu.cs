using System;
using UnityEngine;
using manager.Interface;
using Senspark;
using Utilities;


namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private Transform playButton;

        public void OnPlayButtonPressed()
        {
            _ = ServiceLocator.Instance
                .Resolve<ISceneLoader>()
                .LoadScene<GameScene>(nameof(GameScene));
        }
    }
}