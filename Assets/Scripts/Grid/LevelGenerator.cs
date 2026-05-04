using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GridManager gridManager;
    public TerrainDatabase terrainDatabase;
    [SerializeField] private TrapDatabase trapDatabase;
    [Range(0f, 1f)] public float trapChance = 0.1f;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        gridManager.Initialize();

        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                TerrainData terrain = ChooseTerrain(pos);
                GameObject prefab = terrain.GetRandomPrefab();

                GameObject tileObject = Instantiate(
                    prefab,
                    gridManager.GridToWorld(pos),
                    Quaternion.identity,
                    transform
                );

                Tile tile = tileObject.GetComponent<Tile>();
                tile.Initialize(pos, terrain);

                gridManager.SetTile(pos, tile);
                if (terrain == terrainDatabase.floor && Random.value < trapChance)
                {
                    GameObject trapPrefab = trapDatabase.GetRandomTrap();

                    if (trapPrefab != null)
                    {
                        GameObject trapObj = Instantiate(
                            trapPrefab,
                            tileObject.transform.position,
                            Quaternion.identity,
                            tileObject.transform
                        );

                        Trap trap = trapObj.GetComponent<Trap>();
                        tile.SetTrap(trap);
                    }
                }
            }
        }
    }

    private TerrainData ChooseTerrain(Vector2Int pos)
    {
        bool isBorder =
            pos.x == 0 ||
            pos.y == 0 ||
            pos.x == gridManager.width - 1 ||
            pos.y == gridManager.height - 1;

        return isBorder
            ? terrainDatabase.wall
            : terrainDatabase.floor;
    }
}