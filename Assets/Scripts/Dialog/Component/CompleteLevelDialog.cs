using Defines;
using Dialog.Controller;
using UnityEngine;

namespace Dialog
{
    public class CompleteLevelDialog : Dialog<CompleteLevelDialog>
    {
        private ICompleteLevelDialogController _controller;
        
        public void OnClickNextLevelButton()
        {
            _controller.OnNextLevelButtonPressed();
        }
        
        public override void Show(Canvas canvas)
        {
            OnDidHide(() =>
            {
                _controller.PlayEffect(AudioEnum.CloseDialog);
            });
            base.Show(canvas);
        }
        
    }
}