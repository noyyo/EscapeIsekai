using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingManager : CustomSingleton<ItemCraftingManager>
{
    [SerializeField] private GameObject _itemCraftingUI;
    [SerializeField] private ItemCraftingController _craftingController;
    [SerializeField] private GameObject _craftingSlotPrefab;
    [SerializeField] private GameObject _itemCaftingMaterials_UI;
    [SerializeField] private GameObject _itemExplanation_UI;
    private MaterialsSlot[] _materialsSlots;

    private UI_Manager _uiManager;
    private ItemRecipe _clickSlot;
    private bool _isMake;
    private Button _craftingButton;
    private InventoryManager _inventoryManager;
    private bool _isDisplay;

    //슬롯 클릭했을때의 이벤트
    public event Action onClickCraftingSlotEvent;
    //슬롯 아웃라인 off이벤트
    public event Action offOutLineEvent;
    //아이템 설명창 및 재료창 Off이벤트
    public event Action OffItemCaftingMaterialsEvent;
    //UI업데이트 이벤트
    public event Action<ItemRecipe> onUpdateUIEvent;
    //제작버튼 눌렀을시 이벤트
    public event Action onClickCraftingButtonEvent;
    //제작창 종료 이벤트
    public event Action offCraftingUIEvent;
    //제작창 오픈 이벤트
    public event Action onCraftingUIEvent;

    public GameObject ItemCraftingUI { get {  return _itemCraftingUI; } }
    public GameObject CraftingSlotPrefab { get { return _craftingSlotPrefab; } }
    public GameObject ItemCaftingMaterials_UI { get { return _itemCaftingMaterials_UI; } }
    public GameObject ItemExplanation_UI { get { return _itemExplanation_UI; } }
    public bool IsMake { get { return _isMake; } }
    public ItemRecipe ClickSlot { get { return _clickSlot; } }
    public MaterialsSlot[] MaterialsSlots { get { return _materialsSlots; } }
    public bool IsDisplay { get { return _isDisplay; } }
    public ItemCraftingController CraftingController 
    { 
        get { return _craftingController; }
        set
        {
            if(_craftingController == null)
                _craftingController = value;
        }
    }

    private void Awake()
    {
        _uiManager = UI_Manager.Instance;
        _inventoryManager = InventoryManager.Instance;
        _itemCraftingUI = GameObject.FindGameObjectWithTag("CraftingUI");
        if (_itemCraftingUI == null)
        {
            _itemCraftingUI = Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/ItemCraftingUI");
            _itemCraftingUI = Instantiate(_itemCraftingUI, _uiManager.Canvas.transform);
        }
        if (_craftingSlotPrefab == null)
            _craftingSlotPrefab = Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/CreftingSlot");

        if (_itemCaftingMaterials_UI == null)
            _itemCaftingMaterials_UI = _itemCraftingUI.transform.GetChild(2).gameObject;

        if(_itemExplanation_UI == null)
            _itemExplanation_UI = _itemCraftingUI.transform.GetChild(3).gameObject;

        _craftingButton = _itemCaftingMaterials_UI.transform.GetChild(2).GetComponent<Button>();

        if(_materialsSlots == null)
        {
            _materialsSlots = ItemCaftingMaterials_UI.transform.GetChild(3).GetComponentsInChildren<MaterialsSlot>();
        }
    }
    private void Start()
    {
        _craftingButton.onClick.AddListener(CallOnClickCraftingButtonEvent);
        onClickCraftingButtonEvent += CraftingItem;
        onClickCraftingButtonEvent += CallOnUpdateUIEvent;

        onClickCraftingSlotEvent += SetActiveItemUI;
        onClickCraftingSlotEvent += CallOnUpdateUIEvent;
        onClickCraftingSlotEvent += CallOffOutLineEvent;

        OffItemCaftingMaterialsEvent += UnDisplayItemMaterialsUI;

        offCraftingUIEvent += CallOffOutLineEvent;
        offCraftingUIEvent += CallOffItemCaftingMaterials;
        offCraftingUIEvent += UnDisplayCraftingUI;

        onCraftingUIEvent += DisplayCraftingUI;
    }

    private void Update()
    {
        //테스트용 나중에 다른 코드랑 연결시 수정해야됨
        if (Input.GetKeyDown(KeyCode.H))
        {
            CallOnCraftingUI();
        }
    }

    public void CallOnClickCraftingSlotEvent(ItemRecipe newRecipe, bool isMake)
    {
        _clickSlot = newRecipe;
        _isMake = isMake;
        onClickCraftingSlotEvent?.Invoke();
    }

    public void CallOffOutLineEvent()
    {
        offOutLineEvent?.Invoke();
        offOutLineEvent = null;
    }

    public void CallOffItemCaftingMaterials()
    {
        OffItemCaftingMaterialsEvent?.Invoke();
    }

    public void CallOnUpdateUIEvent()
    {
        onUpdateUIEvent?.Invoke(ClickSlot);
    }

    public void CraftingItem()
    {
        if (IsMake)
        {
            //나중에 금화시스템 추가되면 비교문 추가할것
            Debug.Log(ClickSlot == null);
            _inventoryManager.CallAddItems(ClickSlot, out int[] sum);
            foreach(var item in sum)
            {
                Debug.Log(item);
            }
        }
    }

    public void CallOnClickCraftingButtonEvent()
    {
        onClickCraftingButtonEvent?.Invoke();
    }

    private void SetActiveItemUI()
    {
        _itemCaftingMaterials_UI.SetActive(true);
        _itemExplanation_UI.SetActive(true);
    }

    private void UnDisplayItemMaterialsUI()
    {
        _itemCaftingMaterials_UI.SetActive(false);
        _itemExplanation_UI.SetActive(false);
    }

    public void CallOffCraftingUIEvent()
    {
        offCraftingUIEvent?.Invoke();
        _isDisplay = false;
    }

    public void UnDisplayCraftingUI()
    {
        _itemCraftingUI.SetActive(false);
    }

    public void CallOnCraftingUI()
    {
        if (!(_inventoryManager.IsDisplay))
        {
            onCraftingUIEvent?.Invoke();
            _isDisplay = true;
        }
            
    }

    private void DisplayCraftingUI()
    {
        _itemCraftingUI.SetActive(true);
    }

    public void CallAddRecipe(int id)
    {
        _craftingController.AddRecipe(id);
    }
}
