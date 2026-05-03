using UnityEngine;

public class Unit : MonoBehaviour
{
    public Tile CurrentTile { get; private set; }

    public void Place(Tile tile)
    {
        CurrentTile?.SetOccupant(null);
        CurrentTile = tile;
        tile.SetOccupant(this);
        transform.position = tile.transform.position;
    }
}
