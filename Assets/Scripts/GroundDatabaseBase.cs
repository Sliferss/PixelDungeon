using UnityEngine;

public abstract class GroundDatabaseBase : ScriptableObject
{
    public abstract GroundLayerBase GetGround(GameObject groundObject);
}