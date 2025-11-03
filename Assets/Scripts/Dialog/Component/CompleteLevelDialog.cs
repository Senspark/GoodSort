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
            OnWillShow(() =>
            {
                _controller.PlayEffect(AudioEnum.LevelComplete);
            });
            base.Show(canvas);
        }
        
    }
}