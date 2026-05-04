using UnityEngine;

public class Unit : MonoBehaviour
{
    public Tile CurrentTile { get; private set; }
    public int health = 10;

    public void Place(Tile tile)
    {
        CurrentTile?.SetOccupant(null);

        CurrentTile = tile;
        tile.SetOccupant(this);

        transform.position = tile.transform.position;

        // Trigger trap if present
        if (tile.Trap != null)
        {
            tile.Trap.Trigger(this);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"{name} took {amount} damage. HP: {health}");

        if (health <= 0)
        {
            Debug.Log($"{name} died.");
            Destroy(gameObject);
        }
    }
}
