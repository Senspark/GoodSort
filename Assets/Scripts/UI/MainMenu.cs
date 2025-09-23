using System;
using UnityEngine;
using manager.Interface;
using Senspark;
using UnityEngine.UI;
using Utilities;


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
            int level = int.Parse(inputField.text);
            Debug.Log($"Play level: {level}");
            _ = ServiceLocator.Instance
                .Resolve<ISceneLoader>()
                .LoadScene<GameScene>(nameof(GameScene)).Then(gameScene =>
                {
                });
        }
    }
}