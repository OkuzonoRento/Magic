using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ShopSceneManager : MonoBehaviour
{
    [SerializeField] private SellSlotController _selectItem;
    [Header("Scriptable Object")]
    [SerializeField] private Inventory _inventoryBase;
    [SerializeField] private ShopTable _shopTableBase;

    [Header("Inventory")]
    [SerializeField] private GameObject _slotImageObject;
    [SerializeField] private GameObject _attackImageObject;
    [SerializeField] private Transform _inventoryParent;
    [SerializeField] private Transform _attackInventoryParent;
    [SerializeField, Min(0)] private int _inventorySize;
    private List<MyInventory> _inventoryData;
    private MagicBaseData[] _attackData = new MagicBaseData[3];

    [Header("Shop")]
    [SerializeField] private TextMeshProUGUI _creditText;
    [SerializeField] private TextMeshProUGUI _shopButtonText;
    [SerializeField] private GameObject[] _shopTableObject = new GameObject[3];
    [SerializeField] private TextMeshProUGUI[] _shopTableText = new TextMeshProUGUI[3];
    [System.NonSerialized]public MagicBaseData[] _shopTable = new MagicBaseData[3];

    private List<SlotController> _slotsData = new();
    private List<AttackSlotController> _attackSlotData = new();
    private List<MagicBaseData> _table = new();
    private List<MagicBaseData> _chasTable = new();


    public Inventory MyInventory { get => _inventoryBase; }
    public List<MagicBaseData> MyTable { get => _table; set => _table = value; }
    public SellSlotController MySellSlot { get => _selectItem; }

    private void Awake()
    {
        _inventoryData = _inventoryBase.GetInventory();
        _attackData = _inventoryBase.GetAttackData();

        for (int c = 0; c < _inventorySize; c++)
        {
            GameObject slotObject = Instantiate(_slotImageObject, _inventoryParent);
            SlotController Islot = slotObject.GetComponent<SlotController>();
            _slotsData.Add(Islot);
            Islot.Init(c);
        }

        for (int c = _inventorySize; c < 3 + _inventorySize; c++)
        {
            GameObject slotObject = Instantiate(_attackImageObject, _attackInventoryParent);
            AttackSlotController Aslot = slotObject.GetComponent<AttackSlotController>();
            _attackSlotData.Add(Aslot);
            Aslot.Init(c);
        }

        _table = new(_shopTableBase.GetTable());
        OnRerollButton();
    }

    private void Start()
    {
        ImageUpData();
    }

    public void ImageUpData()
    {
        _creditText.text = _inventoryBase.GetCredit() + " G";
        for (int c = 0; c < _inventorySize; c++)
        {
            _slotsData[c].Init(c);
            if (_inventoryData.Count > c)
            {
                _slotsData[c].SetItem(_inventoryData[c]);
            }
            else
            {
                _slotsData[c].SetItem(null);
            }
        }

        for (int c = 0; c < 3; c++)
        {
            _attackSlotData[c].Init(c + _inventorySize);

            if (_attackData != null && _attackData.Length > c && _attackData[c] != null)
            {
                _attackSlotData[c].SetMagic(new MyInventory(_attackData[c]));
            }
            else
            {
                _attackSlotData[c].SetMagic(null);
            }
        }
    }

    public void OnRerollButton()
    {
        _chasTable = new(_table);

        _selectItem.SetItem(null);

        for (int c = 0; c < 3; c++)
        {
            _shopTable[c] = null;
            _shopTableText[c].text = null;
        }

        for (int c = 0; c < _shopTable.Length; c++)
        {
            ShopItemController slot = _shopTableObject[c].GetComponent<ShopItemController>();
            slot.MyIsSelected = false;
            slot.MyIsSoldOut = false;

            if (_chasTable.Count <= 0)
            {
                slot.SetItem(null);
                _shopTableText[c].text = "Slod Out".ToString();
                break;
            }
            MagicBaseData shopItem = _chasTable[Random.Range(0, _chasTable.Count)];
            _shopTable[c] = shopItem;
            _chasTable.Remove(shopItem);

            slot.SetItem(shopItem);
            _shopTableText[c].text = shopItem.GetAmount() + " G";
        }
    }
}