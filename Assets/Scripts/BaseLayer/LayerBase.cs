using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class LayerBase : MonoBehaviour
{
    public TileBase Tile;

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnStartTurn() { }
    public virtual void OnInteract() { }

    public virtual bool IsWalkable() { return false; }
    public virtual bool IsSolid() { return false; }
}