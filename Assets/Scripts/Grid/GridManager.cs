using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Map Size")]
    public int Width;
    public int Height;

    [Header("Tilemaps")]
    public Tilemap GroundTilemap;
    public Tilemap FloorTilemap;
    public Tilemap ItemTilemap;
    public Tilemap ActorTilemap;
    public Tilemap StatusTilemap;

    private Grid grid;

    public List<List<Tile>> GridData = new List<List<Tile>>();

    private void Awake()
    {
        grid = GetComponent<Grid>();
        InitializeGrid(Width, Height);
    }

    // =========================================================
    // GRID SETUP
    // =========================================================

    public void InitializeGrid(int width, int height)
    {
        Width = width;
        Height = height;

        GridData.Clear();

        for (int y = 0; y < Height; y++)
        {
            List<Tile> row = new List<Tile>();

            for (int x = 0; x < Width; x++)
                row.Add(null);

            GridData.Add(row);
        }
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width &&
               y >= 0 && y < Height;
    }

    public Tile GetTile(int x, int y)
    {
        if (!IsInBounds(x, y))
            return null;

        return GridData[y][x];
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (!IsInBounds(x, y))
            return;

        GridData[y][x] = tile;
    }

    // =========================================================
    // COORDINATES
    // =========================================================

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return grid.WorldToCell(worldPos);
    }

    public Vector3 CellToWorld(Vector3Int cellPos)
    {
        return grid.GetCellCenterWorld(cellPos);
    }

    // =========================================================
    // TILE CREATION (NOW PURE DATA)
    // =========================================================

    public Tile CreateTile(int x, int y)
    {
        Tile tile = new Tile();
        SetTile(x, y, tile);
        return tile;
    }

    // =========================================================
    // LAYER PLACEMENT
    // =========================================================

    public void PlaceGround(int x, int y, TileBase tile)
    {
        GroundTilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    public void PlaceFloor(int x, int y, TileBase tile)
    {
        FloorTilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    public void PlaceItem(int x, int y, TileBase tile)
    {
        ItemTilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    public void PlaceActor(int x, int y, TileBase tile)
    {
        ActorTilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    public void PlaceStatus(int x, int y, TileBase tile)
    {
        StatusTilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    // =========================================================
    // RENDER
    // =========================================================

    public void RenderTile(int x, int y)
    {
        Tile tile = GetTile(x, y);

        if (tile == null)
            return;

        if (tile.GroundLayer != null)
            PlaceGround(x, y, tile.GroundLayer.Tile);

        if (tile.FloorLayer != null)
            PlaceFloor(x, y, tile.FloorLayer.Tile);

        if (tile.ItemLayer != null && tile.ItemLayer.Count > 0)
            PlaceItem(x, y, tile.ItemLayer[^1]);

        if (tile.Character != null)
            PlaceActor(x, y, tile.Character);

        if (tile.StatusLayer != null && tile.StatusLayer.Count > 0)
            PlaceStatus(x, y, tile.StatusLayer[^1]);
    }

    public void RenderAll()
    {
        GroundTilemap.ClearAllTiles();
        FloorTilemap.ClearAllTiles();
        ItemTilemap.ClearAllTiles();
        ActorTilemap.ClearAllTiles();
        StatusTilemap.ClearAllTiles();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
                RenderTile(x, y);
        }
    }
}