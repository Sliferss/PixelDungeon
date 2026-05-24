using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Size")]
    public int MinWidth = 4;
    public int MaxWidth = 10;

    public int MinHeight = 4;
    public int MaxHeight = 10;

    [Header("Databases")]
    public FirstGroundDatabase GroundDB;
    public FirstFloorDatabase FloorDB;

    private int width;
    private int height;

    private Vector2Int origin;

    public void Generate(GridManager grid, Vector2Int startOrigin)
    {
        Debug.Log("[Room] Generate() START");

        origin = startOrigin;

        // ------------------------
        // Validate DBs
        // ------------------------
        if (grid == null)
        {
            Debug.LogError("[Room] GridManager is NULL");
            return;
        }

        if (GroundDB == null)
        {
            Debug.LogError("[Room] GroundDB is NULL");
            return;
        }

        if (FloorDB == null)
        {
            Debug.LogError("[Room] FloorDB is NULL");
            return;
        }

        // ------------------------
        // Size generation
        // ------------------------
        width = Random.Range(MinWidth, MaxWidth + 1);
        height = Random.Range(MinHeight, MaxHeight + 1);

        Debug.Log($"[Room] Generated size: {width}x{height}");
        Debug.Log($"[Room] Origin: {origin}");

        int createdTiles = 0;
        int skippedTiles = 0;
        int floorPlaced = 0;

        // ------------------------
        // Main loop
        // ------------------------
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int gx = origin.x + x;
                int gy = origin.y + y;

                if (!grid.IsInBounds(gx, gy))
                {
                    Debug.LogWarning($"[Room] Out of bounds tile skipped at ({gx},{gy})");
                    skippedTiles++;
                    continue;
                }

                Tile tile = grid.GetTile(gx, gy);

                if (tile == null)
                {
                    tile = grid.CreateTile(gx, gy);
                    Debug.Log($"[Room] Created Tile at ({gx},{gy})");
                }

                // ------------------------
                // Ground
                // ------------------------
                if (GroundDB.GetLayer(GroundDB.Ground) == null)
                {
                    Debug.LogError("[Room] GroundDB returned NULL layer");
                }

                tile.GroundLayer = GroundDB.GetLayer(GroundDB.Ground);
                createdTiles++;

                // ------------------------
                // Floor (border only)
                // ------------------------
                bool isBorder =
                    x == 0 || y == 0 ||
                    x == width - 1 || y == height - 1;

                if (isBorder)
                {
                    tile.FloorLayer = FloorDB.GetLayer(FloorDB.Floor);
                    floorPlaced++;

                    Debug.Log($"[Room] Floor placed at ({gx},{gy})");
                }

                // ------------------------
                // Rules
                // ------------------------
                if (tile.GroundLayer != null)
                {
                    tile.IsWalkable = tile.GroundLayer.IsWalkable();
                    tile.IsSolid = tile.GroundLayer.IsSolid();
                }
                else
                {
                    Debug.LogWarning($"[Room] Missing GroundLayer at ({gx},{gy})");
                }
            }
        }
        // ------------------------
        // Summary
        // ------------------------
        Debug.Log("========== [Room] GENERATION COMPLETE ==========");
        Debug.Log($"Size: {width}x{height}");
        Debug.Log($"Tiles processed: {createdTiles}");
        Debug.Log($"Floor tiles placed: {floorPlaced}");
        Debug.Log($"Skipped (out of bounds): {skippedTiles}");
    }
}