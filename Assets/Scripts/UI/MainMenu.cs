using UnityEngine;
using Constant;
using manager.Interface;
using Senspark;


namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private Transform playButton;
        
        public void OnPlayButtonPressed()
        {
            // change to game scene
            ServiceLocator.Instance.Resolve<ISceneLoader>().LoadScene<GameScene>(nameof(GameScene));
        }
        
    }
}