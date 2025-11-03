using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIToggle : Toggle
    {
        [SerializeField] private Image onImage;
        [SerializeField] private Image offImage;

        protected override void Start()
        {
            base.Start();
            onValueChanged.AddListener(RefreshState);
        }

        private void OnDestroy()
        {
            onValueChanged.RemoveListener(RefreshState);
        }
        
        private void RefreshState(bool value)
        {
            if (onImage)
            {
                onImage.gameObject.SetActive(value);
            }
            
            if(offImage)
            {
                offImage.gameObject.SetActive(!value);
            }
        }
        
    }
}
