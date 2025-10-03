using UnityEngine;
using manager.Interface;
using Senspark;
using TMPro;
using Utilities;

namespace UI
{
    public class MainMenu : MonoBehaviour

    {
    [SerializeField] private Canvas canvasDialog;
    [SerializeField] private Transform playButton;
    [SerializeField] private TMP_InputField tmpInputField;

    public void OnPlayButtonPressed()
    {
        // get level from input field
        _ = ServiceLocator.Instance
            .Resolve<ISceneLoader>()
            .LoadScene<GameScene>(nameof(GameScene)).Then(gameScene =>
            {
                gameScene.SetLevel(int.Parse(tmpInputField.text));
            });
    }
    }
}