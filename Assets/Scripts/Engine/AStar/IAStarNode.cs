namespace Engine.AStar
{
    // Interface cho Node của puzzle
    public interface IAStarNode
    {
        // Unique identifier để so sánh các node
        string GetId();

        // Kiểm tra xem node này có phải là goal không
        bool IsGoal();

        // Clone node để tránh mutation
        IAStarNode Clone();
    }
}
