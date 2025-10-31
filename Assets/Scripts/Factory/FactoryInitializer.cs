using Dialog;
using Dialog.Controller;
using manager;

namespace Factory
{
    public class FactoryInitializer
    {
        public void Initialize(IServiceDeclaration containerManager)
        {
            var factory = UIControllerFactory.Instance;
            factory.Register<SelectLevelDialog>(() => new SelectLevelDialogController(
                containerManager.AudioManager,
                containerManager.SceneLoader
            ));
            factory.Register<CompleteLevelDialog>(() => new CompleteLevelDialogController(
                containerManager.AudioManager,
                containerManager.LevelManager,
                containerManager.SceneLoader
            ));
        }
    }
}