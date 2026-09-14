using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static HandController;

public class SellSlotController : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    [SerializeField] private ShopSceneManager _shopManager;
    [SerializeField] private HandController _hand;

    [Header("Drag")]
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private GameObject _itemImageObject;

    [Header("UI")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private Slider _selectCount;
    [SerializeField] private TextMeshProUGUI _selectInformationText;

    [SerializeField] private List<TextMeshProUGUI> _costText;

    private MyInventory _selectItem;

    private int totalPrice;
    private GameObject _draggingObject;

    public MyInventory MySelectItem { get => _selectItem; set => _selectItem = value; }
    public int MySelectCount => (int)_selectCount.value;


    private void FixedUpdate()
    {
        if (_selectItem == null)
        {
            _selectInformationText.text = "No Select Items";
            return;
        }

        totalPrice = _selectItem._amount * (int)_selectCount.value;
        _selectInformationText.text = $"Select : {(int)_selectCount.value}\n" + $"Credit : +{totalPrice}";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_selectItem == null) return;

        _hand.SetDropped(false);

        _hand.SetDragSource(DragSource.Sell);

        _draggingObject = Instantiate(_itemImageObject, _canvasTransform);

        _draggingObject.transform.SetAsLastSibling();

        _hand.SetGrabbingItem(_selectItem);

        Image dragImage = _draggingObject.GetComponent<Image>();

        if (dragImage != null)
        {
            dragImage.sprite = _selectItem._itemImage;

            dragImage.color = Color.white;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_selectItem == null) return;

        if (_draggingObject != null)
        {
            _draggingObject.transform.position = _hand.transform.position;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_hand.IsHavingItem()) return;

        MyInventory handItem = _hand.GetGrabbingItem();

        if (handItem == null) return;

        DragSource source = _hand.GetDragSource();


        // =====================================
        // ShopItem ¨ Sell
        // =====================================

        if (handItem._itemType == MyInventory.ItemType.Shop)
        {
            return;
        }

        // =====================================
        // Sell ¨ Sell
        // =====================================

        if (source == DragSource.Sell)
        {
            return;
        }


        // =====================================
        // ‹ó‚¶‚á‚È‚¢ê‡‚Í“ü‚ê‘Ö‚¦
        // =====================================

        MyInventory oldSellItem = _selectItem;


        // =====================================
        // Inventory ¨ Sell
        // =====================================

        if (source == DragSource.Inventory)
        {

            if (oldSellItem != null)
            {
                _shopManager.MyInventory.AddInventory(oldSellItem);
            }

            _shopManager.MyInventory.RemoveInventory(handItem);

            SetItem(handItem);
        }


        // =====================================
        // Attack ¨ Sell
        // =====================================

        else if (source == DragSource.Attack)
        {

            (int dragIndex, int dropIndex) = _hand.GetIndex();

            dragIndex -= 30;

            if (oldSellItem != null)
            {
                if (oldSellItem._item is MagicBaseData oldMagic)
                {
                    _shopManager.MyInventory.AttackData[dragIndex] = oldMagic;
                }
                else
                {
                    _shopManager.MyInventory.AddInventory(oldSellItem);

                    _shopManager.MyInventory.AttackData[dragIndex] = null;
                }
            }
            else
            {
                _shopManager.MyInventory.AttackData[dragIndex] = null;
            }

            handItem._count = 1;

            SetItem(handItem);

        }

        _hand.SetDropped(true);

        _hand.Clear();

        _shopManager.ImageUpData();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggingObject != null)
        {
            Destroy(_draggingObject);
        }

        if (!_hand.IsDropped())
        {
            SetItem(_selectItem);
        }

        _shopManager.ImageUpData();
    }

    public void SetItem(MyInventory item)
    {
        _selectItem = item;

        if (_selectItem != null)
        {
            _selectCount.maxValue = _selectItem._count;
            _selectCount.value = 1;

            _itemImage.color = Color.white;
            _itemImage.sprite = _selectItem._itemImage;
        }
        else
        {
            _selectCount.value = 0;

            _itemImage.color = Color.clear;
            _itemImage.sprite = null;
        }
    }

    public void OnSellButton()
    {
        if (_selectItem == null) return;

        int sellCount = (int)_selectCount.value;

        _shopManager.MyInventory.AddCredit(_selectItem._amount * sellCount);

        int remainCount = _selectItem._count - sellCount;

        if (remainCount > 0)
        {
            MyInventory remainItem = new MyInventory(_selectItem._item, MyInventory.ItemType.Inventory);

            remainItem._count = remainCount;

            _shopManager.MyInventory.AddInventory(remainItem);
        }

        SetItem(null);

        _shopManager.ImageUpData();
    }

    public void OnExitButton()
    {
        if (_selectItem != null)
        {
            _shopManager.MyInventory.AddInventory(_selectItem);
            SetItem(null);
            _selectItem = null;
            _shopManager.ImageUpData();
        }

        SceneManager._instance.ChangeScene();
    }
}
