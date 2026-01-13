using Game;
using UI;

namespace Strategy.Level
{
    public class LevelAnimationStepAddStar : ILevelAnimationStep
    {
        private readonly LevelAnimationSwitchStateControl _control;
        private readonly LevelDataManager _levelDataManager;
        private readonly InputData _inputData;
        private readonly GameController _gameSceneController;
        
        public LevelAnimationStepAddStar(
            LevelAnimationSwitchStateControl control,
            InputData inputData,
            LevelDataManager levelDataManager,
            GameController gameSceneController
        )
        {
            _control = control;
            _inputData = inputData;
            _levelDataManager = levelDataManager;
            _gameSceneController = gameSceneController;
        }
        
        public void Enter()
        {
        }

        public void Update(float dt)
        {
            var shelf = _levelDataManager.GetShelf(_inputData.ShelfId) as ShelfBase;
            if (shelf) _gameSceneController.AddStar(0, shelf.transform.position);
            _control.ToDragDrop();
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