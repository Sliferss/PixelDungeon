// DestructibleWallTile.cs
using UnityEngine;
using System;

/// <summary>
/// A wall that can be damaged and destroyed, becoming a floor tile logically.
/// Use OnDestroyed to swap the visual and notify the grid.
/// </summary>
public class DestructibleWallTile : WallTile
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public bool IsDestroyed => CurrentHealth <= 0;

    public event Action<DestructibleWallTile> OnDestroyed;
    public event Action<DestructibleWallTile, int> OnDamaged; // tile, remaining hp

    public DestructibleWallTile(Vector2Int position, int health = 3) : base(position)
    {
        TileID = "wall_destructible";
        MaxHealth = health;
        CurrentHealth = health;
    }

    public void TakeDamage(int amount)
    {
        if (IsDestroyed) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnDamaged?.Invoke(this, CurrentHealth);

        if (IsDestroyed)
            Destroy();
    }

    private void Destroy()
    {
        // Become passable — grid manager should swap sprite/prefab
        IsWalkable = true;
        IsTransparent = true;
        OnDestroyed?.Invoke(this);
        Debug.Log($"Wall at {GridPosition} destroyed!");
    }
}