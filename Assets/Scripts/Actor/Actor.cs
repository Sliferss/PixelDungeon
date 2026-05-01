using UnityEngine;
using System.Collections.Generic;

public abstract class Actor : MonoBehaviour
{
    public GridPosition Position;

    protected MovementController movementController;

    protected virtual void Awake()
    {
        movementController = GetComponent<MovementController>();
    }

    public virtual void Initialize(GridPosition position)
    {
        Position = position;
        transform.position = GridManager.Instance.GridToWorld(position);
    }

    public virtual void MoveAlongPath(List<GridPosition> path)
    {
        movementController.FollowPath(path);
    }

    public virtual void OnTurnStart() { }
    public virtual void OnTurnEnd() { }
}