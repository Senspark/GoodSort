using Cysharp.Threading.Tasks;
using Defines;
using manager.Interface;
using Senspark;
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
        public SelectLevelDialogController(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        public void OpenLevel(int level)
        {
            _ = _sceneLoader.LoadScene<GameScene>(nameof(GameScene)).Then(_ =>
            {
            });
        }
    }
}