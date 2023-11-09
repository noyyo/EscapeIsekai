using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemAvailability;
    [SerializeField] private GameObject _outLine;
    [SerializeField] private Image _icon;

    //���� ������ ����
    private ItemData_Test _itemData;
    private Inventory _inventory;
    private int[] _materialsCount;
    private bool[] _boolArray;
    private ItemCraftingManager _craftingManager;

    private Button _button;
    private bool isDisplay = false;
    private bool _isMake = true;
    private ItemDB _itemDB;

    private ItemRecipe itemRecipe;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);
        _inventory = InventoryManager.Instance.Inventory;
        _craftingManager = ItemCraftingManager.Instance;
        _itemDB = ItemDB.Instance;
    }

    private void Start()
    {
        _craftingManager.onUpdateUIEvent += MakeCheck;
    }

    //������ư ������ ������ Ȱ��ȭ�ϱ� ���� �޼���
    public void TurnOnOffSlot()
    {
        isDisplay = !isDisplay;
        this.gameObject.SetActive(isDisplay);
    }

    //������ ������ �����ϱ� ���� �޼���
    public void SetSlot(in ItemRecipe newRecipe)
    {
        if(_itemDB == null)
            _itemDB = ItemDB.Instance;
        _itemDB.GetItemData(newRecipe.CraftingID, out ItemData_Test newItemData);
        _itemData = newItemData;
        _icon.sprite = newItemData.Icon;
        _itemName.text = newItemData.ItemName;
        itemRecipe = newRecipe;
        MakeCheck(newRecipe);
        if (_isMake)
        {
            _itemAvailability.text = "������ �����մϴ�.";
            _itemAvailability.color = Color.white;
        }
        else
        {
            _itemAvailability.text = "������ �Ұ����մϴ�.";
            _itemAvailability.color = Color.red;
        }
    }

    //��ư�� ������ ����
    public void OnButtonClick()
    {
        MakeCheck(itemRecipe);
        _craftingManager.CallOnClickCraftingSlotEvent(itemRecipe, _isMake);
        _outLine.SetActive(true);
        _craftingManager.offOutLineEvent += OutLineTrunOff;
    }

    //�ڵ������� ���� �޼���
    public void OutLineTrunOff()
    {
        _outLine.SetActive(false);
    }

    public void MakeCheck(ItemRecipe newRecipe)
    {
        if (_inventory == null)
            _inventory = InventoryManager.Instance.Inventory;
        _boolArray = _inventory.IsCheckItems(newRecipe.Materials, newRecipe.MaterialsCount, out _materialsCount);

        _isMake = true;
        foreach (bool i in _boolArray)
        {
            if(i == false)
                _isMake = false;
        }
    }
}
