using Senspark;
using Tutorial;

namespace manager.Interface
{
    [Service(typeof(ITutorialManager))]
    public interface ITutorialManager : IService
    {
        public bool IsTutorialFinished(TutorialType tutorialType);
        public void FinishTutorial(TutorialType tutorialType);
    }
}