using System;
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
    private GameObject _player;
    public event Action onClickCraftingSlotEvent;
    public event Action offOutLineEvent;
    public event Action OffItemCaftingMaterialsEvent;
    public event Action<ItemRecipe> onUpdateUIEvent;
    public event Action onClickCraftingButtonEvent;
    public event Action offCraftingUIEvent;
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

        if(_craftingController == null)
        {
            _player = _uiManager.Player;
            if (_player != null)
                _craftingController = _player.GetComponent<ItemCraftingController>();
            if(_craftingController == null)
            {
                _player = GameObject.FindGameObjectWithTag("Player");
                _craftingController = _player.GetComponent<ItemCraftingController>();
            }
        }
    }
    private void Start()
    {
        _craftingButton.onClick.AddListener(CallOnClickCraftingButtonEvent);
        onClickCraftingButtonEvent += CraftingItem;
        //onClickCraftingButtonEvent += CallOnUpdateUIEvent;

        onClickCraftingSlotEvent += SetActiveItemUI;
        onClickCraftingSlotEvent += CallOnUpdateUIEvent;
        onClickCraftingSlotEvent += CallOffOutLineEvent;

        OffItemCaftingMaterialsEvent += UnDisplayItemMaterialsUI;

        offCraftingUIEvent += CallOffOutLineEvent;
        offCraftingUIEvent += CallOffItemCaftingMaterials;
        offCraftingUIEvent += UnDisplayCraftingUI;

        onCraftingUIEvent += DisplayCraftingUI;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.H))
    //    {
    //        if(_isDisplay)
    //            CallOffCraftingUIEvent();
    //        else
    //            CallOnCraftingUI();
    //    }
    //}

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
        if (_isMake)
            _inventoryManager.CallAddItems(ClickSlot, out int[] sum);
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
        Cursor.lockState = CursorLockMode.Locked;
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
            Cursor.lockState = CursorLockMode.Confined;
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
