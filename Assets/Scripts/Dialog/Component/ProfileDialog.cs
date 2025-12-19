using Dialog.Controller;
using UnityEngine;

namespace Dialog
{
    public class ProfileDialog : Dialog<ProfileDialog>
    {
        public override void Show(Canvas canvas)
        {
            OnWillShow(() =>
            {
                UpdateUI();
            });
            base.Show(canvas);
        }

        private void UpdateUI()
        {
            
        }
    }
}