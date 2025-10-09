using System;
using Core;
using JetBrains.Annotations;

namespace Strategy.Level
{
    /* Điều khiển chuyển động/hiệu ứng/hoạt ảnh trong Game Scene
     Chia làm nhiều Step, mỗi Step chỉ làm 1 nhiệm vụ
     */
    public class LevelAnimation
    {
        private readonly LevelDataManager _levelDataManager;
        private readonly IDragDropManager _dragDropManager;

        private readonly LevelAnimationStepDragDrop _stateDragDrop;
        private readonly LevelAnimationSwitchStateControl _stateControl;
        [CanBeNull] private ILevelAnimationStep _currentStep;

        public LevelAnimation(LevelDataManager levelDataManager, IDragDropManager dragDropManager)
        {
            _levelDataManager = levelDataManager;
            _dragDropManager = dragDropManager;
            _stateControl = new LevelAnimationSwitchStateControl(
                SwitchToState_MergeLayer,
                SwitchToState_UnlockShelves,
                SwitchToState_DragDrop
            );
            _currentStep = _stateDragDrop =
                new LevelAnimationStepDragDrop(_stateControl, levelDataManager, dragDropManager);
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

        private void SwitchToState_MergeLayer(LevelAnimationStepMergeLayer.InputData data)
        {
            _currentStep?.Exit();
            _currentStep = new LevelAnimationStepMergeLayer(_stateControl, data, _levelDataManager, _dragDropManager);
            _currentStep.Enter();
        }
        
        private void SwitchToState_UnlockShelves(LevelAnimationUnlockShelves.InputData data)
        {
            _currentStep?.Exit();
            _currentStep = new LevelAnimationUnlockShelves(_stateControl, data, _levelDataManager);
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
        public readonly Action<LevelAnimationStepMergeLayer.InputData> ToMergeLayer;
        public readonly Action ToDragDrop;
        public readonly Action<LevelAnimationUnlockShelves.InputData> ToUnlockShelves;

        public LevelAnimationSwitchStateControl(
            Action<LevelAnimationStepMergeLayer.InputData> toMergeLayer,
            Action<LevelAnimationUnlockShelves.InputData> toUnlockShelves,
            Action toDragDrop
        )
        {
            ToMergeLayer = toMergeLayer;
            ToUnlockShelves = toUnlockShelves;
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