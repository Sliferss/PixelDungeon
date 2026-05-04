using UnityEngine;

[CreateAssetMenu(menuName = "Game/Terrain")]
public class TerrainData : ScriptableObject
{
    [Header("Info")]
    public string terrainName;

    [Header("Gameplay")]
    public bool IsWalkable = true;
    public int movementCost = 1;

    [Header("Visuals")]
    public GameObject[] tilePrefabs;

    public GameObject GetRandomPrefab()
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0)
            return null;

        return tilePrefabs[Random.Range(0, tilePrefabs.Length)];
    }
}