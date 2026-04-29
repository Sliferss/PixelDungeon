// Tile.cs
using UnityEngine;

/// <summary>
/// Abstract base class for all tiles. Contains shared state and virtual hooks
/// that subclasses override to implement special behaviour.
/// </summary>
public abstract class Tile
{
    // --- Position ---
    public Vector2Int GridPosition { get; private set; }

    // --- State Flags ---
    public bool IsWalkable { get; protected set; } = true;
    public bool IsTransparent { get; protected set; } = true;  // for line-of-sight / FOV
    public bool IsVisible { get; protected set; } = false; // currently in FOV
    public bool IsExplored { get; protected set; } = false; // seen at least once

    // --- Metadata (useful for dungeon gen / saving) ---
    public string TileID { get; protected set; } = "base_tile";

    protected Tile(Vector2Int position)
    {
        GridPosition = position;
    }

    // --- Virtual Hooks ---

    /// <summary>Called when an entity steps onto this tile.</summary>
    public virtual void OnEnter(GameObject entity) { }

    /// <summary>Called when an entity leaves this tile.</summary>
    public virtual void OnExit(GameObject entity) { }

    /// <summary>Called when this tile enters the player's field of view.</summary>
    public virtual void OnReveal()
    {
        IsVisible = true;
        IsExplored = true;
    }

    /// <summary>Called when this tile leaves the player's field of view.</summary>
    public virtual void OnHide()
    {
        IsVisible = false;
    }

    /// <summary>Optional per-turn tick — override for environmental effects.</summary>
    public virtual void OnTick() { }

    public override string ToString() =>
        $"[{TileID}] @ {GridPosition} | Walkable:{IsWalkable}";
}