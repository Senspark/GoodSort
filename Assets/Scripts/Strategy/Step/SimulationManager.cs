using System;

namespace Strategy.Step
{
    public interface IStepStrategy
    {
        void Reset();
        void Step(double dt);
    }
    
    public class FullStepStrategy : IStepStrategy
    {
        private double _accumulatedDelta = 0;
        private readonly double _minStepDuration;
        private readonly double _maxStepDuration;
        private readonly Action<double> _callback;

        public FullStepStrategy(
            double minStepDuration, 
            double maxStepDuration, 
            Action<double> callback)
        {
            _minStepDuration = minStepDuration;
            _maxStepDuration = maxStepDuration;
            _callback = callback;
        }

        public void Reset()
        {
            _accumulatedDelta = 0;
        }

        public void Step(double delta)
        {
            _accumulatedDelta += delta;

            while (_accumulatedDelta >= _maxStepDuration)
            {
                _accumulatedDelta -= _maxStepDuration;
                _callback?.Invoke(_maxStepDuration);
            }

            if (_accumulatedDelta >= _minStepDuration)
            {
                _callback?.Invoke(_accumulatedDelta);
                _accumulatedDelta = 0;
            }
        }
    }
    
    public class SimulationManager
    {
        private readonly IStepStrategy _strategy;
        public bool Paused = false;
        
        private readonly float _speed = 1;
        
        public SimulationManager(IStepStrategy strategy)
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
            _strategy.Step(delta * _speed);
            return delta * _speed;
        }
    }
}