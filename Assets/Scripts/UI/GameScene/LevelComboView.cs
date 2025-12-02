using Defines;
using Senspark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LevelComboView : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private TextMeshProUGUI scoreText;

        private float _speed = 1f;

        private int _combo;

        public int Combo
        {
            get => _combo;
            set
            {
                _combo = value;
                UpdateDisplay();
                if (_combo <= 0) return;
                PlaySoundCombo(_combo);
                var duration = GetDuration(value);
                _speed = 1f / (duration); // 60 FPS
                progressBar.value = 1;
            }
        }

        private int _score;
        private int Score
        {
            get => _score;
            set
            {
                _score = value;
                UpdateDisplay();
            }
        }

        private void Awake()
        {
            Combo = 0;
            Score = 0;
        }

        public void Step(float dt)
        {
            if (Combo <= 0) return;
            progressBar.value = Mathf.Clamp01(progressBar.value - dt * _speed);
            if (progressBar.value <= 0)
            {
                Combo = 0;
            }
        }

        public void AddScore()
        {
            Score += GetScore();
        }

        public void IncreaseCombo()
        {
            Combo++;
        }

        private void UpdateDisplay()
        {
            comboText.text = _combo <= 0 ? "" : $"Combo x{_combo}";
            scoreText.text = $"{_score}";
        }

        // Helper function to get duration
        private float GetDuration(int combo)
        {
            const float t0 = 25;
            const float decayRate = 0.4f;
            return 3 + (t0 - 3) / (1 + Mathf.Exp(decayRate * (combo - 9)));
        }

        public int GetScore()
        {
            return (_combo - 1) / 3 + 1;
        }

        private void PlaySoundCombo(int combo)
        {
            // play sound with pitch
            var audioMgr = ServiceLocator.Instance.Resolve<IAudioManager>();
            // audioMgr.PlaySoundOnPitch(AudioEnum.Match, Mathf.Clamp(1f + combo * 0.1f, 1f, 2f) );
        }
    }
}