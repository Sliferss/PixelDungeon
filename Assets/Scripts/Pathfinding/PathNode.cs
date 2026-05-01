public class PathNode
{
    public GridPosition Position;

    public int GCost;
    public int HCost;

    public int FCost => GCost + HCost;

    public PathNode Parent;
}