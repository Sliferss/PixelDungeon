using UnityEngine;
using System.Collections.Generic;

public abstract class Room : MonoBehaviour
{
    [Header("Size")]
    public int MinWidth = 5;
    public int MaxWidth = 15;
    public int MinHeight = 5;
    public int MaxHeight = 15;

    [Header("Databases")]
    public FirstGroundDatabase GroundDB;
    public FirstFloorDatabase FloorDB;

    // Tracks every grid position this room occupies
    public List<Vector2Int> OccupiedPositions { get; private set; } = new List<Vector2Int>();

    protected abstract void SetSizeBounds(
        out int minWidth, out int maxWidth,
        out int minHeight, out int maxHeight);

    protected abstract void OnGenerate(GridManager grid, Vector2Int startOrigin, int width, int height);

    protected virtual void Awake()
    {
        SetSizeBounds(out MinWidth, out MaxWidth, out MinHeight, out MaxHeight);
    }

    public void Generate(GridManager grid, Vector2Int origin)
    {
        int width = Random.Range(MinWidth, MaxWidth + 1);
        int height = Random.Range(MinHeight, MaxHeight + 1);

        OccupiedPositions.Clear();
        PlaceTiles(grid, origin, width, height);
        OnGenerate(grid, origin, width, height);
    }

    // -----------------------------
    // Core tile placement
    // -----------------------------
    private void PlaceTiles(GridManager grid, Vector2Int origin, int width, int height)
    {
        // origin is just any tile inside the room, so offset so it sits
        // somewhere inside rather than always being the bottom-left corner
        int offsetX = Random.Range(0, width);
        int offsetY = Random.Range(0, height);

        int startX = origin.x - offsetX;
        int startY = origin.y - offsetY;

        for (int dy = 0; dy < height; dy++)
        {
            for (int dx = 0; dx < width; dx++)
            {
                int x = startX + dx;
                int y = startY + dy;

                if (!grid.IsInBounds(x, y))
                    continue;

                Tile existing = grid.GetTile(x, y);

                // Skip if another room already owns this tile
                if (existing != null && existing.Room != null && existing.Room != this)
                    continue;

                Tile tile = existing ?? grid.CreateTile(x, y);
                tile.Room = this;

                // Place ground visual if a database is assigned
                if (GroundDB != null && GroundDB.Ground != null)
                {
                    LayerBase layer = GroundDB.Ground.GetComponent<LayerBase>();
                    if (layer != null)
                    {
                        tile.GroundLayer = layer;
                        grid.PlaceGround(x, y, layer.Tile);
                    }
                }

                OccupiedPositions.Add(new Vector2Int(x, y));
            }
        }
    }
}