using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }
    public TerrainData Terrain { get; private set; }
    public Unit Occupant { get; private set; }

    public Trap Trap { get; private set; }

    public bool IsWalkable =>
        Terrain.IsWalkable && Occupant == null;

    public void Initialize(Vector2Int position, TerrainData terrain)
    {
        GridPosition = position;
        Terrain = terrain;

        gameObject.name = $"{terrain.terrainName} ({position.x}, {position.y})";
    }

    public void SetOccupant(Unit unit)
    {
        Occupant = unit;
    }

    public void SetTrap(Trap trap)
    {
        Trap = trap;
    }

}