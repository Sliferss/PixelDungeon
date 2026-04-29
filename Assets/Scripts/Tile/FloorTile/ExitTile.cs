// ExitTile.cs
using UnityEngine;
using System;

/// <summary>
/// The dungeon exit. Triggers level transition when entered.
/// Can be locked until a condition is met (e.g. boss defeated).
/// </summary>
public class ExitTile : FloorTile
{
    public bool IsLocked { get; private set; } = false;
    public int TargetLevel { get; private set; }

    public event Action<GameObject, int> OnExitReached; // entity, next level

    public ExitTile(Vector2Int position, int targetLevel = -1, bool startsLocked = false)
        : base(position)
    {
        TileID = "exit";
        TargetLevel = targetLevel;
        IsLocked = startsLocked;
    }

    public override void OnEnter(GameObject entity)
    {
        if (IsLocked)
        {
            Debug.Log("Exit is locked!");
            return;
        }

        Debug.Log($"{entity.name} reached the exit — going to level {TargetLevel}");
        OnExitReached?.Invoke(entity, TargetLevel);
    }

    public void Unlock() => IsLocked = false;
    public void Lock() => IsLocked = true;
}