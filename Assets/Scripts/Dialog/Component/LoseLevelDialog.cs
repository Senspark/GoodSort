using Cysharp.Threading.Tasks;
using Dialog.Controller;
using Factory;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Dialog
{
    public class LoseLevelDialog : Dialog<LoseLevelDialog>
    {
        [SerializeField] private Button btnPlayAgain;

        private ILoseLevelDialogController _controller;

        protected override void Awake()
        {
            base.Awake();
            btnPlayAgain?.onClick.AddListener(OnPlayAgainClicked);
        }

        public override void Show(Canvas canvas)
        {
            IgnoreOutsideClick = true;
            base.Show(canvas);
        }

        public void OnExitClicked()
        {
            // _controller.BackToMenu();
            SceneUtils.LoadScene("MainMenu");
        }

        private void OnPlayAgainClicked()
        {
            if (_controller.HasLives())
            {
                _controller.UseLiveAndRestart();
                Hide();
            }
            else
            {
                ShowOutOfLivesDialog();
            }
        }
        
        private void ShowOutOfLivesDialog()
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Dialog/OutOfLiveDialog")
                .ContinueWith(prefab =>
                {
                    var dialog = UIControllerFactory.Instance.Instantiate<OutOfLivesDialog>(prefab);
                    dialog.Show(Canvas);
                });
        }
    }
}

