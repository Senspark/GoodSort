using Senspark;

namespace manager.Interface
{
    [Service(typeof(ITutorialManager))]
    public interface ITutorialManager : IService
    {
        public bool IsTutorialFinished();
        public void FinishTutorial();
    }
}