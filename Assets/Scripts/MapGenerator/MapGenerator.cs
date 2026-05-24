using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Scene References")]
    public GridManager GridManager;
    public Room Room;

    [Header("Generation Settings")]
    public Vector2Int RoomOrigin = new Vector2Int(1, 1);

    private void Awake()
    {
        ValidateReferences();
    }

    private void Start()
    {
        GenerateMap();
    }

    // -----------------------------
    // Validate setup
    // -----------------------------
    private void ValidateReferences()
    {
        if (GridManager == null)
        {
            Debug.LogError("MapGenerator: GridManager is not assigned.");
        }

        if (Room == null)
        {
            Debug.LogError("MapGenerator: Room is not assigned.");
        }
    }

    // -----------------------------
    // Main entry point
    // -----------------------------
    public void GenerateMap()
    {
        if (GridManager == null || Room == null)
        {
            Debug.LogError("MapGenerator: Missing values assigned.");
            return;
        }

        Room.Generate(GridManager, RoomOrigin);
        GridManager.RenderAll();

        // Log the chosen entrance tile
        Debug.Log($"[MapGenerator] Room entrance is at: {Room.EntranceOrigin}");
    }

    // -----------------------------
    // Optional manual regen hook
    // -----------------------------
    [ContextMenu("Regenerate Map")]
    public void Regenerate()
    {
        GenerateMap();
    }
}