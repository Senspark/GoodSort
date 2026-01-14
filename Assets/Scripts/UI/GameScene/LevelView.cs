using Strategy.Level;
using Strategy.Step;
using UnityEngine;

namespace Game
{
    public enum LevelStatus
    {
        Loading,
        Playing,
        Failed,  
        Completed,
    }
    
    public class LevelView : MonoBehaviour
    {
        [SerializeField] LevelTimeView levelTimeView;
        [SerializeField] LevelComboView levelComboView;
        // private ILevelProgress _levelProgress;
        private ComboData _comboData;

        private void Awake()
        {
            var step = new FullStepStrategy(0.001, 1 / 60f, _ => { });
            SimulationManager = new SimulationManager(step);
            Status = LevelStatus.Loading;
            _comboData = new ComboData();
            levelComboView.SetComboData(_comboData);
        }
        
        // Update
        private void Update()
        {
            if (Status != LevelStatus.Playing) return;
            var dt = SimulationManager.Step(Time.deltaTime);
            levelTimeView.Time += dt;
            _comboData.Step(dt);
        }
        
        public SimulationManager SimulationManager { get; private set; }
        public LevelStatus Status { get; set; } = LevelStatus.Loading;
        public ComboData ComboData => _comboData;
        public void SetConfig(int duration, Booster.Booster[] boosters)
        {
            levelTimeView.MaxTime = duration;
            Debug.Log("Boosters: " + boosters);
        }
    }
}