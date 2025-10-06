namespace Engine.AStar
{
    public class AStarNodeWrapper<T> where T : IAStarNode
    {
        public T Node { get; set; }
        public double G { get; set; }
        public double H { get; set; }
        public double F { get; set; }
        public AStarNodeWrapper<T> Parent { get; set; }

        public AStarNodeWrapper(
            T node,
            double g = 0,
            double h = 0,
            double f = 0,
            AStarNodeWrapper<T> parent = null)
        {
            Node = node;
            G = g;
            H = h;
            F = g + h;
            Parent = parent;
        }
    }
}
