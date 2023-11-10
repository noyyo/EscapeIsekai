using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private GameObject _inventory_GameObject;
    private GameObject _inventoryTypeGroup;
    private GameObject _inventoryTailGroup;
    private GameObject _itemExplanationPopup;
    private Inventory _inventory;

    private InventoryManager _inventoryManager;
    private UI_Manager _ui_manager;

    //�κ��丮 ON OFF �� bool
    private bool _isInventoryUI;

    //UI����
    private Button[] _inventoryTypeButton;
    private Button[] _inventoryTailButton;
    private GameObject _tailUseButton;
    private TMP_Text _tailUseButtonText;
    private Button _backButton;

    //���� ��µǰ� �ִ� ī�װ�
    private ItemType _nowDisplayItemType;

    public GameObject ItemExplanationPopup { get { return _itemExplanationPopup; } }

    private void Awake()
    {
        Init();
    }

    //���Ҽ��ִ� �� �ʱ�ȭ
    private void Init()
    {
        _inventoryManager = InventoryManager.Instance;
        _ui_manager = UI_Manager.Instance;
        _nowDisplayItemType = ItemType.Equipment;
        _isInventoryUI = false;
    }


    private void Start()
    {
        if (_inventory_GameObject == null)
            _inventory_GameObject = _inventoryManager.Inventory_UI;

        if (_inventoryTypeGroup == null)
            _inventoryTypeGroup = _inventory_GameObject.transform.GetChild(2).gameObject;

        if (_inventoryTailGroup == null)
            _inventoryTailGroup = _inventory_GameObject.transform.GetChild(1).GetChild(1).gameObject;

        if (_itemExplanationPopup == null)
            _itemExplanationPopup = _inventory_GameObject.transform.GetChild(3).gameObject;

        if (_inventory == null)
            _inventory = GetComponent<Inventory>();

        //��ư ����
        // ��� : 0, �Һ� : 1, ��� : 2, ��Ÿ : 3
        _inventoryTypeButton = _inventoryTypeGroup.transform.GetComponentsInChildren<Button>();

        //���� : 0, ������ : 1, ��� : 2
        _inventoryTailButton = _inventoryTailGroup.transform.GetComponentsInChildren<Button>();

        //�ڷΰ���
        _backButton = _inventory_GameObject.transform.GetChild(1).GetChild(0).GetComponent<Button>();

        _tailUseButton = _inventoryTailButton[2].gameObject;
        _tailUseButtonText = _tailUseButton.GetComponentInChildren<TMP_Text>();
        _tailUseButtonText = _tailUseButton.GetComponentInChildren<TMP_Text>();

        _inventoryManager.OnInventoryDisplayEvent += SetActiveInventroyUI;
        _inventoryTypeButton[0].onClick.AddListener(() =>{ OnCategoryButton(ItemType.Equipment); });
        _inventoryTypeButton[1].onClick.AddListener(() =>{ OnCategoryButton(ItemType.Consumable); });
        _inventoryTypeButton[2].onClick.AddListener(() =>{ OnCategoryButton(ItemType.Material); });
        _inventoryTypeButton[3].onClick.AddListener(() =>{ OnCategoryButton(ItemType.ETC); });

        _inventoryTailButton[0].onClick.AddListener(_inventory.SortInventory);
        _inventoryTailButton[1].onClick.AddListener(_inventory.Drop);
        _inventoryTailButton[2].onClick.AddListener(_inventory.UseItem);

        _backButton.onClick.AddListener(_inventoryManager.CallOnInventoryDisplayEvent); //���ư���

        _inventoryManager.onTextChangeEquipEvent += ButtonTextChange_Equip;
        _inventoryManager.onTextChangeUnEquipEvent += ButtonTextChange_Unequip;
    }

    //�̺�Ʈ�� �ɸ� �޼���( �κ��丮 â ON, OFF)
    public void SetActiveInventroyUI()
    {
        _isInventoryUI = !_isInventoryUI;
        _inventory_GameObject.SetActive(_isInventoryUI);
        if (_isInventoryUI)
            _ui_manager.CallTurnOffQuickSlot();
        else
            _ui_manager.CallTurnOnQuickSlot();

    }

    // ī�װ� ��������ν� ���� ī�װ� ������ UIǥ��
    private void CallItemSlots(ItemType displayType)
    {
        _nowDisplayItemType = displayType;
        _inventory.SetDisplayType(displayType);
    }

    // ����â ����
    public void TurnOffItemExplanationPopup()
    {
        _itemExplanationPopup.SetActive(false);
    }

    //���� ī�װ��� ���� ���� ��ư�ؽ�Ʈ ���� �� ON, OFF
    public void DisplayInventoryTailUI()
    {
        _inventoryTailGroup.SetActive(true);
        switch (_nowDisplayItemType)
        {
            case ItemType.Equipment:
                _tailUseButton.SetActive(true);
                if (_inventoryManager.ClickItem.equip)
                    ButtonTextChange_Unequip();
                else
                    ButtonTextChange_Equip();
                break;
            case ItemType.Consumable:
                _tailUseButton.SetActive(true);
                _tailUseButtonText.text = "�Һ�";
                break;
            case ItemType.Material:
                _tailUseButton.SetActive(false);
                break;
            default:
                _tailUseButton.SetActive(false);
                break;
        }
    }
    
    // ���� ��ư ��� OFF
    public void TurnOffInventoryTailUI()
    {
        _inventoryTailGroup.SetActive(false);
    }

    //ī�װ� ��ư �������� �ؾ��ش� ���� ���� (�̺�Ʈ�� �ٲ㵵 ���������)
    public void OnCategoryButton(ItemType categoryType)
    {
        if (_nowDisplayItemType == categoryType) return;
        InventoryUITurnOff();
        CallItemSlots(categoryType); //Slot�� ǥ�õǴ� Item �ٲٱ�
    }

    public void InventoryUITurnOff()
    {
        TurnOffItemExplanationPopup(); // ����â ����
        TurnOffInventoryTailUI(); // �κ��丮 UI ����
        _inventoryManager.CallTurnOffItemClick(); //Ŭ���� �ߴ� UI ����
    }
    
    public void ButtonTextChange_Equip()
    {
        _tailUseButtonText.text = "����";
    }

    public void ButtonTextChange_Unequip()
    {
        _tailUseButtonText.text = "��� ����";
    }
}
