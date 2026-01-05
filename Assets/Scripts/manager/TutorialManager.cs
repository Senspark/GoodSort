using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;

namespace manager
{
    public class TutorialManager : ITutorialManager
    {
        public UniTask<bool> Initialize()
        {
            return UniTask.FromResult(true);
        }
        
        // Constructor, inject data manager
        public TutorialManager(IDataManager dataManager)
        {
        }

        public bool IsTutorialFinished()
        {
            
        }

        public void FinishTutorial()
        {
            throw new System.NotImplementedException();
        }
    }
}