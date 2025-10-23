using Cysharp.Threading.Tasks;
using manager.Interface;
using UI;
using Utilities;

namespace Dialog.Controller
{
    public interface ISelectLevelDialogController
    {
        public void OpenLevel(int level);
    }
    public class SelectLevelDialogController : ISelectLevelDialogController
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly ILevelManager _levelManager;
        public SelectLevelDialogController(ISceneLoader sceneLoader, ILevelManager levelManager)
        {
            _sceneLoader = sceneLoader;
            _levelManager = levelManager;
        }
        
        public void OpenLevel(int level)
        {
            _ = _sceneLoader.LoadScene<GameScene>(nameof(GameScene)).Then(_ =>
            {
            });
        }
        
        public int GetLevel()
        {
            return _levelManager.GetCurrentLevel();
        }
    }
}