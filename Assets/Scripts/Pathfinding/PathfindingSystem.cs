using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all A* pathfinding requests.
/// Player reference is assigned automatically when the player spawns.
/// </summary>
public sealed class PathfindingSystem : MonoBehaviour
{
    public static PathfindingSystem Instance { get; private set; }

    private PlayerController player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Called by PlayerController when it is created.
    /// </summary>
    public void RegisterPlayer(PlayerController playerController)
    {
        player = playerController;
    }

    /// <summary>
    /// Requests a path for the registered player.
    /// </summary>
    public void RequestPath(GridPosition target)
    {
        if (player == null)
        {
            Debug.LogWarning(
                "[PathfindingSystem] No player registered yet."
            );
            return;
        }

        List<GridPosition> path =
            FindPath(player.Position, target);

        if (path == null || path.Count == 0)
            return;

        player.MoveAlongPath(path);
    }

    public List<GridPosition> FindPath(
        GridPosition start,
        GridPosition end)
    {
        var openList = new List<PathNode>();
        var closedSet = new HashSet<GridPosition>();
        var allNodes = new Dictionary<GridPosition, PathNode>();

        PathNode startNode = new()
        {
            Position = start,
            GCost = 0,
            HCost = Heuristic(start, end)
        };

        openList.Add(startNode);
        allNodes[start] = startNode;

        while (openList.Count > 0)
        {
            PathNode current = GetLowestFCostNode(openList);

            if (current.Position == end)
                return RetracePath(current);

            openList.Remove(current);
            closedSet.Add(current.Position);

            foreach (GridPosition neighborPos in GetNeighbors(current.Position))
            {
                if (closedSet.Contains(neighborPos))
                    continue;

                GridCell cell =
                    GridManager.Instance.GetCell(neighborPos);

                if (cell == null || !cell.IsWalkable)
                    continue;

                int tentativeGCost =
                    current.GCost +
                    cell.GroundTile.MovementCost;

                if (!allNodes.TryGetValue(
                        neighborPos,
                        out PathNode neighbor))
                {
                    neighbor = new PathNode
                    {
                        Position = neighborPos,
                        GCost = int.MaxValue
                    };

                    allNodes.Add(neighborPos, neighbor);
                }

                if (tentativeGCost >= neighbor.GCost)
                    continue;

                neighbor.GCost = tentativeGCost;
                neighbor.HCost =
                    Heuristic(neighborPos, end);
                neighbor.Parent = current;

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
            }
        }

        return null;
    }

    private static PathNode GetLowestFCostNode(
        List<PathNode> nodes)
    {
        PathNode best = nodes[0];

        for (int i = 1; i < nodes.Count; i++)
        {
            PathNode candidate = nodes[i];

            if (candidate.FCost < best.FCost ||
                (candidate.FCost == best.FCost &&
                 candidate.HCost < best.HCost))
            {
                best = candidate;
            }
        }

        return best;
    }

    private static List<GridPosition> RetracePath(
        PathNode endNode)
    {
        List<GridPosition> path = new();

        PathNode current = endNode;

        while (current != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }

        path.Reverse();

        if (path.Count > 0)
            path.RemoveAt(0);

        return path;
    }

    private static int Heuristic(
        GridPosition a,
        GridPosition b)
    {
        return Mathf.Abs(a.X - b.X) +
               Mathf.Abs(a.Y - b.Y);
    }

    private static IEnumerable<GridPosition> GetNeighbors(
        GridPosition pos)
    {
        yield return new GridPosition(pos.X + 1, pos.Y);
        yield return new GridPosition(pos.X - 1, pos.Y);
        yield return new GridPosition(pos.X, pos.Y + 1);
        yield return new GridPosition(pos.X, pos.Y - 1);
    }
}