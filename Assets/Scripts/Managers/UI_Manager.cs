using System;
using UnityEngine;

public class UI_Manager : CustomSingleton<UI_Manager>
{
    protected UI_Manager() { }
    [SerializeField] private GameObject cavas;
    public GameObject gathering;
    public GameObject talkManager;
    public GameObject questManager;
    public GameObject dialog;

    private GameManager gameManager;
    private InventoryManager inventoryManager;
    private ItemCraftingManager itemCraftingManager;
    private GameObject inventoryUI;
    private GameObject itemCraftingUI;
    //private GameObject _quickSlot_ui;

    private bool isNotUIInputPossible = false;
    private bool isTurnOnInventory;
    public GameObject Canvas { get { return cavas; } }
    public GameObject Inventory_UI { get { return inventoryUI; } }
    public GameObject ItemCrafting_UI { get { return itemCraftingUI; } }
    //public GameObject QuickSlot_UI { get { return _quickSlot_ui; } }
    public bool IsTurnOnInventory { get { return isTurnOnInventory; } }

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
        gameManager = GameManager.Instance;
        inventoryManager = InventoryManager.Instance;
        itemCraftingManager = ItemCraftingManager.Instance;
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
        cavas = GameObject.FindGameObjectWithTag("Canvas");
        if (cavas == null)
            cavas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas"));

        if (gathering == null)
        {
            gathering = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Gathering/GatheringUI"),cavas.transform);
            gathering.SetActive(false);
        }

        if (inventoryUI == null)
        {
            inventoryUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Inventory/Inventory"), cavas.transform);
            inventoryUI.SetActive(false);
        }

        if (itemCraftingUI == null)
            itemCraftingUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/ItemCraftingUI"), cavas.transform);
       // if (_quickSlot_ui == null)
            //_quickSlot_ui = Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpecialAbilities/QuickSlot_UI"), _cavas.transform);
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

    //UI ON, OFF¸¦ À§ÇÑ ¸Þ¼­µå
    public void CallUI_AllTurnOff()
    {
        UI_AllTurnOffEvent?.Invoke();
    }

    public void CallUI_InventoryTurnOn()
    {
        if (isNotUIInputPossible) return;
        UI_InventoryTurnOnEvent?.Invoke();
        isTurnOnInventory = !isTurnOnInventory;
    }
    public void CallUI_InventoryTurnOff()
    {
        if (isNotUIInputPossible) return;
        UI_InventoryTurnOffEvent?.Invoke();
        isTurnOnInventory = !isTurnOnInventory;
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
