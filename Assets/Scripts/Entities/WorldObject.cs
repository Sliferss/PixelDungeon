using UnityEngine;

public abstract class WorldObject : MonoBehaviour
{
    public GridPosition Position;

    public bool BlocksMovement = false;
    public bool BlocksVision = false;

    public virtual void Initialize(GridPosition position)
    {
        Position = position;
        transform.position = GridManager.Instance.GridToWorld(position);
    }

    public virtual void Interact(Actor actor) { }

    public virtual void OnStepped(Actor actor) { }
}