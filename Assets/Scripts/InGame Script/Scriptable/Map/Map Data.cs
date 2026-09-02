using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Data", menuName = "My Create Asset/MapData")]
public class MapData : ScriptableObject
{
    public MapID _mapID;
    public int _popCount;
    public List<GameObject> _popEnemy;
    public Vector3 _spawnPos;

    public enum MapID
    {
        NomalMap,
        BossMap,
    }
}