namespace Strategy.Level
{
    public class LevelAnimationStepAddScore : ILevelAnimationStep
    {
        private readonly LevelAnimationSwitchStateControl _control;
        private readonly LevelDataManager _levelDataManager;
        private readonly InputData _inputData;
        
        public LevelAnimationStepAddScore(
            LevelAnimationSwitchStateControl control,
            InputData inputData,
            LevelDataManager levelDataManager
        )
        {
            _control = control;
            _inputData = inputData;
            _levelDataManager = levelDataManager;
        }
        
        public void Enter()
        {
        }

        public void Update(float dt)
        {
        }

        public void Exit()
        {
        }
        
        public class InputData
        {
            public readonly int ShelfId;
            public readonly int LayerId;

            public InputData(int shelfId, int layerId)
            {
                ShelfId = shelfId;
                LayerId = layerId;
            }
        }
    }
}