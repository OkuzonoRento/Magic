using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    private MyInventory _grabbingItem;

    private bool _isDropped;
    private int _dragIndex;
    private int _dropIndex;
    private ShopItemController _fromShopSlot;
    [SerializeField] private DragSource _dragSource;

    public enum DragSource
    {
        Inventory,
        Attack,
        Shop,
        Buy,
        Sell
    }

    void FixedUpdate()
    {
        Vector3 cameraPosition = Mouse.current.position.ReadValue();
        cameraPosition.z = 10.0f;
        Vector2 position = Camera.main.ScreenToWorldPoint(cameraPosition);
        transform.position = position;
    }

    public MyInventory GetGrabbingItem()
    {
        MyInventory oldItem = _grabbingItem;
        return oldItem;
    }

    public void SetGrabbingItem(MyInventory item)
    {
        _grabbingItem = item;
    }

    public void SetDragIndex(int dragIndex)
    {
        _dragIndex = dragIndex;
        _dropIndex = -1;
    }

    public void SetDropIndex(int dropIndex)
    {
        _dropIndex = dropIndex;
    }

    public (int, int) GetIndex()
    {
        return (_dragIndex, _dropIndex);
    }

    public bool IsHavingItem()
    {
        return _grabbingItem != null;
    }

    public void SetDropped(bool value)
    {
        _isDropped = value;
    }

    public bool IsDropped()
    {
        return _isDropped;
    }

    public void SetFromShopSlot(ShopItemController slot)
    {
        _fromShopSlot = slot;
    }

    public ShopItemController GetFromShopSlot()
    {
        return _fromShopSlot;
    }

    public void SetDragSource(DragSource source)
    {
        _dragSource = source;
    }

    public DragSource GetDragSource()
    {
        return _dragSource;
    }

    public void Clear()
    {
        _grabbingItem = null;

        _dragIndex = -1;
        _dropIndex = -1;

        _fromShopSlot = null;

        //_dragSource = DragSource.None;
    }
}
