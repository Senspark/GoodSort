using Dialog;
using Dialog.Controller;
using manager;

namespace Factory
{
    public class FactoryInitializer
    {
        public void Initialize(IServiceDeclaration containerManager)
        {
            var factory = new UIControllerFactory();
            factory.Register<TestDialog>(() => new TestDialogController(containerManager.DataManager));
        }
    }
}