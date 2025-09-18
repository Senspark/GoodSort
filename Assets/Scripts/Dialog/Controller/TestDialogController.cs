

using manager.Interface;

namespace Dialog.Controller
{
    public interface ITestDialogController
    {
        public int Money { get; set; }
        public void AddMoney(int value);
    }
    public class TestDialogController : ITestDialogController
    {
        private readonly IDataManager dataManager;
        //constructor
        public TestDialogController(IDataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        public int Money { get; set; } = 0;
        public void AddMoney(int value)
        {
            Money += value;
        }
    }
    
    public class MockTestDialogController: ITestDialogController
    {
        public int Money { get; set; } = 1000000;
        public void AddMoney(int value)
        {
            Money += 100;
        }
    }
}