using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/Wall")]
public class WallDefinition : ScriptableObject
{
    public string Id;
    public Sprite Sprite;

    public bool BlocksMovement = true;
    public bool BlocksVision = true;
}