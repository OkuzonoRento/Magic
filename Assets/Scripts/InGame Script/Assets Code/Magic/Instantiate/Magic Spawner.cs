using System;
using UnityEngine;

[Serializable]
public abstract class MagicSpawner : ScriptableObject
{
    public abstract void MagicInstantiate(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent, int count, float angle, MagicBaseData data, Transform target);
}
