using UnityEngine;

[CreateAssetMenu(menuName = "Game/Terrain Database")]
public class TerrainDatabase : ScriptableObject
{
    public TerrainData floor;
    public TerrainData wall;
    public TerrainData door;
    public TerrainData water;
}