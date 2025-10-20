using System.Collections.Generic;

namespace Engine.AStar
{
    // Interface cho Puzzle
    public interface IAStarPuzzle<T> where T : IAStarNode
    {
        // Tính G cost (actual cost từ start đến node này)
        double CalculateG(T node, T parent);

        // Tính H cost (heuristic cost từ node này đến goal)
        double CalculateH(T node);

        // Tìm tất cả neighbours có thể từ node hiện tại
        List<T> GetNeighbours(T node);
    }
}
