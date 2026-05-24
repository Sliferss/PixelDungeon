using UnityEngine;

[CreateAssetMenu(fileName = "FirstFloorDatabase", menuName = "Scriptable Objects/FirstFloorDatabase")]
public class FirstFloorDatabase : LayerDatabaseBase
{
    public GameObject Ground;

    public override LayerBase GetLayer(GameObject groundObject)
    {
        return groundObject.GetComponent<LayerBase>();
    }
}
