using System;
using UnityEngine;


[Serializable]
public abstract class Item : ScriptableObject
{
    [SerializeField] private string _itemName;
    [SerializeField] private GameObject _dropObject;
    [SerializeField] private Sprite _itemImage;
    [SerializeField, Range(0, 1)] private float _dropRate = 0;
    [SerializeField, Min(0)] private int _amount;
    [SerializeField] private bool _isCanStack = true;

    public bool _CanStack => _isCanStack;

    public Sprite GetItemImage()
    {
        return _itemImage;
    }

    public float GetDropRate()
    {
        return _dropRate;
    }

    public GameObject GetDropObject()
    {
        return _dropObject;
    }

    public int GetAmount()
    {
        return _amount;
    }
}
