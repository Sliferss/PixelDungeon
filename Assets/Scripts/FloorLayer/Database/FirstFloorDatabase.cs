using UnityEngine;

[CreateAssetMenu(fileName = "FirstFloorDatabase", menuName = "Databases/Floor/FirstFloorDatabase")]
public class FirstFloorDatabase : LayerDatabaseBase
{
    public GameObject Floor;

    public override LayerBase GetLayer(GameObject groundObject)
    {
        return groundObject.GetComponent<LayerBase>();
    }
}
