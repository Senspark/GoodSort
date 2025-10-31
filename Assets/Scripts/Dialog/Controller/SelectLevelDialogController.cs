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
        public void PlayEffect(AudioEnum audioEnum);
    }
    public class SelectLevelDialogController : ISelectLevelDialogController
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IAudioManager _audioManager;
        public SelectLevelDialogController(IAudioManager audioManager, ISceneLoader sceneLoader)
        {
            _audioManager = audioManager;
            _sceneLoader = sceneLoader;
        }
        
        public void OpenLevel(int level)
        {
            _ = _sceneLoader.LoadScene<GameScene>(nameof(GameScene)).Then(_ =>
            {
            });
        }
        
        public void PlayEffect(AudioEnum audioEnum)
        {
            _audioManager.PlaySound(audioEnum, 1f);
        }
    }
}