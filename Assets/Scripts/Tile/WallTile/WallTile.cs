// WallTile.cs
using UnityEngine;

/// <summary>
/// Solid, impassable wall. Blocks movement and line of sight.
/// </summary>
public class WallTile : Tile
{
    public WallTile(Vector2Int position) : base(position)
    {
        TileID = "wall";
        IsWalkable = false;
        IsTransparent = false;
    }
}