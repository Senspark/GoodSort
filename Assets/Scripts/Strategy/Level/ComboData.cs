using UnityEngine;

namespace Strategy.Level
{
    public class ComboData
    {
        private int _combo;
        private int _score;
        private float _remainingTime;

        public int Combo
        {
            get => _combo;
            set => _combo = value;
        }

        public int Score => _score;
        public float RemainingTime => _remainingTime;
        public bool IsActive => _combo > 0 && _remainingTime > 0;

        public void IncreaseCombo()
        {
            _combo++;
            _remainingTime = GetDuration(_combo);
        }

        public void AddScore()
        {
            _score += CalculateScore();
        }

        public int CalculateScore()
        {
            return (_combo - 1) / 3 + 1;
        }

        public void Step(float dt)
        {
            if (_combo <= 0) return;
            _remainingTime -= dt;
            if (_remainingTime <= 0)
            {
                _combo = 0;
                _remainingTime = 0;
            }
        }

        public float GetProgress()
        {
            if (_combo <= 0) return 0;
            return Mathf.Clamp01(_remainingTime / GetDuration(_combo));
        }

        private float GetDuration(int combo)
        {
            const float t0 = 25;
            const float decayRate = 0.4f;
            return 3 + (t0 - 3) / (1 + Mathf.Exp(decayRate * (combo - 9)));
        }
    }
}