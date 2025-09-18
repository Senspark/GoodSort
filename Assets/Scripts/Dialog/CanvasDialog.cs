using Senspark;
using UnityEngine;

namespace Dialog
{
    [Service(nameof(IDialogCanvas))]
    public interface IDialogCanvas
    {
        public Canvas getCanvas();
    }
    public class CanvasDialog : MonoBehaviour, IDialogCanvas
    {
        [SerializeField] private Canvas canvas;

        private void Awake()
        {
            ServiceLocator.Instance.Provide(this);
        }

        public Canvas getCanvas()
        {
            return canvas;
        }
    }
}