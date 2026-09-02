using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static HandController;

public class AttackSlotController : MonoBehaviour, IBeginDragHandler ,IDragHandler, IDropHandler, IEndDragHandler
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] protected private Image _itemImage;
    private MyInventory _magic;

    [SerializeField] private GameObject _ItemImageObject;
    private GameObject _draggingObject;
    private Transform _canvasTransform;
    private HandController _hand;
    private ShopSceneManager _shopSceneManager;

    private int _slotIndex;

    public MyInventory MyMagic { get => _magic; private set => _magic = value; }
    public HandController MyHandController { get => _hand; }

    private void Start()
    {
        _shopSceneManager = FindFirstObjectByType<ShopSceneManager>();
        _canvasTransform = FindFirstObjectByType<Canvas>().transform;
        _hand = FindFirstObjectByType<HandController>();
        if(MyMagic == null) SetMagic(null);
    }

    public void Init(int index)
    {
        _slotIndex = index;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (MyMagic == null) return;
        _hand.SetDragSource(DragSource.Attack);
        _hand.SetDropped(false);

        _draggingObject = Instantiate(_ItemImageObject, _canvasTransform);
        _draggingObject.transform.SetAsLastSibling();
        _itemImage.color = Color.gray;
        _hand.SetGrabbingItem(MyMagic);
        _hand.SetDragIndex(_slotIndex);

        Image dragImage = _draggingObject.GetComponent<Image>();

        if (dragImage != null)
        {
            dragImage.sprite = MyMagic._itemImage;
            dragImage.color = Color.white;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (MyMagic == null) return;
        _draggingObject.transform.position = _hand.transform.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_hand.IsHavingItem()) return;

        MyInventory handItem = _hand.GetGrabbingItem();

        if (handItem == null) return;

        if (handItem._item is not MagicBaseData) return;

        _hand.SetDropIndex(_slotIndex);

        int attackIndex = _slotIndex - 30;

        MagicBaseData oldMagic = _inventory.AttackData[attackIndex];

        // ===================================================
        // Sell Å® Attack
        // ===================================================

        if (_hand.GetDragSource() == DragSource.Sell)
        {
            MagicBaseData sellMagic = (MagicBaseData)handItem._item;

            _inventory.AttackData[attackIndex] = sellMagic;

            if (oldMagic != null)
            {
                MyInventory oldItem = new MyInventory(oldMagic, MyInventory.ItemType.Inventory);

                oldItem._count = 1;

                _shopSceneManager.MySellSlot.SetItem(oldItem);
            }
            else
            {
                _shopSceneManager.MySellSlot.SetItem(null);
            }

            _hand.SetDropped(true);

            _hand.Clear();

            _shopSceneManager.ImageUpData();

            return;
        }

        (int dragIndex, int dropIndex)
            = _hand.GetIndex();

        // ===================================================
        // Inventory Å® Attack
        // ===================================================

        if (_hand.GetDragSource() == DragSource.Inventory)
        {
            _inventory.InventoryToAttack(dragIndex, dropIndex);

            _hand.SetDropped(true);

            _hand.Clear();

            _shopSceneManager.ImageUpData();

            return;
        }

        // ===================================================
        // Attack Å® Attack
        // ===================================================

        if (_hand.GetDragSource() == DragSource.Attack)
        {
            _inventory.AttackSlotChange(dragIndex, dropIndex);

            _hand.SetDropped(true);

            _hand.Clear();

            _shopSceneManager.ImageUpData();

            return;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggingObject != null) Destroy(_draggingObject);

        if (!_hand.IsDropped()) SetMagic(MyMagic);

        _shopSceneManager.ImageUpData();
    }

    public void SetMagic(MyInventory magic)
    {
        MyMagic = magic;

        if (_magic != null)
        {
            _itemImage.sprite = _magic._itemImage;

            _itemImage.color = Color.white;
        }
        else
        {
            _itemImage.sprite = null;

            _itemImage.color = Color.clear;
        }
    }
}
