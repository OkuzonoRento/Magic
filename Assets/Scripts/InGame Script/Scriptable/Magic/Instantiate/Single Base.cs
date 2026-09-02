using UnityEngine;

[CreateAssetMenu(fileName = "Single", menuName = "My Create Asset / Shot Type / Single shot")]
public class SingleBase : MagicSpawner
{
    public override void MagicInstantiate(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent, int count, float angle, MagicBaseData data, Transform target)
    {
        GameObject obj = Instantiate(prefab, parent);

        Vector3 setPos = pos;
        setPos.y = 0.75f;

        obj.transform.position = setPos;
        obj.transform.rotation = rot;

        MagicController controller = obj.GetComponent<MagicController>();
        controller._baseData = data;
        controller._target = target;
    }
}
