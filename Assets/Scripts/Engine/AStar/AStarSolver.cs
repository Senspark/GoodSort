using System.Collections.Generic;
using System.Linq;

namespace Engine.AStar
{
    // Generic A* Solver
    public class AStarSolver<T> where T : IAStarNode
    {
        private List<AStarNodeWrapper<T>> openList = new List<AStarNodeWrapper<T>>();
        private Dictionary<string, AStarNodeWrapper<T>> closedList = new Dictionary<string, AStarNodeWrapper<T>>();
        private Dictionary<string, AStarNodeWrapper<T>> openListMap = new Dictionary<string, AStarNodeWrapper<T>>();
        private readonly IAStarPuzzle<T> puzzle;

        public AStarSolver(IAStarPuzzle<T> puzzle)
        {
            this.puzzle = puzzle;
        }

        /// <summary>
        /// Giải puzzle từ startNode
        /// </summary>
        /// <returns>Mảng các node từ start đến goal, hoặc null nếu không tìm thấy</returns>
        public List<T> Solve(T startNode)
        {
            // Reset lists
            openList = new List<AStarNodeWrapper<T>>();
            closedList.Clear();
            openListMap.Clear();

            // Khởi tạo start node
            var startWrapper = new AStarNodeWrapper<T>(
                startNode,
                0,
                puzzle.CalculateH(startNode),
                0,
                null
            );
            startWrapper.F = startWrapper.G + startWrapper.H;

            AddToOpenList(startWrapper);

            // Main loop
            while (openList.Count > 0)
            {
                // Lấy node có F nhỏ nhất (nếu F bằng nhau thì ưu tiên H nhỏ hơn)
                var current = GetLowestFNode();

                // Kiểm tra goal
                if (current.Node.IsGoal())
                {
                    return ReconstructPath(current);
                }

                // Di chuyển từ open list sang closed list
                RemoveFromOpenList(current);
                closedList[current.Node.GetId()] = current;

                // Xử lý neighbours
                var neighbours = puzzle.GetNeighbours(current.Node);

                foreach (var neighbourNode in neighbours)
                {
                    var neighbourId = neighbourNode.GetId();

                    // Skip nếu đã trong closed list
                    if (closedList.ContainsKey(neighbourId))
                    {
                        continue;
                    }

                    // Tính costs
                    var g = puzzle.CalculateG(neighbourNode, current.Node);
                    var h = puzzle.CalculateH(neighbourNode);
                    var f = g + h;

                    // Kiểm tra xem neighbour đã trong open list chưa
                    if (openListMap.TryGetValue(neighbourId, out var existingNeighbour))
                    {
                        // Nếu tìm thấy đường tốt hơn, cập nhật
                        if (g < existingNeighbour.G)
                        {
                            existingNeighbour.G = g;
                            existingNeighbour.F = f;
                            existingNeighbour.Parent = current;
                        }
                    }
                    else
                    {
                        // Thêm neighbour mới vào open list
                        var neighbourWrapper = new AStarNodeWrapper<T>(
                            neighbourNode,
                            g,
                            h,
                            f,
                            current
                        );
                        AddToOpenList(neighbourWrapper);
                    }
                }
            }

            // Không tìm thấy đường
            return null;
        }

        /// <summary>
        /// Giải puzzle từng bước (iterator pattern)
        /// Useful để visualize hoặc debug
        /// </summary>
        public IEnumerable<SolveStep<T>> SolveStepByStep(T startNode)
        {
            // Reset lists
            openList = new List<AStarNodeWrapper<T>>();
            closedList.Clear();
            openListMap.Clear();

            // Khởi tạo start node
            var startWrapper = new AStarNodeWrapper<T>(
                startNode,
                0,
                puzzle.CalculateH(startNode),
                0,
                null
            );
            startWrapper.F = startWrapper.G + startWrapper.H;

            AddToOpenList(startWrapper);

            // Main loop với yield
            while (openList.Count > 0)
            {
                var current = GetLowestFNode();

                // Yield current state
                yield return new SolveStep<T>(
                    current.Node,
                    openList.Select(w => w.Node).ToList(),
                    closedList.Values.Select(w => w.Node).ToList(),
                    current.Node.IsGoal()
                );

                if (current.Node.IsGoal())
                {
                    yield break;
                }

                // Di chuyển từ open list sang closed list
                RemoveFromOpenList(current);
                closedList[current.Node.GetId()] = current;

                // Xử lý neighbours
                var neighbours = puzzle.GetNeighbours(current.Node);

                foreach (var neighbourNode in neighbours)
                {
                    var neighbourId = neighbourNode.GetId();

                    if (closedList.ContainsKey(neighbourId))
                    {
                        continue;
                    }

                    var g = puzzle.CalculateG(neighbourNode, current.Node);
                    var h = puzzle.CalculateH(neighbourNode);
                    var f = g + h;

                    if (openListMap.TryGetValue(neighbourId, out var existingNeighbour))
                    {
                        if (g < existingNeighbour.G)
                        {
                            existingNeighbour.G = g;
                            existingNeighbour.F = f;
                            existingNeighbour.Parent = current;
                        }
                    }
                    else
                    {
                        var neighbourWrapper = new AStarNodeWrapper<T>(
                            neighbourNode,
                            g,
                            h,
                            f,
                            current
                        );
                        AddToOpenList(neighbourWrapper);
                    }
                }
            }
        }

        private void AddToOpenList(AStarNodeWrapper<T> wrapper)
        {
            openList.Add(wrapper);
            openListMap[wrapper.Node.GetId()] = wrapper;
        }

        private void RemoveFromOpenList(AStarNodeWrapper<T> wrapper)
        {
            openList.Remove(wrapper);
            openListMap.Remove(wrapper.Node.GetId());
        }

        private AStarNodeWrapper<T> GetLowestFNode()
        {
            // Sort by F ascending, then by H ascending
            openList.Sort((a, b) =>
            {
                var fCompare = a.F.CompareTo(b.F);
                if (fCompare != 0)
                {
                    return fCompare;
                }
                return a.H.CompareTo(b.H);
            });

            return openList[0];
        }

        private List<T> ReconstructPath(AStarNodeWrapper<T> goalWrapper)
        {
            var path = new List<T>();
            AStarNodeWrapper<T>? current = goalWrapper;

            while (current != null)
            {
                path.Insert(0, current.Node);
                current = current.Parent;
            }

            return path;
        }
    }

    public class SolveStep<T> where T : IAStarNode
    {
        public T Current { get; }
        public List<T> OpenList { get; }
        public List<T> ClosedList { get; }
        public bool IsGoal { get; }

        public SolveStep(T current, List<T> openList, List<T> closedList, bool isGoal)
        {
            Current = current;
            OpenList = openList;
            ClosedList = closedList;
            IsGoal = isGoal;
        }
    }
}
