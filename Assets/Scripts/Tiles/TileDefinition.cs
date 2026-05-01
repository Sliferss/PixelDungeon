using UnityEngine;

public abstract class TileDefinition : ScriptableObject
{
    public string Id;
    public Sprite Sprite;
    public bool IsWalkable = true;
    public int MovementCost = 1;
}