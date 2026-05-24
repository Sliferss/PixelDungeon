using UnityEngine;

public class GenericFloor : LayerBase
{
    public override bool IsWalkable() { return false; }
    public override bool IsSolid() { return true; }
}
