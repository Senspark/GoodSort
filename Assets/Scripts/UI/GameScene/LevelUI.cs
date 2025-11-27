using TMPro;
using UnityEngine;

namespace Game
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private LevelTimeView levelTimeView;
        [SerializeField] private LevelComboView levelComboView;
        [SerializeField] private RectTransform scoreBar;
        [SerializeField] private RectTransform starPosition;
        private float _timeAccumulator;

        public void SetMaxTime(int maxTime)
        {
            levelTimeView.MaxTime = maxTime;
        }
        
        //get combo
        public int GetScore() => levelComboView.GetScore();
        public int GetCombo() => levelComboView.Combo;

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
        
        public void AddScore() => levelComboView.AddScore();
        
        public void IncreaseCombo() => levelComboView.IncreaseCombo();

        public bool LevelTimeOut()
        {
            return levelTimeView.Time >= levelTimeView.MaxTime;
        }

        public RectTransform GetComboViewTransform() => scoreBar;
        public RectTransform GetStarPosition() => starPosition;
    }
}