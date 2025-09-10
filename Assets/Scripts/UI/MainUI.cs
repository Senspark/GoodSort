using System;
using UnityEngine;
using System.Threading.Tasks;
using Defines;
using manager;


namespace UI
{
    public class MainUI : MonoBehaviour
    {
        public void OnStartButtonPressed()
        {
            EventManager.Instance.Emit(EventKey.StartGame);
        }
        
    }
}