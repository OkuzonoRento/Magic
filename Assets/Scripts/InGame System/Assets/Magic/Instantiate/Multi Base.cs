using UnityEngine;

[CreateAssetMenu(fileName = "Multi", menuName = "My Create Asset / Shot Type / Multi shot")]
public class MultiBase : MagicSpawner
{
    public override void MagicInstantiate(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent, int count, float angle, MagicBaseData data, Transform target)
    {
        float step = angle / (count - 1);
        float startAngle = -angle / 2.0f;
        for (int c = 0; c < count; c++)
        {
            GameObject obj = Instantiate(prefab, parent);

            float offset = startAngle + step * c;
            Vector3 setPos = pos;
            setPos.y = 0.75f;

            obj.transform.position = setPos;
            obj.transform.rotation = rot * Quaternion.Euler(0, offset, 0);

            MagicController controller = obj.GetComponent<MagicController>();
            controller._baseData = data;
            controller._target = target;
        }
    }
}
