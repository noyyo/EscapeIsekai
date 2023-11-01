using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [Header("UI관련 오브젝트")]
    [SerializeField][Tooltip("인벤토리 UI")] private GameObject _inventory_GameObject;
    [SerializeField][Tooltip("인벤토리안에 있는 InventoryType을 넣어주세요")] private GameObject _inventoryTypeGroup;
    [SerializeField][Tooltip("인벤토리안에 있는 Tail을 넣어주세요")] private GameObject _inventoryTailGroup;
    [SerializeField][Tooltip("인벤토리안에 있는 ItemExplanation을 넣어주세요")] private GameObject _itemExplanationPopup;

    [Header("Inventory")]
    [SerializeField] private Inventory _inventory;

    private InventoryManager _inventoryManager;
    private UI_Manager _ui_manager;

    //인벤토리 ON OFF 용 bool
    private bool _isInventoryUI;

    //UI관련
    private Button[] _inventoryTypeButton;
    private Button[] _inventoryTailButton;
    private GameObject _tailUseButton;
    private TMP_Text _tailUseButtonText;

    //현재 출력되고 있는 카테고리
    private ItemType _nowDisplayItemType;

    public GameObject ItemExplanationPopup { get { return _itemExplanationPopup; } }

    private void Awake()
    {
        Preferences();
        Init();
    }

    //고정된 변수 초기화
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

        //버튼 설정
        // 장비 : 0, 소비 : 1, 재료 : 2, 기타 : 3
        _inventoryTypeButton = _inventoryTypeGroup.transform.GetComponentsInChildren<Button>();

        //정렬 : 0, 뒤로가기 : 1, 버리기 : 2, 사용 : 3
        _inventoryTailButton = _inventoryTailGroup.transform.GetComponentsInChildren<Button>();

        _tailUseButton = _inventoryTailButton[3].gameObject;
        _tailUseButtonText = _tailUseButton.GetComponentInChildren<TMP_Text>();
    }

    //변할수있는 값 초기화
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

    //이벤트에 걸린 메서드( 인벤토리 창만 띄움)
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
                _tailUseButtonText.text = "장착";
                break;
            case ItemType.Consumable:
                _tailUseButton.SetActive(true);
                _tailUseButtonText.text = "소비";
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
