using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class GroundLayerBase : MonoBehaviour
{
    public TileBase GroundLayer;

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnStartTurn() { }
    public virtual void OnInteract() { }
}