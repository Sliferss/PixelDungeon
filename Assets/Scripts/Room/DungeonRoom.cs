using UnityEngine;

public class DungeonRoom : Room
{
    protected override void SetSizeBounds(
        out int minWidth, out int maxWidth,
        out int minHeight, out int maxHeight)
    {
        minWidth = 5;
        maxWidth = 15;
        minHeight = 5;
        maxHeight = 15;
    }

    protected override void OnGenerate(GridManager grid, Vector2Int startOrigin, int width, int height)
    {
        Debug.Log($"Generating DungeonRoom at {startOrigin} ({width}x{height}), tiles placed: {OccupiedPositions.Count}");
    }
}