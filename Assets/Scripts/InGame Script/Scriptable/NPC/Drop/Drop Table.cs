using System;
using UnityEngine;

[Serializable]
public abstract class DropTable : ScriptableObject
{
    [SerializeField] private Item[] _itemTable;

    public Item[] GetItemTable()
    {
        return _itemTable;
    }
}
