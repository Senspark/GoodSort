using System;
using UnityEngine;

namespace Dialog
{
    public class QuitLevelDialog : Dialog<QuitLevelDialog>
    {
        [SerializeField] private GameObject[] remindNodes;
        private int _currentRemindIndex = 0;
        private Action _onQuitGameScene;

        public void SetActions(Action leaveGameAction)
        {
            _onQuitGameScene = leaveGameAction;
        }

        public void OnClickQuitButton()
        {
            if (_currentRemindIndex == remindNodes.Length - 1)
            {
                // Go to menu
                _onQuitGameScene?.Invoke();
            }
            else
            {
                // Show next remind
                _currentRemindIndex++;
                ShowNextRemind();
            }
        }
        
        private void ShowNextRemind()
        {
            for (int i = 0; i < remindNodes.Length; i++)
            {
                remindNodes[i].SetActive(i == _currentRemindIndex);
            }
        }
    }
}