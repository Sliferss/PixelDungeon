using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile
{
    public bool IsWalkable = false;
    public bool IsSolid = false;
    public bool IsSeen = false;

    public LayerBase GroundLayer;
    public LayerBase FloorLayer;

    public List<TileBase> StatusLayer = new List<TileBase>();
    public List<TileBase> ItemLayer = new List<TileBase>();

    public TileBase Character;

    public bool IsOccupied()
    {
        return Character != null;
    }

    public void OnEnter() { }
    public void OnExit() { }
    public void OnStartTurn() { }
    public void OnInteract() { }
}