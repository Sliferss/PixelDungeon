using UnityEngine;

public class GenericDoor: LayerBase
{
    public override bool IsWalkable() { return true; }
    public override bool IsSolid() { return true; }
}
