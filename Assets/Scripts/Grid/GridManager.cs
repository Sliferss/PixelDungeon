using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int Width;
    public int Height;

    private GridCell[,] grid;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(int width, int height)
    {
        Width = width;
        Height = height;

        grid = new GridCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new GridCell
                {
                    Position = new GridPosition(x, y)
                };
            }
        }
    }

    public GridCell GetCell(GridPosition pos)
    {
        if (pos.X < 0 || pos.X >= Width || pos.Y < 0 || pos.Y >= Height)
            return null;

        return grid[pos.X, pos.Y];
    }

    public Vector3 GridToWorld(GridPosition pos)
    {
        return new Vector3(pos.X, pos.Y, 0);
    }

    public GridPosition WorldToGrid(Vector3 worldPos)
    {
        return new GridPosition(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
    }
}