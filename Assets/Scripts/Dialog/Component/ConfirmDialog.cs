using System;
using UnityEngine;

namespace Dialog
{
    public class ConfirmDialog : Dialog<ConfirmDialog>
    {
        [SerializeField] private GameObject[] remindPanels;
        private int _currentRemindIndex = 0;

        private Action _confirmYes;
        private Action _confirmNo;

        public void SetActions(Action confirmYes, Action confirmNo)
        {
            _confirmYes = confirmYes;
            _confirmNo = confirmNo;
        }

        public override void Show(Canvas canvas)
        {
            _currentRemindIndex = 0;
            ShowCurrent();
            base.Show(canvas);
        }

        // YES: advance through panels; on last, fire confirmYes
        public void OnClickYesButton()
        {
            if (remindPanels == null || remindPanels.Length == 0)
            {
                _confirmYes?.Invoke();
                return;
            }

            if (_currentRemindIndex >= remindPanels.Length - 1)
            {
                _confirmYes?.Invoke();
            }
            else
            {
                _currentRemindIndex++;
                ShowCurrent();
            }
        }

        // NO: immediately fire confirmNo
        public void OnClickNoButton()
        {
            _confirmNo?.Invoke();
        }

        private void ShowCurrent()
        {
            if (remindPanels == null) return;
            for (int i = 0; i < remindPanels.Length; i++)
            {
                if (remindPanels[i] != null)
                    remindPanels[i].SetActive(i == _currentRemindIndex);
            }
        }
    }
}

