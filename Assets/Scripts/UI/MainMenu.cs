using UnityEngine;
using Constant;
using manager.Interface;
using Senspark;


namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        
        public void OnStartButtonPressed()
        {
            // EventManager.Instance.Emit(EventKey.StartGame);
            ServiceLocator.Instance.Resolve<IEventManager>().Invoke(EventKey.StartGame);
        }
        
    }
}