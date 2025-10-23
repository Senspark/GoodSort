namespace Defines
{
    public enum GameStateType
    {
        UnInitialize, // chưa load xong scene
        Initialized, // đã load xong scene
        Loaded, // đã sẵn sàng
        Started, // đã bắt đầu chơi
        Paused, // đang tạm dừng
        GameOver, // đã kết thúc
    }
}