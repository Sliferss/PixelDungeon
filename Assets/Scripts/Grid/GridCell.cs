using Mono.Cecil;
using System;

public class GridCell
{
    public GridPosition Position;
    public TileDefinition GroundTile;
    public WallDefinition WallTile;

    public WorldObject Occupant;

    public bool IsVisible;
    public bool IsExplored;

    public bool IsWalkable =>
        GroundTile != null &&
        GroundTile.IsWalkable &&
        (WallTile == null || !WallTile.BlocksMovement) &&
        (Occupant == null || !Occupant.BlocksMovement);
}