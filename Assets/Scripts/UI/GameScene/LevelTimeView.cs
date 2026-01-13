using TMPro;
using UnityEngine;
using Utilities;
using Math = System.Math;

namespace Game
{
    public class LevelTimeView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;

        private float _time;
        private int _maxTime;
        private bool _dirty;
        
        private void Awake()
        {
            UpdateDisplay();
        }
        
        public float Time
        {
            get => _time;
            set
            {
                _time = value;
                _dirty = true;
            }
        }
        
        public int MaxTime
        {
            get => _maxTime;
            set
            {
                if (_maxTime == value) return;
                _maxTime = value;
                _dirty = true;
            }
        }
        
        private void Update()
        {
            if (!_dirty) return;
            _dirty = false;
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            var remainingTime = Math.Max(_maxTime - _time, 0);
            timeText.text = TimeUtils.FormatTime((int)remainingTime, "mm:ss");
        }
    }
}