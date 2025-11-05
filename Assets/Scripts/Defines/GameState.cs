namespace Defines
{
    public enum GameStateType
    {
        UnInitialize, // chưa load xong scene
        Initialized, // đã load xong scene
        Loaded, // đã sẵn sàng
        Playing,
        Paused, // đang tạm dừng
        GameOver, // đã kết thúc
    }
}