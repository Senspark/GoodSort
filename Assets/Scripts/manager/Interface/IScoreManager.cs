using Senspark;

namespace manager.Interface
{
    [Service(nameof(IScoreManager))]
    public interface IScoreManager : IService
    {
        int HighestLevelPlayed { get; }
        int LastLevelPlayed { get; }
        int AmountLevelPlayed { get; }
        void PlayedLevel(int level);
    }
}