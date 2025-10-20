namespace Strategy.Level
{
    public class LevelAnimationUnlockShelves : ILevelAnimationStep
    {
        private readonly LevelAnimationSwitchStateControl _control;
        private readonly LevelDataManager _levelDataManager;
        private readonly InputData _inputData;

        public LevelAnimationUnlockShelves(
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
            var shelves = _levelDataManager.GetShelves();
            foreach (var shelf in shelves)
            {
                for (var i = 0; i < _inputData.MergeCompletedAmount; i++)
                {
                    shelf.DecreaseLockCount();
                }
            }

            _control.ToDragDrop();
        }

        public void Exit()
        {
        }

        public class InputData
        {
            /* Số lượng đã merge thành công */
            public readonly int MergeCompletedAmount;

            public InputData(int mergeCompletedAmount)
            {
                MergeCompletedAmount = mergeCompletedAmount;
            }
        }
    }
}