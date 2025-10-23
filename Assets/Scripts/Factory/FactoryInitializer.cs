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
                containerManager.SceneLoader,
                containerManager.LevelManager
            ));
            factory.Register<CompleteLevelDialog>(() => new CompleteLevelDialogController(
                containerManager.LevelManager,
                containerManager.SceneLoader
            ));
        }
    }
}