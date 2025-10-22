using Dialog;
using Dialog.Controller;
using manager;
using Senspark;

namespace Factory
{
    public class FactoryInitializer
    {
        public void Initialize(IServiceDeclaration containerManager)
        {
            // Create UIControllerFactory instance
            var factory = new UIControllerFactory();

            // Register all dialog controllers
            RegisterDialogControllers(factory, containerManager);

            // Provide factory to ServiceLocator for global access
            ServiceLocator.Instance.Provide<IUIControllerFactory>(factory);
        }

        private void RegisterDialogControllers(UIControllerFactory factory, IServiceDeclaration services)
        {
            // Register SelectLevelDialog controller
            factory.Register<SelectLevelDialog>(() =>
                new SelectLevelDialogController(
                    services.SceneLoader,
                    services.LevelManager
                )
            );

            // Register other dialog controllers here...
            // factory.Register<AnotherDialog>(() => new AnotherDialogController(...));
        }
    }
}