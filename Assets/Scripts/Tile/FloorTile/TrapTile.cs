// TrapTile.cs
using UnityEngine;
using System;

/// <summary>
/// A hidden floor tile that triggers once when stepped on.
/// Subscribe to OnTrapTriggered to handle damage, effects, sounds, etc.
/// </summary>
public class TrapTile : FloorTile
{
    public bool IsArmed { get; private set; } = true;
    public bool IsRevealed { get; private set; } = false;
    public int DamageAmount { get; private set; }

    public event Action<GameObject, int> OnTrapTriggered; // entity, damage

    public TrapTile(Vector2Int position, int damage = 5) : base(position)
    {
        TileID = "trap";
        DamageAmount = damage;
    }

    public override void OnEnter(GameObject entity)
    {
        if (!IsArmed) return;

        Trigger(entity);
    }

    private void Trigger(GameObject entity)
    {
        IsArmed = false;
        IsRevealed = true;

        Debug.Log($"Trap triggered at {GridPosition} by {entity.name} for {DamageAmount} dmg");
        OnTrapTriggered?.Invoke(entity, DamageAmount);
    }

    /// <summary>Allow a rogue/perception check to reveal the trap without triggering.</summary>
    public void Reveal() => IsRevealed = true;

    /// <summary>Rearm the trap (for resettable dungeon rooms, etc.).</summary>
    public void Rearm()
    {
        IsArmed = true;
        IsRevealed = false;
    }
}