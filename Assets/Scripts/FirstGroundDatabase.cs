using UnityEngine;

[CreateAssetMenu(fileName = "FirstGroundDatabase", menuName = "Databases/Ground/FirstGroundDatabase")]
public class FirstGroundDatabase : GroundDatabaseBase
{
    public GameObject Ground;

    public override GroundLayerBase GetGround(GameObject groundObject)
    {
        return groundObject.GetComponent<GroundLayerBase>();
    }
}