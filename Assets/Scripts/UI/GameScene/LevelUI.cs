using UnityEngine;

namespace Game
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private LevelTimeView levelTimeView;

        private float _timeAccumulator = 0f;

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
        }

        public void ResetTimer()
        {
            levelTimeView.Time = 0;
            _timeAccumulator = 0f;
        }
    }
}