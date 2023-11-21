using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Manager : CustomSingleton<UI_Manager>
{
    protected UI_Manager() { }
    [SerializeField] private GameObject _cavas;
    public GameObject gathering;
    public GameObject talkManager;
    public GameObject questManager;
    public GameObject dialog;

    private GameManager _gameManager;
    private InventoryManager _inventoryManager;
    private ItemCraftingManager _itemCraftingManager;
    private GameObject _inventory_ui;
    private GameObject _itemCrafting_ui;
    private GameObject _quickSlot_ui;

    private bool _isNotUIInputPossible = false;
    private bool _isTurnOnInventory;
    public GameObject Canvas { get { return _cavas; } }
    public GameObject Inventory_UI { get { return _inventory_ui; } }
    public GameObject ItemCrafting_UI { get { return _itemCrafting_ui; } }
    public GameObject QuickSlot_UI { get { return _quickSlot_ui; } }
    public bool IsTurnOnInventory { get { return _isTurnOnInventory; } }

    public event Action UI_AllTurnOffEvent;
    public event Action UI_InventoryTurnOnEvent;
    public event Action UI_InventoryTurnOffEvent;
    public event Action UI_ItemCraftingTurnOnEvent;
    public event Action UI_ItemCraftingTurnOffEvent;
    public event Action UI_QuickSlotTurnOnEvent;
    public event Action UI_QuickSlotTurnOffEvent;

    public string itemName;
    public string itemExplanation;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _inventoryManager = InventoryManager.Instance;
        _itemCraftingManager = ItemCraftingManager.Instance;
        Init();
    }

    private void Start()
    {
        UI_InventoryTurnOnEvent += CallUI_QuickSlotTurnOff;
        UI_InventoryTurnOffEvent += CallUI_QuickSlotTurnOn;
        UI_ItemCraftingTurnOnEvent += CallUI_QuickSlotTurnOff;
        UI_ItemCraftingTurnOffEvent += CallUI_QuickSlotTurnOn;

        UI_AllTurnOffEvent += CallUI_InventoryTurnOff;
        UI_AllTurnOffEvent += CallUI_ItemCraftingTurnOff;
    }

    public void Init()
    {
        _cavas = GameObject.FindGameObjectWithTag("Canvas");
        if (_cavas == null)
            _cavas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas"));

        if (gathering == null)
        {
            gathering = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Gathering/GatheringUI"),_cavas.transform);
            gathering.SetActive(false);
        }

        if (_inventory_ui == null)
        {
            _inventory_ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Inventory/Inventory"), _cavas.transform);
            _inventory_ui.SetActive(false);
        }

        if (_itemCrafting_ui == null)
            _itemCrafting_ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/ItemCraftingUI"), _cavas.transform);
        if (_quickSlot_ui == null)
            _quickSlot_ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpecialAbilities/QuickSlot_UI"), _cavas.transform);
        if(questManager == null)
            questManager = Instantiate(Resources.Load<GameObject>("Prefabs/Manager/QuestManager"));
        if (talkManager == null)
            talkManager = Instantiate(Resources.Load<GameObject>("Prefabs/Manager/TalkManager"));
        if (dialog == null)
        {
            dialog = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/UI_Dialog"));
            dialog.GetComponent<Dialog>().questManager = questManager.GetComponent<QuestManager>();
        }
    }

    //UI ON, OFF를 위한 메서드
    public void CallUI_AllTurnOff()
    {
        UI_AllTurnOffEvent?.Invoke();
    }

    public void CallUI_InventoryTurnOn()
    {
        if (_isNotUIInputPossible) return;
        UI_InventoryTurnOnEvent?.Invoke();
        _isTurnOnInventory = !_isTurnOnInventory;
    }
    public void CallUI_InventoryTurnOff()
    {
        if (_isNotUIInputPossible) return;
        UI_InventoryTurnOffEvent?.Invoke();
        _isTurnOnInventory = !_isTurnOnInventory;
    }
    public void CallUI_ItemCraftingTurnOn()
    {
        UI_ItemCraftingTurnOnEvent?.Invoke();
    }
    public void CallUI_ItemCraftingTurnOff()
    {
        UI_ItemCraftingTurnOffEvent?.Invoke();
    }

    public void CallUI_QuickSlotTurnOn()
    {
        UI_QuickSlotTurnOnEvent?.Invoke();
    }

    public void CallUI_QuickSlotTurnOff()
    {
        UI_QuickSlotTurnOffEvent?.Invoke();
    }
}
