using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [Header("UI���� ������Ʈ")]
    [SerializeField][Tooltip("�κ��丮 UI")] private GameObject _inventory_GameObject;
    [SerializeField][Tooltip("�κ��丮�ȿ� �ִ� InventoryType�� �־��ּ���")] private GameObject _inventoryTypeGroup;
    [SerializeField][Tooltip("�κ��丮�ȿ� �ִ� Tail�� �־��ּ���")] private GameObject _inventoryTailGroup;
    [SerializeField][Tooltip("�κ��丮�ȿ� �ִ� ItemExplanation�� �־��ּ���")] private GameObject _itemExplanationPopup;

    [Header("Inventory")]
    [SerializeField] private Inventory _inventory;

    private InventoryManager _inventoryManager;
    private UI_Manager _ui_manager;

    //�κ��丮 ON OFF �� bool
    private bool _isInventoryUI;

    //UI����
    private Button[] _inventoryTypeButton;
    private Button[] _inventoryTailButton;
    private GameObject _tailUseButton;
    private TMP_Text _tailUseButtonText;

    //���� ��µǰ� �ִ� ī�װ�
    private ItemType _nowDisplayItemType;

    public GameObject ItemExplanationPopup { get { return _itemExplanationPopup; } }

    private void Awake()
    {
        Preferences();
        Init();
    }

    //������ ���� �ʱ�ȭ
    private void Preferences()
    {
        _inventoryManager = InventoryManager.Instance;
        _ui_manager = UI_Manager.Instance;

        if (_inventory_GameObject == null)
            _inventory_GameObject = this.gameObject;

        if (_inventoryTypeGroup == null)
            _inventoryTypeGroup = transform.GetChild(2).gameObject;

        if (_inventoryTailGroup == null)
            _inventoryTailGroup = transform.GetChild(1).gameObject;

        if (_itemExplanationPopup == null)
            _itemExplanationPopup = transform.GetChild(3).gameObject;

        if (_inventory == null)
            _inventory = Resources.Load<GameObject>("Prefabs/Test/Player").GetComponent<Inventory>();

        //��ư ����
        // ��� : 0, �Һ� : 1, ��� : 2, ��Ÿ : 3
        _inventoryTypeButton = _inventoryTypeGroup.transform.GetComponentsInChildren<Button>();

        //���� : 0, �ڷΰ��� : 1, ������ : 2, ��� : 3
        _inventoryTailButton = _inventoryTailGroup.transform.GetComponentsInChildren<Button>();

        _tailUseButton = _inventoryTailButton[3].gameObject;
        _tailUseButtonText = _tailUseButton.GetComponentInChildren<TMP_Text>();
    }

    //���Ҽ��ִ� �� �ʱ�ȭ
    private void Init()
    {
        _nowDisplayItemType = ItemType.Equipment;
        _isInventoryUI = false;

    }


    private void Start()
    {
        _inventory.OnInventoryDisplayEvent += SetActiveInventroyUI;
        _inventoryTypeButton[0].onClick.AddListener(() => 
        { 
            if (_nowDisplayItemType != ItemType.Equipment) 
            { 
                CallItemSlots(ItemType.Equipment); 
                TurnOffItemExplanationPopup();
                TurnOffInventoryTailUI();
                _inventoryManager.CallTurnOffItemClick();
            } 
        });
        _inventoryTypeButton[1].onClick.AddListener(() => 
        { 
            if (_nowDisplayItemType != ItemType.Consumable) 
            { 
                CallItemSlots(ItemType.Consumable); 
                TurnOffItemExplanationPopup(); 
                TurnOffInventoryTailUI();
                _inventoryManager.CallTurnOffItemClick(); 
            } 
        });
        _inventoryTypeButton[2].onClick.AddListener(() => { 
            if (_nowDisplayItemType != ItemType.Material) 
            { 
                CallItemSlots(ItemType.Material); 
                TurnOffItemExplanationPopup(); 
                TurnOffInventoryTailUI();
                _inventoryManager.CallTurnOffItemClick(); 
            } 
        });
        _inventoryTypeButton[3].onClick.AddListener(() => 
        { 
            if (_nowDisplayItemType != ItemType.ETC) 
            { 
                CallItemSlots(ItemType.ETC); 
                TurnOffItemExplanationPopup(); 
                TurnOffInventoryTailUI();
                _inventoryManager.CallTurnOffItemClick(); 
            } 
        });

        _inventoryTailButton[0].onClick.AddListener(_inventory.SortInventory);
        _inventoryTailButton[1].onClick.AddListener(SetActiveInventroyUI);
        _inventoryTailButton[2].onClick.AddListener(_inventory.Drop);
        _inventoryTailButton[3].onClick.AddListener(_inventory.UseItem);
    }

    //�̺�Ʈ�� �ɸ� �޼���( �κ��丮 â�� ���)
    public void SetActiveInventroyUI()
    {
        _isInventoryUI = !_isInventoryUI;
        _inventory_GameObject.SetActive(_isInventoryUI);
    }

    private void CallItemSlots(ItemType displayType)
    {
        _nowDisplayItemType = displayType;
        _inventory.SetDisplayType(displayType);

    }

    public void TurnOffItemExplanationPopup()
    {
        _itemExplanationPopup.SetActive(false);
    }

    public void DisplayInventoryTailUI()
    {
        _inventoryTailGroup.SetActive(true);
        switch (_nowDisplayItemType)
        {
            case ItemType.Equipment:
                _tailUseButton.SetActive(true);
                _tailUseButtonText.text = "����";
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

    public void TurnOffInventoryTailUI()
    {
        _inventoryTailGroup.SetActive(false);
    }
}
