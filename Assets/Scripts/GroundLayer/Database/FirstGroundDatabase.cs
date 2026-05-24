using UnityEngine;

[CreateAssetMenu(fileName = "FirstGroundDatabase", menuName = "Databases/Ground/FirstGroundDatabase")]
public class FirstGroundDatabase : LayerDatabaseBase
{
    public GameObject Ground;

    public override LayerBase GetLayer(GameObject groundObject)
    {
        return groundObject.GetComponent<LayerBase>();
    }
}