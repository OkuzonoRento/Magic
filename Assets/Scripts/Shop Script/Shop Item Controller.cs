using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopItemController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ShopSceneManager _shopManager;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private HandController _hand;
    [SerializeField] protected Image _itemImage;
    [SerializeField] private GameObject _ItemImageObject;
    [SerializeField] private Sprite _soldOutImage;
    [SerializeField] private TextMeshProUGUI _itemCostText;

    private MagicBaseData _item;
    private GameObject _draggingObject;

    private bool _isSelected;
    private bool _isSoldout;

    public MagicBaseData MyShopItem { get => _item; private set => _item = value; }

    public bool MyIsSelected { get => _isSelected; set => _isSelected = value; }

    public bool MyIsSoldOut { get => _isSoldout; set => _isSoldout = value; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isSelected) return;

        if (_isSoldout) return;

        if (MyShopItem == null) return;

        _hand.SetDropped(false);

        _hand.SetDragSource(HandController.DragSource.Shop);

        _draggingObject = Instantiate(_ItemImageObject, _canvasTransform);

        _draggingObject.transform.SetAsLastSibling();

        Image dragImage = _draggingObject.GetComponent<Image>();

        if (dragImage != null)
        {
            dragImage.sprite = MyShopItem.GetItemImage();

            dragImage.color = Color.white;

            dragImage.raycastTarget = false;
        }

        _hand.SetGrabbingItem(new MyInventory(MyShopItem, MyInventory.ItemType.Shop));

        _hand.SetFromShopSlot(this);

        SetSelected(true);
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

        // ====================================
        // ÉhÉçÉbÉvé∏îs
        // ====================================

        if (!_hand.IsDropped())
        {
            SetSelected(false);

            _hand.Clear();
        }
    }

    public void SetItem(MagicBaseData item)
    {
        MyShopItem = item;

        if (item != null)
        {
            _itemImage.sprite = item.GetItemImage();

            _itemImage.color = Color.white;
        }
        else
        {
            _itemImage.sprite = null;

            _itemImage.color = Color.clear;
        }
    }

    public void SoldOut()
    {
        _isSoldout = true;

        _isSelected = false;

        _itemImage.color = Color.white;

        _itemImage.sprite = _soldOutImage;

        MyShopItem = null;

        _itemCostText.text = "Sold Out";
    }

    public void SetSelected(bool value)
    {
        _isSelected = value;

        if (_isSoldout) return;

        if (_isSelected)
        {
            _itemImage.color = Color.gray;
        }
        else
        {
            _itemImage.color = Color.white;
        }
    }
}