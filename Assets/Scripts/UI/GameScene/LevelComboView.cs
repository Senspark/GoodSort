using TMPro;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LevelComboView : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI comboText;

        private float speed = 1f;
        
        private int _combo;
        public int Combo
        {
            get => _combo;
            set
            {
                _combo = value;
                gameObject.SetActive(_combo > 0);
                if (_combo <= 0) return;
                UpdateDisplay();
                var duration = GetDuration(value);
                speed = 1f / (duration * 60);     // 60 FPS
                progressBar.value = 1;
            }
        }
        
        public void Step(float dt)
        {
            if (Combo <= 0) return;
            progressBar.value = Mathf.Clamp01(progressBar.value - dt * speed);
            if (progressBar.value <= 0)
            {
                Combo = 0;
            }
        }

        private void UpdateDisplay()
        {
            comboText.text = $"Combo x{_combo}";
        }
        
        // Helper function to get duration
        private float GetDuration(int combo)
        {
            const float t0 = 25;
            const float decayRate = 0.4f;
            return 3 + (t0 - 3)/(1 + Mathf.Exp(decayRate * (combo - 9)));
        }
    }
}