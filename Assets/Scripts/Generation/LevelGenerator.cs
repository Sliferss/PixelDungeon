using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int Width = 50;
    public int Height = 50;

    public FloorTileDefinition DefaultFloor;

    public void Generate()
    {
        GridManager.Instance.Initialize(Width, Height);

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var cell = GridManager.Instance.GetCell(new GridPosition(x, y));
                cell.GroundTile = DefaultFloor;
            }
        }

        // TODO: Rooms, corridors, walls, objects
    }
}