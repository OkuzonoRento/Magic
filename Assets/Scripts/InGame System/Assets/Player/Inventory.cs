using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Inventory), true)]
public class InventoryEditor : Editor
{
    [Header("Editor")]
    private Item _addItem;
    public override void OnInspectorGUI()
    {
        Inventory inventory = (Inventory)target;
        _addItem = (Item)EditorGUILayout.ObjectField("Add Item Data", _addItem, typeof(Item), false);

        if (GUILayout.Button("Add Inventory Data"))
        {
            if (_addItem == null) return;

            inventory.AddInventory(_addItem);
            EditorUtility.SetDirty(inventory);
        }

        if (GUILayout.Button("Reset Inventory Data"))
        {
            inventory.ResetInventoryData();
            EditorUtility.SetDirty(inventory);
        }

        GUILayout.Space(20);
        base.OnInspectorGUI();
    }
}
#endif

[System.Serializable]
public class MyInventory
{
    public Item _item;
    public int _count;
    public Sprite _itemImage;
    public int _amount;
    public ItemType _itemType;

    public enum ItemType
    {
        None,
        Inventory,
        Shop
    }

    public MyInventory(Item item)
    {
        _item = item;
        _itemImage = item.GetItemImage();
        _amount = item.GetAmount();

        _itemType = ItemType.Inventory;
    }
    public MyInventory(Item item, ItemType type)
    {
        _item = item;
        _itemImage = _item.GetItemImage();
        _amount = _item.GetAmount();
        _itemType = type;
    }
}

[Serializable]
public abstract class Inventory : ScriptableObject
{
    [SerializeField] private MagicBaseData _defaltMagic;
    [SerializeField] private MagicBaseData[] _attackData = new MagicBaseData[3];
    [SerializeField, Min(0)] private int _credit;

    [Header("Inventory")]
    [SerializeField] private List<MyInventory> _inventory = new();

    //public List<MyInventory> MyInventory { get => _inventory; }
    public MagicBaseData[] AttackData { get => _attackData; set => _attackData = value; }

    public void AddInventory(Item inventory)
    {
        if (inventory._CanStack)
        {
            foreach (MyInventory item in _inventory)
            {
                if (item._item == inventory)
                {
                    item._count++;
                    return;
                }
            }
        }

        if (_inventory.Count <= 30)
        {
            MyInventory addInventory = new(inventory, MyInventory.ItemType.Inventory);
            addInventory._count = 1;
            addInventory._itemType = MyInventory.ItemType.Inventory;
            _inventory.Add(addInventory);
        }
    }

    public void AddInventory(MyInventory item)
    {
        if (item == null) return;

        item._itemType = MyInventory.ItemType.Inventory;
        _inventory.Add(item);
    }

    public MagicBaseData[] GetAttackData()
    {
        return _attackData;
    }

    public int GetCredit()
    {
        return _credit;
    }

    public void AddCredit(int credit)
    {
        _credit += credit;
    }

    public List<MyInventory> GetInventory()
    {
        if (_inventory == null) _inventory = new List<MyInventory>();
        return _inventory;
    }

    public void ChangeInventory(int dragIndex, int dropIndex)
    {
        if (dragIndex < 0 || dragIndex >= _inventory.Count) return;

        if (dropIndex < 0 || dropIndex >= _inventory.Count) return;

        MyInventory oldItem = _inventory[dragIndex];

        _inventory[dragIndex] = _inventory[dropIndex];

        _inventory[dropIndex] = oldItem;
    }

    public void InventoryToAttack(int dragIndex, int dropIndex)
    {
        dropIndex -= 30;

        if (_inventory[dragIndex]._item is not MagicBaseData) return;

        MagicBaseData inventoryMagic = (MagicBaseData)_inventory[dragIndex]._item;

        MagicBaseData oldAttackMagic = _attackData[dropIndex];

        _attackData[dropIndex] = inventoryMagic;

        if (oldAttackMagic == null)
        {
            _inventory.RemoveAt(dragIndex);
        }
        else
        {
            MyInventory oldItem = new MyInventory(oldAttackMagic, MyInventory.ItemType.Inventory);

            oldItem._count = 1;

            _inventory[dragIndex] = oldItem;
        }
    }

    public void AttackToInventory(int dragIndex, int dropIndex)
    {
        dragIndex -= 30;

        if (_attackData[dragIndex] == null) return;
        if (dropIndex < 0 || dropIndex >= _inventory.Count) return;

        if (_inventory[dropIndex]._item is not MagicBaseData inventoryMagic) return;

        MagicBaseData attackMagic = _attackData[dragIndex];

        _attackData[dragIndex] = inventoryMagic;

        MyInventory newItem = new MyInventory(attackMagic, MyInventory.ItemType.Inventory);
        newItem._count = 1;
        newItem._itemType = MyInventory.ItemType.Inventory;

        _inventory[dropIndex] = newItem;
    }

    public void AttackSlotChange(int dragIndex, int dropIndex)
    {
        dragIndex -= 30; dropIndex -= 30;
        MagicBaseData dragItem = _attackData[dragIndex];
        _attackData[dragIndex] = _attackData[dropIndex];
        _attackData[dropIndex] = dragItem;
    }

    public void ResetInventoryData()
    {
        _credit = 9999;
        _inventory.Clear();
        _attackData = new MagicBaseData[3];
        _attackData[0] = _defaltMagic;
    }

    public void RemoveInventory(MyInventory item)
    {
        if (item == null) return;
        _inventory.Remove(item);
    }

    public void ReplaceInventory(int index, MyInventory item)
    {
        if (index < 0 || index >= _inventory.Count) return;

        _inventory[index] = item;
    }
}