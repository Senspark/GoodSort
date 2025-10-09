using System;
using Core;
using JetBrains.Annotations;

namespace Strategy.Level
{
    public enum LevelAnimationStepType
    {
        DragDrop, // Cho phép user control
        MergeLayer, // lock control để diễn hình ảnh
    }

    /* Điều khiển chuyển động/hiệu ứng/hoạt ảnh trong Game Scene
     Chia làm nhiều Step, mỗi Step chỉ làm 1 nhiệm vụ
     */
    public class LevelAnimation
    {
        private readonly LevelDataManager _levelDataManager;
        private readonly IDragDropGameManager _dragDropManager;

        private readonly LevelAnimationStepDragDrop _stateDragDrop;
        private readonly LevelAnimationSwitchStateControl _stateControl;
        [CanBeNull] private ILevelAnimationStep _currentStep;

        public LevelAnimation(LevelDataManager levelDataManager, IDragDropGameManager dragDropManager)
        {
            _levelDataManager = levelDataManager;
            _dragDropManager = dragDropManager;
            _stateControl = new LevelAnimationSwitchStateControl(
                SwitchToState_MergeLayer,
                SwitchToState_DragDrop
            );
            _currentStep = _stateDragDrop = new LevelAnimationStepDragDrop(_stateControl, levelDataManager, dragDropManager);
        }

        public void Enter()
        {
            _currentStep?.Enter();
        }

        public void Update(float dt)
        {
            _currentStep?.Update(dt);
        }

        public void Dispose()
        {
            _currentStep = null;
        }

        private void SwitchToState_MergeLayer(int shelfId, int layerId)
        {
            _currentStep?.Exit();
            _currentStep = new LevelAnimationStepMergeLayer(_stateControl,
                new LevelAnimationStepMergeLayer.InputData(shelfId, layerId), _levelDataManager, _dragDropManager);
            _currentStep.Enter();
        }

        private void SwitchToState_DragDrop()
        {
            _currentStep?.Exit();
            _currentStep = _stateDragDrop;
            _currentStep?.Enter();
        }
    }

    public class LevelAnimationSwitchStateControl
    {
        public readonly Action<int, int> ToMergeLayer;
        public readonly Action ToDragDrop;

        public LevelAnimationSwitchStateControl(
            Action<int, int> toMergeLayer,
            Action toDragDrop
        )
        {
            ToMergeLayer = toMergeLayer;
            ToDragDrop = toDragDrop;
        }
    }

    public interface ILevelAnimationStep
    {
        void Enter();
        void Update(float dt);
        void Exit();
    }
}