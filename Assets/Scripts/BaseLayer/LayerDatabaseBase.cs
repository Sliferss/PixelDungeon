using UnityEngine;

public abstract class LayerDatabaseBase : ScriptableObject
{
    public abstract LayerBase GetLayer(GameObject groundObject);
}