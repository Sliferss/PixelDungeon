using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile : MonoBehaviour
{
    public bool IsWalkable = false;
    public bool IsSolid = false;
    public bool IsSeen = false;

    public TileBase GroundLayer;
    public TileBase FloorLayer;
    public List<TileBase> StatusLayer = new List<TileBase>();
    public List<TileBase> ItemLayer = new List<TileBase>();

    public TileBase Character;

    public bool IsOccupied()
    {
        return Character != null;
    }

    public void OnEnter()
    {
        return;
    }

    public void OnExit() { 
        return; 
    }

    public void OnStartTurn()
    {
        return;
    }

    public void OnInteract()
    {
        return;
    }
}