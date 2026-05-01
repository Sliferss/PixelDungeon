using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Automatically creates the initial dungeon scene hierarchy.
/// Attach this to an empty GameObject in a blank scene,
/// then press Play once.
/// </summary>
public sealed class SceneBootstrapper : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField] private int mapWidth = 50;
    [SerializeField] private int mapHeight = 50;

    [Header("Player")]
    [SerializeField] private float playerMoveSpeed = 6f;

    private void Awake()
    {
        BuildScene();
    }

    private void BuildScene()
    {
        CreateMainCamera();
        CreateSystems();
        CreateTilemapRoot();
        CreatePlayer();

        Debug.Log("Dungeon scene successfully generated.");
    }

    private void CreateMainCamera()
    {
        if (Camera.main != null)
            return;

        GameObject cameraGO = new("Main Camera");

        Camera cam = cameraGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 12f;
        cam.clearFlags = CameraClearFlags.SolidColor;

        cameraGO.tag = "MainCamera";
        cameraGO.transform.position = new Vector3(0f, 0f, -10f);
    }

    private void CreateSystems()
    {
        CreateSystem<GridManager>("Grid Manager");
        CreateSystem<PathfindingSystem>("Pathfinding System");
        CreateSystem<InputManager>("Input Manager");
        CreateSystem<LevelGenerator>("Level Generator");

        GridManager.Instance.Initialize(mapWidth, mapHeight);
    }

    private void CreateTilemapRoot()
    {
        GameObject gridRoot = new("Grid");

        Grid grid = gridRoot.AddComponent<Grid>();
        grid.cellLayout = GridLayout.CellLayout.Rectangle;

        CreateTilemap(gridRoot.transform, "Ground");
        CreateTilemap(gridRoot.transform, "Walls");
        CreateTilemap(gridRoot.transform, "Overlay");
        CreateTilemap(gridRoot.transform, "Fog");
    }

    private void CreateTilemap(Transform parent, string name)
    {
        GameObject tilemapGO = new(name);

        tilemapGO.transform.SetParent(parent);

        tilemapGO.AddComponent<Tilemap>();
        tilemapGO.AddComponent<TilemapRenderer>();
    }

    private void CreatePlayer()
    {
        GameObject playerGO = new("Player");

        playerGO.transform.position = Vector3.zero;

        playerGO.AddComponent<SpriteRenderer>();
        playerGO.AddComponent<MovementController>();

        PlayerController player = playerGO.AddComponent<PlayerController>();
        player.Initialize(new GridPosition(0, 0));

        MovementController movement = playerGO.GetComponent<MovementController>();
        movement.MoveSpeed = playerMoveSpeed;
    }

    private T CreateSystem<T>(string objectName) where T : Component
    {
        T existing = FindFirstObjectByType<T>();

        if (existing != null)
            return existing;

        GameObject go = new(objectName);
        return go.AddComponent<T>();
    }
}