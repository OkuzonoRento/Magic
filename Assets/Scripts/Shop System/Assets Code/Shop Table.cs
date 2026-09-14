using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public abstract class ShopTable : ScriptableObject
{
    [SerializeField] private List<MagicBaseData> _table;

    public List<MagicBaseData> GetTable()
    {
        return _table;
    }

    public MagicBaseData RandomSelect()
    {
        int random = UnityEngine.Random.Range(0, _table.Count);

        return _table[random];
    }
}
