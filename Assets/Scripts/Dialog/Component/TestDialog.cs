using Dialog.Controller;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class TestDialog : Dialog<TestDialog>
    {
        [SerializeField] private Button testButton;
        [SerializeField] private Text testText;

        private ITestDialogController _controller;

        protected override void Awake()
        {
            base.Awake();
            testText.text = "DANGKHOA";
        }
    }
}