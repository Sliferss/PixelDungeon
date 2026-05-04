using UnityEngine;

[CreateAssetMenu(menuName = "Game/Trap Database")]
public class TrapDatabase : ScriptableObject
{
    [Header("Trap Prefabs")]
    public GameObject[] trapPrefabs;

    public GameObject GetRandomTrap()
    {
        if (trapPrefabs == null || trapPrefabs.Length == 0)
            return null;

        return trapPrefabs[Random.Range(0, trapPrefabs.Length)];
    }
}