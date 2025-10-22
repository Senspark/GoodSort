using Senspark;

namespace manager.Interface
{
    [Service(nameof(ILevelManager))]
    public interface ILevelManager : IService
    {
        void SetCurrentLevel(int level);
        int GetCurrentLevel();
    }
}