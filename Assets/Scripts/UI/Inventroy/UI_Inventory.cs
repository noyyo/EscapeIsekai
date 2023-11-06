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
        Init();
    }

    //변할수있는 값 초기화
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
            _inventoryTailGroup = _inventory_GameObject.transform.GetChild(1).GetChild(0).gameObject;

        if (_itemExplanationPopup == null)
            _itemExplanationPopup = _inventory_GameObject.transform.GetChild(3).gameObject;

        if (_inventory == null)
            _inventory = GetComponent<Inventory>();

        //버튼 설정
        // 장비 : 0, 소비 : 1, 재료 : 2, 기타 : 3
        _inventoryTypeButton = _inventoryTypeGroup.transform.GetComponentsInChildren<Button>();

        //정렬 : 0, 뒤로가기 : 1, 버리기 : 2, 사용 : 3
        _inventoryTailButton = _inventoryTailGroup.transform.GetComponentsInChildren<Button>();

        _tailUseButton = _inventoryTailButton[3].gameObject;
        _tailUseButtonText = _tailUseButton.GetComponentInChildren<TMP_Text>();
        _tailUseButtonText = _tailUseButton.GetComponentInChildren<TMP_Text>();

        _inventory.OnInventoryDisplayEvent += SetActiveInventroyUI;
        _inventoryTypeButton[0].onClick.AddListener(() =>{ OnCategoryButton(ItemType.Equipment); });
        _inventoryTypeButton[1].onClick.AddListener(() =>{ OnCategoryButton(ItemType.Consumable); });
        _inventoryTypeButton[2].onClick.AddListener(() =>{ OnCategoryButton(ItemType.Material); });
        _inventoryTypeButton[3].onClick.AddListener(() =>{ OnCategoryButton(ItemType.ETC); });

        _inventoryTailButton[0].onClick.AddListener(_inventory.SortInventory);
        _inventoryTailButton[1].onClick.AddListener(SetActiveInventroyUI); //돌아가기
        _inventoryTailButton[2].onClick.AddListener(_inventory.Drop);
        _inventoryTailButton[3].onClick.AddListener(_inventory.UseItem);
    }

    //이벤트에 걸린 메서드( 인벤토리 창 ON, OFF)
    public void SetActiveInventroyUI()
    {
        _isInventoryUI = !_isInventoryUI;
        _inventory_GameObject.SetActive(_isInventoryUI);
        if (_isInventoryUI)
            _ui_manager.CallTurnOffQuickSlot();
        else
            _ui_manager.CallTurnOnQuickSlot();
    }

    // 카테고리 변경됨으로써 현재 카테고리 저장후 UI표시
    private void CallItemSlots(ItemType displayType)
    {
        _nowDisplayItemType = displayType;
        _inventory.SetDisplayType(displayType);
    }

    // 설명창 끄기
    public void TurnOffItemExplanationPopup()
    {
        _itemExplanationPopup.SetActive(false);
    }

    //현재 카테고리에 따른 밑쪽 버튼텍스트 번경 및 ON, OFF
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
    
    // 밑쪽 버튼 모두 OFF
    public void TurnOffInventoryTailUI()
    {
        _inventoryTailGroup.SetActive(false);
    }

    //카테고리 버튼 눌렀을시 해야해는 동작 모음 (이벤트로 바꿔도 상관없을듯)
    public void OnCategoryButton(ItemType categoryType)
    {
        if (_nowDisplayItemType == categoryType) return;
        InventoryUITurnOff();
        CallItemSlots(categoryType); //Slot에 표시되는 Item 바꾸기
    }

    public void InventoryUITurnOff()
    {
        TurnOffItemExplanationPopup(); // 설명창 오프
        TurnOffInventoryTailUI(); // 인벤토리 UI 오프
        _inventoryManager.CallTurnOffItemClick(); //클릭시 뜨는 UI 오프
    }
}
