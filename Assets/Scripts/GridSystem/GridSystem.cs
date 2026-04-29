// GridSystem.cs
using UnityEngine;
using System;

public class GridSystem<TTile> where TTile : Tile
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float CellSize { get; private set; }

    private TTile[,] _grid;
    private Vector3 _originPosition;

    public event Action<TTile> OnTileChanged;

    public GridSystem(int width, int height, float cellSize, Vector3 origin,
                      Func<Vector2Int, TTile> tileFactory)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        _originPosition = origin;

        _grid = new TTile[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _grid[x, y] = tileFactory(new Vector2Int(x, y));
    }

    // --- Coordinate Conversion ---

    public Vector3 GridToWorld(int x, int y)
        => _originPosition + new Vector3(x * CellSize, y * CellSize);

    public Vector3 GridToWorld(Vector2Int pos)
        => GridToWorld(pos.x, pos.y);

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt((worldPos.x - _originPosition.x) / CellSize),
            Mathf.FloorToInt((worldPos.y - _originPosition.y) / CellSize)
        );
    }

    // --- Tile Access ---

    public bool IsInBounds(int x, int y)
        => x >= 0 && y >= 0 && x < Width && y < Height;

    public bool IsInBounds(Vector2Int pos)
        => IsInBounds(pos.x, pos.y);

    public TTile GetTile(int x, int y)
        => IsInBounds(x, y) ? _grid[x, y] : null;

    public TTile GetTile(Vector2Int pos)
        => GetTile(pos.x, pos.y);

    public void SetTile(int x, int y, TTile tile)
    {
        if (!IsInBounds(x, y)) return;
        _grid[x, y] = tile;
        OnTileChanged?.Invoke(tile);
    }

    // --- Neighbour Queries (useful for dungeon gen) ---

    /// <summary>
    /// Returns all 8 surrounding neighbours (cardinal + diagonal).
    /// </summary>
    public TTile[] GetAllNeighbours(int x, int y)
    {
        var dirs = new Vector2Int[]
        {
        new(x,     y + 1), // N
        new(x + 1, y + 1), // NE
        new(x + 1, y),     // E
        new(x + 1, y - 1), // SE
        new(x,     y - 1), // S
        new(x - 1, y - 1), // SW
        new(x - 1, y),     // W
        new(x - 1, y + 1)  // NW
        };

        var result = new System.Collections.Generic.List<TTile>();
        foreach (var d in dirs)
            if (IsInBounds(d)) result.Add(_grid[d.x, d.y]);

        return result.ToArray();
    }
}