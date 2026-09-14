using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuySlotController : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ShopSceneManager _shopManager;
    [SerializeField] private HandController _hand;

    [Header("Drag")]
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private GameObject _itemImageObject;

    [Header("UI")]
    [SerializeField] private Image _itemImage;

    private MyInventory _buyItem;

    private ShopItemController _sourceShopItem;

    private GameObject _draggingObject;

    public MyInventory MyBuyItem
    {
        get => _buyItem;
        private set => _buyItem = value;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_hand.IsHavingItem()) return;

        MyInventory handItem = _hand.GetGrabbingItem();

        if (handItem == null) return;

        if (handItem._itemType != MyInventory.ItemType.Shop) return;

        // ==========================
        // クレジット不足
        // ==========================

        if (_shopManager.MyInventory.GetCredit() < handItem._amount)
        {
            Debug.Log("Credit不足");

            _hand.SetDropped(false);

            return;
        }
        // ==========================
        // ShopItem → BuySlot
        // ==========================

        if (_buyItem != null)
        {
            if (_sourceShopItem != null)
            {
                _sourceShopItem.SetSelected(false);
            }
        }

        SetItem(handItem);

        _sourceShopItem =
            _hand.GetFromShopSlot();

        _hand.SetDropped(true);

        _shopManager.ImageUpData();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_buyItem == null) return;

        _hand.SetDropped(false);

        _hand.SetDragSource(HandController.DragSource.Buy);

        _hand.SetGrabbingItem(_buyItem);

        _draggingObject = Instantiate(_itemImageObject, _canvasTransform);

        _draggingObject.transform.SetAsLastSibling();

        Image dragImage = _draggingObject.GetComponent<Image>();

        if (dragImage != null)
        {
            dragImage.sprite = _buyItem._itemImage;

            dragImage.color = Color.white;

            dragImage.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggingObject == null) return;

        _draggingObject.transform.position = _hand.transform.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggingObject != null)
        {
            Destroy(_draggingObject);
        }

        // ==========================
        // BuySlot → 適当な場所
        // ==========================

        if (!_hand.IsDropped())
        {
            if (_sourceShopItem != null)
            {
                _sourceShopItem.SetSelected(false);
            }

            SetItem(null);
        }
    }

    public void SetItem(MyInventory item)
    {
        MyBuyItem = item;

        if (_buyItem != null)
        {
            _itemImage.sprite =
                _buyItem._itemImage;

            _itemImage.color =
                Color.white;
        }
        else
        {
            _itemImage.sprite = null;

            _itemImage.color =
                Color.clear;
        }
    }

    // ==========================
    // 購入ボタン
    // ==========================

    public void OnBuyButton()
    {
        if (_buyItem == null) return;

        _buyItem._count = 1;

        _shopManager.MyInventory.AddInventory(_buyItem);

        _shopManager.MyInventory.AddCredit(-_buyItem._amount);

        if (_buyItem._item is MagicBaseData magicData)
        {
            _shopManager.MyTable.Remove(magicData);
        }

        if (_sourceShopItem != null)
        {
            _sourceShopItem.SoldOut();
        }

        SetItem(null);

        _sourceShopItem = null;

        _shopManager.ImageUpData();
    }
}