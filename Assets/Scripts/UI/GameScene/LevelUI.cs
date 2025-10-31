using TMPro;
using UnityEngine;

namespace Game
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private LevelTimeView levelTimeView;
        [SerializeField] private LevelComboView levelComboView;
        
        private float _timeAccumulator;

        public void SetMaxTime(int maxTime)
        {
            levelTimeView.MaxTime = maxTime;
        }

        public void Step(float dt)
        {
            _timeAccumulator += dt;

            if (_timeAccumulator >= 1f)
            {
                levelTimeView.Time += 1;
                _timeAccumulator -= 1f;
            }
            levelComboView.Step(dt);
        }
        
        public void AddScore()
        {
            levelComboView.AddScore();
        }
    }
}