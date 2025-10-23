using manager.Interface;
using UI;
using UnityEngine;
using Utilities;

namespace Dialog.Controller
{
    public interface ICompleteLevelDialogController
    {
        public void OnNextLevelButtonPressed();
    }
    
    public class CompleteLevelDialogController : ICompleteLevelDialogController
    {
        private readonly ILevelManager _levelManager;
        private readonly ISceneLoader _sceneLoader;
        public CompleteLevelDialogController(ILevelManager levelManager, ISceneLoader sceneLoader)
        {
            _levelManager = levelManager;
            _sceneLoader = sceneLoader;
        }
        
        public void OnNextLevelButtonPressed()
        {
            _levelManager.GoToNextLevel();
            _ = _sceneLoader.LoadScene<MainMenu>(nameof(MainMenu)).Then(_ =>
            {

            });
        }
    }
}