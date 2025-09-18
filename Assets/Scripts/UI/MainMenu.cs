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
        
        public void OnStartButtonPressed()
        {
            // EventManager.Instance.Emit(EventKey.StartGame);
            ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.StartGame);
        }

        public void OnPlayButtonPressed()
        {
            
        }
        
    }
}