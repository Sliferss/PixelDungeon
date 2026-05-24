using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Size")]
    public int MinWidth = 7;
    public int MaxWidth = 14;
    public int MinHeight = 7;
    public int MaxHeight = 14;

    [Header("Databases")]
    public FirstGroundDatabase GroundDB;
    public FirstFloorDatabase FloorDB;

    public Vector2Int EntranceOrigin { get; private set; }

    private int width;
    private int height;
    private Vector2Int origin;

    public void Generate(GridManager grid, Vector2Int startOrigin)
    {
        Debug.Log("[Room] Generate() START");
        origin = startOrigin;

        if (grid == null) { Debug.LogError("[Room] GridManager is NULL"); return; }
        if (GroundDB == null) { Debug.LogError("[Room] GroundDB is NULL"); return; }
        if (FloorDB == null) { Debug.LogError("[Room] FloorDB is NULL"); return; }

        width = Random.Range(MinWidth, MaxWidth + 1);
        height = Random.Range(MinHeight, MaxHeight + 1);

        Debug.Log("[Room] Generated size: " + width + "x" + height);
        Debug.Log("[Room] Origin: " + origin);

        int createdTiles = 0;
        int skippedTiles = 0;
        int floorPlaced = 0;

        var perimeterFloorTiles = new Dictionary<Vector2Int, object>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int gx = origin.x + x;
                int gy = origin.y + y;

                if (!grid.IsInBounds(gx, gy))
                {
                    Debug.LogWarning("[Room] Out of bounds tile skipped at (" + gx + "," + gy + ")");
                    skippedTiles++;
                    continue;
                }

                Tile tile = grid.GetTile(gx, gy);
                if (tile == null)
                {
                    tile = grid.CreateTile(gx, gy);
                    Debug.Log("[Room] Created Tile at (" + gx + "," + gy + ")");
                }

                if (GroundDB.GetLayer(GroundDB.Ground) == null)
                    Debug.LogError("[Room] GroundDB returned NULL layer");

                tile.GroundLayer = GroundDB.GetLayer(GroundDB.Ground);
                createdTiles++;

                bool isBorder = x == 0 || y == 0 || x == width - 1 || y == height - 1;
                if (isBorder)
                {
                    var floorLayer = FloorDB.GetLayer(FloorDB.Floor);
                    tile.FloorLayer = floorLayer;
                    floorPlaced++;
                    perimeterFloorTiles[new Vector2Int(gx, gy)] = floorLayer;
                    Debug.Log("[Room] Floor placed at (" + gx + "," + gy + ")");
                }

                bool groundWalkable = tile.GroundLayer != null && tile.GroundLayer.IsWalkable();
                bool floorWalkable = tile.FloorLayer != null && tile.FloorLayer.IsWalkable();
                bool groundSolid = tile.GroundLayer != null && tile.GroundLayer.IsSolid();
                bool floorSolid = tile.FloorLayer != null && tile.FloorLayer.IsSolid();

                if (tile.GroundLayer == null)
                    Debug.LogWarning("[Room] Missing GroundLayer at (" + gx + "," + gy + ")");

                tile.IsWalkable = groundWalkable && floorWalkable;
                tile.IsSolid = groundSolid || floorSolid;
            }
        }

        EntranceOrigin = FindEntranceOnPerimeter(perimeterFloorTiles);

        Debug.Log("========== [Room] GENERATION COMPLETE ==========");
        Debug.Log("Size: " + width + "x" + height);
        Debug.Log("Tiles processed: " + createdTiles);
        Debug.Log("Floor tiles placed: " + floorPlaced);
        Debug.Log("Skipped (out of bounds): " + skippedTiles);
        Debug.Log("Entrance origin: " + EntranceOrigin);
    }

    private Vector2Int FindEntranceOnPerimeter(Dictionary<Vector2Int, object> perimeterFloorTiles)
    {
        var edges = new List<List<Vector2Int>>
        {
            BuildEdge(origin.x,             origin.y,              width,  true),
            BuildEdge(origin.x,             origin.y + height - 1, width,  true),
            BuildEdge(origin.x,             origin.y,              height, false),
            BuildEdge(origin.x + width - 1, origin.y,              height, false),
        };

        var validCandidates = new List<Vector2Int>();

        foreach (var edge in edges)
        {
            for (int i = 0; i < edge.Count - 1; i++)
            {
                Vector2Int a = edge[i];
                Vector2Int b = edge[i + 1];

                bool aHasFloor = perimeterFloorTiles.ContainsKey(a);
                bool bHasFloor = perimeterFloorTiles.ContainsKey(b);

                if (aHasFloor && bHasFloor && perimeterFloorTiles[a] == perimeterFloorTiles[b])
                {
                    if (!validCandidates.Contains(a)) validCandidates.Add(a);
                    if (!validCandidates.Contains(b)) validCandidates.Add(b);
                }
            }
        }

        if (validCandidates.Count == 0)
        {
            Debug.LogWarning("[Room] No valid entrance candidate found; falling back to room origin.");
            return origin;
        }

        Vector2Int chosen = validCandidates[Random.Range(0, validCandidates.Count)];
        Debug.Log("[Room] Entrance chosen at " + chosen + " from " + validCandidates.Count + " candidates.");
        return chosen;
    }

    private List<Vector2Int> BuildEdge(int startX, int startY, int count, bool isHorizontal)
    {
        var edge = new List<Vector2Int>(count);
        for (int i = 0; i < count; i++)
        {
            edge.Add(isHorizontal
                ? new Vector2Int(startX + i, startY)
                : new Vector2Int(startX, startY + i));
        }
        return edge;
    }
}