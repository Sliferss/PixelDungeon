// FloorTile.cs
using UnityEngine;

/// <summary>
/// Standard walkable floor. Serves as the base for all interactive floor types.
/// </summary>
public class FloorTile : Tile
{
    public FloorTile(Vector2Int position) : base(position)
    {
        TileID = "floor";
        IsWalkable = true;
        IsTransparent = true;
    }
}