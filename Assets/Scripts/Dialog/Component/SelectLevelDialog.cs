using Defines;
using Dialog.Controller;
using TMPro;
using UnityEngine;

namespace  Dialog
{
    public class SelectLevelDialog : Dialog<SelectLevelDialog>
    {
        [SerializeField] private RectTransform panel;
        [SerializeField] private TextMeshProUGUI textLevel;
        private ISelectLevelDialogController _controller;
        private int _currentLevel;
        
        public SelectLevelDialog SetCurrentLevel(int level)
        {
            _currentLevel = level;
            textLevel.text = $"Level {_currentLevel}";
            return this;
        }

        public void ActiveLostStreakPanel()
        {
            panel.GetChild(1).gameObject.SetActive(false);
            panel.GetChild(2).gameObject.SetActive(true);
        }
        
        public void OnClickLevelButton()
        {
            _controller.OpenGameScene();
        }
    }

}
