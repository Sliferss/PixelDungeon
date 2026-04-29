// SpawnTile.cs
using UnityEngine;
using System;

/// <summary>
/// Marks a tile as a spawn point — for the player, enemies, or items.
/// The dungeon generator uses SpawnType to decide what to place here.
/// </summary>
public enum SpawnType { Player, Enemy, Item, Boss }

public class SpawnTile : FloorTile
{
    public SpawnType SpawnType { get; private set; }
    public bool HasSpawned { get; private set; } = false;

    public event Action<SpawnTile> OnSpawn;

    public SpawnTile(Vector2Int position, SpawnType spawnType) : base(position)
    {
        TileID = $"spawn_{spawnType.ToString().ToLower()}";
        SpawnType = spawnType;
    }

    /// <summary>Call this when the dungeon spawner places the entity.</summary>
    public void MarkSpawned()
    {
        HasSpawned = true;
        OnSpawn?.Invoke(this);
    }

    public void Reset() => HasSpawned = false;
}