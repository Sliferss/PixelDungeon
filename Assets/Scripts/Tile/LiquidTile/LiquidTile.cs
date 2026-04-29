// LiquidTile.cs
using UnityEngine;
using System;

/// <summary>
/// Water, lava, acid, etc. Walkable but applies an effect each tick and on enter.
/// Extend with specific types (LavaTile, AcidTile) overriding ApplyEffect().
/// </summary>
public enum LiquidType { Water, Lava, Acid, Mud }

public class LiquidTile : Tile
{
    public LiquidType LiquidType { get; private set; }
    public int DamagePerTurn { get; protected set; } = 0;

    private GameObject _occupant;

    public event Action<GameObject, int> OnLiquidDamage;

    public LiquidTile(Vector2Int position, LiquidType type) : base(position)
    {
        TileID = $"liquid_{type.ToString().ToLower()}";
        LiquidType = type;
        IsWalkable = true;   // entity CAN enter, but will take effects
        IsTransparent = true;

        // Set defaults per type
        switch (type)
        {
            case LiquidType.Lava: DamagePerTurn = 10; break;
            case LiquidType.Acid: DamagePerTurn = 5; break;
            case LiquidType.Mud: DamagePerTurn = 0; break; // slows instead
            case LiquidType.Water: DamagePerTurn = 0; break;
        }
    }

    public override void OnEnter(GameObject entity)
    {
        _occupant = entity;
        ApplyEffect(entity);
    }

    public override void OnExit(GameObject entity)
    {
        _occupant = null;
    }

    public override void OnTick()
    {
        if (_occupant != null)
            ApplyEffect(_occupant);
    }

    protected virtual void ApplyEffect(GameObject entity)
    {
        if (DamagePerTurn > 0)
        {
            Debug.Log($"{entity.name} takes {DamagePerTurn} {LiquidType} damage at {GridPosition}");
            OnLiquidDamage?.Invoke(entity, DamagePerTurn);
        }
    }
}