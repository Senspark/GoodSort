using Dialog.Controller;
using TMPro;
using UnityEngine;

namespace  Dialog
{
    public class SelectLevelDialog : Dialog<SelectLevelDialog>
    {
        [SerializeField] private TextMeshProUGUI textLevel;
        private ISelectLevelDialogController _controller;
        private int _currentLevel;

        public SelectLevelDialog SetCurrentLevel(int level)
        {
            _currentLevel = level;
            textLevel.text = $"Level {_currentLevel}";
            return this;
        }
        
        public void OnClickLevelButton()
        {
            _controller.OpenLevel(_currentLevel);
        }
        
    }

}
