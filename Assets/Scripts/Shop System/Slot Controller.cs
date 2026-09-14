using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static HandController;

public class SlotController : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] protected private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemCountText;
    private MyInventory _item;

    [SerializeField] private GameObject _ItemImageObject;
    private GameObject _draggingObject;
    private Transform _canvasTransform;
    private HandController _hand;
    private ShopSceneManager _shopSceneManager;

    private int _slotIndex;

    public MyInventory MyItem { get => _item; private set => _item = value; }


    private void Start()
    {
        _shopSceneManager = FindFirstObjectByType<ShopSceneManager>();
        _canvasTransform = FindFirstObjectByType<Canvas>().transform;
        _hand = FindFirstObjectByType<HandController>();
        if(MyItem == null) SetItem(null);
    }

    public void Init(int index)
    {
        _slotIndex = index;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (MyItem == null) return;
        _hand.SetDropped(false);

        _draggingObject = Instantiate(_ItemImageObject, _canvasTransform);
        _draggingObject.transform.SetAsLastSibling();
        _itemImage.color = Color.gray;
        _hand.SetGrabbingItem(MyItem);
        _hand.SetDragIndex(_slotIndex);
        _hand.SetDragSource(DragSource.Inventory);
        
        Image dragImage = _draggingObject.GetComponent<Image>();

        if (dragImage != null)
        {
            dragImage.sprite = MyItem._itemImage;

            dragImage.color = Color.white;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (MyItem == null) return;

        _draggingObject.transform.position = _hand.transform.position;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (!_hand.IsHavingItem()) return;

        MyInventory handItem = _hand.GetGrabbingItem();

        if (handItem == null) return;

        DragSource source = _hand.GetDragSource();

        _hand.SetDropIndex(_slotIndex);

        MyInventory oldItem = MyItem;


        // ==================================================
        // Sell Å® Inventory
        // ==================================================

        if (source == DragSource.Sell)
        {
            if (oldItem == null)
            {
                _inventory.AddInventory(handItem);

                _shopSceneManager.MySellSlot.SetItem(null);
            }
            else
            {
                _shopSceneManager.MySellSlot.SetItem(oldItem);

                _inventory.ReplaceInventory(_slotIndex, handItem);
            }

            _hand.SetDropped(true);

            _hand.Clear();

            _shopSceneManager.ImageUpData();

            return;
        }

        if (handItem._itemType != MyInventory.ItemType.Inventory) return;

        (int dragIndex, int dropIndex) = _hand.GetIndex();


        // ==================================================
        // Attack Å® Inventory
        // ==================================================

        if (source == DragSource.Attack)
        {
            dragIndex -= 30;

            MagicBaseData attackMagic = _inventory.AttackData[dragIndex];

            if (attackMagic == null) return;

            if (oldItem == null)
            {
                MyInventory newItem = new MyInventory(attackMagic, MyInventory.ItemType.Inventory);

                newItem._count = 1;

                _inventory.AddInventory(newItem);

                _inventory.AttackData[dragIndex] = null;
            }
            else if (oldItem._item is MagicBaseData inventoryMagic)
            {
                _inventory.AttackData[dragIndex] = inventoryMagic;

                MyInventory newItem = new MyInventory(attackMagic, MyInventory.ItemType.Inventory);

                newItem._count = 1;

                _inventory.ReplaceInventory(dropIndex, newItem);
            }
            else
            {
                MyInventory newItem = new MyInventory(attackMagic, MyInventory.ItemType.Inventory);

                newItem._count = 1;

                _inventory.AddInventory(newItem);

                _inventory.AttackData[dragIndex] = null;
            }

            _hand.SetDropped(true);

            _hand.Clear();

            _shopSceneManager.ImageUpData();

            return;
        }


        // ==================================================
        // Inventory Å® Inventory
        // ==================================================

        if (source == DragSource.Inventory)
        {
            if (dropIndex >= _inventory.GetInventory().Count) return;
            _inventory.ChangeInventory(dragIndex, dropIndex);

            _hand.SetDropped(true);

            _hand.Clear();

            _shopSceneManager.ImageUpData();

            return;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggingObject != null)
        {
            Destroy(_draggingObject);
        }

        if (!_hand.IsDropped())
        {
            SetItem(MyItem);
        }

        _shopSceneManager.ImageUpData();
    }

    public virtual void SetItem(MyInventory item)
    {
        MyItem = item;
        if (_item != null)
        {
            _itemImage.color = new Color(1, 1, 1, 1);
            _itemImage.sprite = _item._itemImage;
            if(_itemCountText != null) _itemCountText.text = "Å~ " + _item._count.ToString();
        }
        else
        {
            _itemImage.color = new Color(0, 0, 0, 0);
            if(_itemCountText != null) _itemCountText.text = null;
        }
    }
}
