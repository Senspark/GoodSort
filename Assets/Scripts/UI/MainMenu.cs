using System;
using UnityEngine;
using manager.Interface;
using Senspark;
using UnityEngine.UI;
using Utilities;
using manager.Interface;


namespace UI
{
    public class MainMenu : MonoBehaviour

    {
    [SerializeField] private Canvas canvasDialog;
    [SerializeField] private Transform playButton;
    [SerializeField] private InputField inputField;

    public void OnPlayButtonPressed()
    {
        // get level from input field
        _ = ServiceLocator.Instance
            .Resolve<ISceneLoader>()
            .LoadScene<GameScene>(nameof(GameScene)).Then(gameScene =>
            {
            });
    }
    }
}