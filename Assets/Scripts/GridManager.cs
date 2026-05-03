using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width;
    public int height;
    public float cellSize = 1f;

    private Tile[,] tiles;

    public void Initialize()
    {
        tiles = new Tile[width, height];
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
    }

    public bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width &&
               pos.y >= 0 && pos.y < height;
    }

    public void SetTile(Vector2Int pos, Tile tile)
    {
        if (IsValidPosition(pos))
            tiles[pos.x, pos.y] = tile;
    }

    public Tile GetTile(Vector2Int pos)
    {
        if (!IsValidPosition(pos))
            return null;

        return tiles[pos.x, pos.y];
    }
}
