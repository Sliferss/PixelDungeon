using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Scene References")]
    public GridManager GridManager;
    public Room RoomPrefab;

    [Header("Generation Settings")]
    public int RoomCount = 5;

    private List<Room> _spawnedRooms = new List<Room>();

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
            Debug.LogError("MapGenerator: GridManager is not assigned.");

        if (RoomPrefab == null)
            Debug.LogError("MapGenerator: RoomPrefab is not assigned.");
    }

    // -----------------------------
    // Main entry point
    // -----------------------------
    public void GenerateMap()
    {
        if (GridManager == null || RoomPrefab == null)
        {
            Debug.LogError("MapGenerator: Missing values assigned.");
            return;
        }

        _spawnedRooms.Clear();

        Vector2Int firstOrigin = new Vector2Int(GridManager.Width / 2, GridManager.Height / 2);
        TrySpawnRoom(firstOrigin);

        for (int i = 1; i < RoomCount; i++)
        {
            Vector2Int? origin = FindOriginNearExistingRooms();
            if (origin.HasValue)
                TrySpawnRoom(origin.Value);
            else
                Debug.LogWarning($"MapGenerator: Could not find space for room {i + 1}.");
        }

        // Sort smallest to largest before border/door passes
        _spawnedRooms.Sort((a, b) => a.OccupiedPositions.Count.CompareTo(b.OccupiedPositions.Count));

        PlaceFloorBorders();
        PlaceDoors();

        GridManager.RenderAll();
    }

    // -----------------------------
    // Spawn a room at origin
    // -----------------------------
    private bool TrySpawnRoom(Vector2Int origin)
    {
        Room room = Instantiate(RoomPrefab, transform);
        room.Generate(GridManager, origin);

        // Check if the room was actually placed (has any tiles)
        if (!HasPlacedTiles(room))
        {
            Destroy(room.gameObject);
            return false;
        }

        _spawnedRooms.Add(room);
        return true;
    }

    private bool HasPlacedTiles(Room room)
    {
        foreach (var pos in room.OccupiedPositions)
        {
            Tile t = GridManager.GetTile(pos.x, pos.y);
            if (t != null && t.Room == room)
                return true;
        }
        return false;
    }

    // -----------------------------
    // Find a valid origin near existing rooms
    // -----------------------------
    private Vector2Int? FindOriginNearExistingRooms()
    {
        if (_spawnedRooms.Count == 0)
            return null;

        List<Room> shuffled = new List<Room>(_spawnedRooms);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        foreach (Room existing in shuffled)
        {
            HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>(existing.OccupiedPositions);
            List<Vector2Int> perimeter = new List<Vector2Int>();

            foreach (Vector2Int pos in existing.OccupiedPositions)
            {
                Vector2Int[] neighbours = {
                new Vector2Int(pos.x + 1, pos.y),
                new Vector2Int(pos.x - 1, pos.y),
                new Vector2Int(pos.x,     pos.y + 1),
                new Vector2Int(pos.x,     pos.y - 1),
            };

                foreach (Vector2Int n in neighbours)
                {
                    if (!roomTiles.Contains(n) && GridManager.IsInBounds(n.x, n.y))
                        perimeter.Add(n);
                }
            }

            for (int i = perimeter.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (perimeter[i], perimeter[j]) = (perimeter[j], perimeter[i]);
            }

            foreach (Vector2Int candidate in perimeter)
            {
                if (IsValidOrigin(candidate))
                    return candidate;
            }
        }

        return null;
    }

    // A candidate origin is valid only if the tile itself is free AND
    // none of its 8 neighbours already belong to a different room.
    // This guarantees at least a 1-tile gap between any two room footprints,
    // preventing double-walls from forming where rooms sit back-to-back.
    private bool IsValidOrigin(Vector2Int origin)
    {
        int[] dx8 = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy8 = { -1, -1, -1, 0, 0, 1, 1, 1 };

        Tile at = GridManager.GetTile(origin.x, origin.y);
        if (at != null && at.Room != null)
            return false;

        for (int i = 0; i < 8; i++)
        {
            int nx = origin.x + dx8[i];
            int ny = origin.y + dy8[i];

            Tile neighbour = GridManager.GetTile(nx, ny);
            if (neighbour == null || neighbour.Room == null)
                continue;

            // This neighbour belongs to a room — is it the room we're expanding from?
            // If it's a DIFFERENT room, reject to avoid double-walling between two existing rooms.
            // Belonging to the room we're attaching to is fine and expected.
            Tile originTile = GridManager.GetTile(origin.x, origin.y);
            // We allow neighbours that belong to the single existing room adjacent to this perimeter point.
            // Reject only if a second distinct room is already nearby.
            foreach (Room spawned in _spawnedRooms)
            {
                bool neighbourBelongsToThis = spawned.OccupiedPositions.Contains(new Vector2Int(nx, ny));
                bool originIsOnPerimeterOfThis = IsOnPerimeterOf(origin, spawned);

                if (neighbourBelongsToThis && !originIsOnPerimeterOfThis)
                    return false;
            }
        }

        return true;
    }

    private bool IsOnPerimeterOf(Vector2Int pos, Room room)
    {
        HashSet<Vector2Int> tiles = new HashSet<Vector2Int>(room.OccupiedPositions);
        Vector2Int[] cardinals = {
        new Vector2Int(pos.x + 1, pos.y),
        new Vector2Int(pos.x - 1, pos.y),
        new Vector2Int(pos.x,     pos.y + 1),
        new Vector2Int(pos.x,     pos.y - 1),
    };

        foreach (Vector2Int c in cardinals)
        {
            if (tiles.Contains(c))
                return true;
        }

        return false;
    }

    // -----------------------------
    // Floor border pass
    // -----------------------------
    private void PlaceFloorBorders()
    {
        int[] dx8 = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy8 = { -1, -1, -1, 0, 0, 1, 1, 1 };

        foreach (Room room in _spawnedRooms)
        {
            if (room.FloorDB == null)
                continue;

            LayerBase floorLayer = room.FloorDB.Floor?.GetComponent<LayerBase>();
            if (floorLayer == null)
                continue;

            foreach (Vector2Int pos in room.OccupiedPositions)
            {
                Tile tile = GridManager.GetTile(pos.x, pos.y);
                if (tile == null || tile.Room != room)
                    continue;

                bool bordersTile = false;
                for (int i = 0; i < 8; i++)
                {
                    int nx = pos.x + dx8[i];
                    int ny = pos.y + dy8[i];

                    Tile neighbour = GridManager.GetTile(nx, ny);

                    // Neighbour has a different room value (null counts as different)
                    if (neighbour == null || neighbour.Room != room && neighbour.FloorLayer != floorLayer)
                    {
                        bordersTile = true;
                        break;
                    }
                }

                if (bordersTile)
                    tile.FloorLayer = floorLayer;
            }
        }
    }

    // -----------------------------
    // Door pass
    // -----------------------------
    private void PlaceDoors()
    {
        // Cardinal direction pairs: axis-aligned neighbours only (no corners)
        // A door tile is one whose two axis-aligned neighbours along one axis
        // both have the floor layer, and whose two neighbours on the other axis
        // do NOT have the floor layer (so it sits in a gap, not a corner)

        foreach (Room room in _spawnedRooms)
        {
            if (room.FloorDB == null)
                continue;

            LayerBase floorLayer = room.FloorDB.Floor?.GetComponent<LayerBase>();
            if (floorLayer == null)
                continue;

            foreach (Vector2Int pos in room.OccupiedPositions)
            {
                Tile tile = GridManager.GetTile(pos.x, pos.y);
                if (tile == null || tile.FloorLayer != floorLayer)
                    continue;

                Tile north = GridManager.GetTile(pos.x, pos.y + 1);
                Tile south = GridManager.GetTile(pos.x, pos.y - 1);
                Tile east = GridManager.GetTile(pos.x + 1, pos.y);
                Tile west = GridManager.GetTile(pos.x - 1, pos.y);

                bool northIsFloor = north?.FloorLayer == floorLayer;
                bool southIsFloor = south?.FloorLayer == floorLayer;
                bool eastIsFloor = east?.FloorLayer == floorLayer;
                bool westIsFloor = west?.FloorLayer == floorLayer;

                // Horizontal wall segment: flanked E/W by floor, NOT flanked N/S
                bool isHorizontalDoor = eastIsFloor && westIsFloor && !northIsFloor && !southIsFloor;
                // Vertical wall segment: flanked N/S by floor, NOT flanked E/W
                bool isVerticalDoor = northIsFloor && southIsFloor && !eastIsFloor && !westIsFloor;

                if (isHorizontalDoor || isVerticalDoor)
                {
                    // TODO: place actual door tile/object here
                    Debug.Log($"Door at {pos} ({'H': 'V'})");
                }
            }
        }
    }

}