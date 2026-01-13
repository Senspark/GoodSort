namespace Strategy.Step
{
    public interface StepStrategy
    {
        void Reset();
        void Step(float dt);
    }
    
    public class SimulationManager
    {
        private readonly StepStrategy _strategy;
        public bool Paused { get; set; }
        public float Speed { get; set; }
        public bool PickUpOn { get; set; }
        
        public SimulationManager(StepStrategy strategy)
        {
            _strategy = strategy;
        }
        
        public void Reset()
        {
            _strategy.Reset();
        }

        public float Step(float delta)
        {
            if(Paused) return 0f;
            _strategy.Step(delta * Speed);
            return delta * Speed;
        }
    }
}