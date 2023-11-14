using System;
using UnityEngine;
using UnityEngine.UI;
public class ItemCraftingManager : CustomSingleton<ItemCraftingManager>
{
    private GameObject _itemCraftingUI;
    private ItemCraftingController _craftingController;
    private GameObject _craftingSlotPrefab;
    private GameObject _itemCaftingMaterials_UI;
    private GameObject _itemExplanation_UI;
    private MaterialsSlot[] _materialsSlots;

    private UI_Manager _ui_Manager;
    private ItemRecipe _clickSlot;
    private bool _isMake;
    private Button _craftingButton;
    private InventoryManager _inventoryManager;
    private GameObject _player;
    private GameManager _gameManager;
    public event Action onClickCraftingSlotEvent;
    public event Action offOutLineEvent;
    public event Action<ItemRecipe> onUpdateUIEvent;
    public event Action onCraftingEvent;
    public GameObject ItemCraftingUI { get {  return _itemCraftingUI; } }
    public GameObject CraftingSlotPrefab { get { return _craftingSlotPrefab; } }
    public GameObject ItemCaftingMaterials_UI { get { return _itemCaftingMaterials_UI; } }
    public GameObject ItemExplanation_UI { get { return _itemExplanation_UI; } }
    public bool IsMake { get { return _isMake; } }
    public ItemRecipe ClickSlot { get { return _clickSlot; } }
    public MaterialsSlot[] MaterialsSlots { get { return _materialsSlots; } }

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _ui_Manager = UI_Manager.Instance;
        _inventoryManager = InventoryManager.Instance;
        _craftingSlotPrefab = Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/CreftingSlot");
    }

    private void Start()
    {
        Init();

        _ui_Manager.UI_ItemCraftingTurnOnEvent += ItemCraftingUITurnOn;

        _craftingButton.onClick.AddListener(CallOnCrafting);
        onCraftingEvent += CraftingItem;

        onClickCraftingSlotEvent += ItemMaterialsUITurnOn;
        onClickCraftingSlotEvent += CallUpdateUI;
        onClickCraftingSlotEvent += CallOffOutLineEvent;

        _ui_Manager.UI_ItemCraftingTurnOffEvent += CallOffOutLineEvent;
        _ui_Manager.UI_ItemCraftingTurnOffEvent += ItemMaterialsUITurnOff;
        _ui_Manager.UI_ItemCraftingTurnOffEvent += ItemCraftingUITurnOff;
    }

    private void Init()
    {
        _itemCraftingUI = _ui_Manager.ItemCrafting_UI;
        _itemCaftingMaterials_UI = _itemCraftingUI.transform.GetChild(2).gameObject;
        _itemExplanation_UI = _itemCraftingUI.transform.GetChild(3).gameObject;
        _craftingButton = _itemCaftingMaterials_UI.transform.GetChild(2).GetComponent<Button>();
        _materialsSlots = _itemCaftingMaterials_UI.transform.GetChild(3).GetComponentsInChildren<MaterialsSlot>();
        _craftingController = _gameManager.Player.GetComponent<ItemCraftingController>();
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

    public void CallUpdateUI()
    {
        onUpdateUIEvent?.Invoke(ClickSlot);
    }

    public void CallOnCrafting()
    {
        onCraftingEvent?.Invoke();
    }

    private void ItemMaterialsUITurnOn()
    {
        _itemCaftingMaterials_UI.SetActive(true);
        _itemExplanation_UI.SetActive(true);
    }

    private void ItemMaterialsUITurnOff()
    {
        _itemCaftingMaterials_UI.SetActive(false);
        _itemExplanation_UI.SetActive(false);
    }

    private void ItemCraftingUITurnOff()
    {
        _itemCraftingUI.SetActive(false);
    }

    private void ItemCraftingUITurnOn()
    {
        _itemCraftingUI.SetActive(true);
    }

    public void CallAddRecipe(int id)
    {
        _craftingController.AddRecipe(id);
    }

    private void CraftingItem()
    {
        if (_isMake)
            _inventoryManager.CallAddItems(ClickSlot, out int[] sum);
    }
}
