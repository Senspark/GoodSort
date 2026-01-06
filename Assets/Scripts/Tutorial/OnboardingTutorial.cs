using System.Collections.Generic;
using manager.Interface;
using UnityEngine;

namespace Tutorial
{
    public class OnboardingTutorial : ITutorial
    {
        private TutorialType TutorialType => TutorialType.Onboarding;
        private readonly ITutorialManager _tutorialManager;
        private readonly List<Step> _steps;
        private Animator _animator;
        
        private int _currentStepIndex;

        public OnboardingTutorial(ITutorialManager tutorialManager, Animator animator)
        {
            _tutorialManager = tutorialManager;
            _animator = animator;
            _steps = new List<Step>
            {
                new()
                {
                    targetObjectName = "Item_0", 
                    actionType = TutorialActionType.OnClear, 
                    eventParameter = "" , 
                    OnStepStart = () => animator.Play("Step1")
                },
                new()
                {
                    targetObjectName = "Item_1", 
                    actionType = TutorialActionType.OnClear, 
                    eventParameter = "" , 
                    OnStepStart = () => animator.Play("Step2")
                }
            };
        }
        
        public void Start()
        {
            _currentStepIndex = 0;
            TutorialEvents.OnActionReached += OnActionReached;
            ExecuteStep();
        }
        
        private void ExecuteStep() {
            var step = _steps[_currentStepIndex];
            step.OnStepStart?.Invoke();
        }

        public void Finish()
        {
            _animator.StopPlayback();
            _animator.gameObject.SetActive(false);
            TutorialEvents.OnActionReached -= OnActionReached;
            _tutorialManager.FinishTutorial(TutorialType);
        }

        public bool CheckStart()
        {
            return !_tutorialManager.IsTutorialFinished(TutorialType);
        }

        public bool CheckFinish() => _currentStepIndex >= _steps.Count;
        
        public string GetCurrentTargetName() => _steps[_currentStepIndex].targetObjectName;
        
        private void OnActionReached(TutorialActionType type, string param) 
        {
            var currentStep = _steps[_currentStepIndex];
            if (type == currentStep.actionType) 
            {
                _currentStepIndex++;
                if (CheckFinish()) Finish();
                else ExecuteStep();
            }
        }
    }
}