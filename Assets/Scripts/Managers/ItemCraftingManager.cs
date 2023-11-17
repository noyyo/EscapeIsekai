using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemCraftingManager : CustomSingleton<ItemCraftingManager>
{
    protected ItemCraftingManager() { }
    private GameManager _gameManager;
    private UI_Manager _ui_Manager;
    private InventoryManager _inventoryManager;
    private ItemCraftingController _craftingController;
    private GameObject _itemCraftingUI;
    private GameObject _craftingSlotPrefab;
    private GameObject _itemCaftingMaterials_UI;
    private GameObject _itemExplanation_UI;
    private MaterialsSlot[] _materialsSlots;
    private ItemRecipe _clickSlot;
    private bool _isMake;

    public GameObject CraftingSlotPrefab { get { return _craftingSlotPrefab; } }
    public ItemRecipe ClickSlot { get { return _clickSlot; } }
    public MaterialsSlot[] MaterialsSlots { get { return _materialsSlots; } }
    public bool IsMake { get { return _isMake; } }

    public event Action onClickCraftingSlotEvent;
    public event Action offOutLineEvent;
    public event Action<ItemRecipe> onUpdateUIEvent;
    public event Action onCraftingEvent;

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
