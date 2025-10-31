using Defines;
using Senspark;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        public AudioEnum audioEnum = AudioEnum.ClickButton;
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                ServiceLocator.Instance.Resolve<IAudioManager>().PlaySound(audioEnum, 1f);
            });
        }
    }
}