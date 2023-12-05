using System;
using UnityEngine;

public class UI_Manager : CustomSingleton<UI_Manager>
{
    protected UI_Manager() { }
    [SerializeField] private GameObject cavas;
    public GameObject gathering;
    public UI_Gathering UI_gathering;
    public GameObject talkManager;
    public GameObject questManager;
    public GameObject dialog;
    public GameObject tutorialUI;
    private GameObject inventoryUI;
    private GameObject itemCraftingUI;
    private GameObject tradingUI;
    private GameObject optionUI;
    private UI_Option option;

    private bool isNotUIInputPossible = false;
    private bool isTurnOnInventory;
    public GameObject Canvas { get { return cavas; } }
    public GameObject Inventory_UI { get { return inventoryUI; } }
    public GameObject ItemCrafting_UI { get { return itemCraftingUI; } }
    public GameObject Trading_UI { get { return tradingUI; } }
    public GameObject Option_UI { get { return optionUI; } }
    public bool IsTurnOnInventory { get { return isTurnOnInventory; } }

    public event Action UI_AllTurnOffEvent;
    public event Action UI_InventoryTurnOnEvent;
    public event Action UI_InventoryTurnOffEvent;
    public event Action UI_ItemCraftingTurnOnEvent;
    public event Action UI_ItemCraftingTurnOffEvent;
    public event Action UI_TradingTurnOnEvent;
    public event Action UI_TradingTurnOffEvent;
    public event Action UI_OptionTurnOnEvent;
    public event Action UI_OptionTurnOffEvent;

    [HideInInspector]public string itemName;
    [HideInInspector]public string itemExplanation;
    [HideInInspector]public bool isPlaying = false;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        UI_AllTurnOffEvent += CallUI_InventoryTurnOff;
        UI_AllTurnOffEvent += CallUI_ItemCraftingTurnOff;
        UI_AllTurnOffEvent += CallUI_TradingTurnOff;
        UI_AllTurnOffEvent += CallUI_OptionTurnOff;
    }

    public void Init()
    {
        cavas = GameObject.FindGameObjectWithTag("Canvas");
        if (cavas == null)
            cavas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas"));

        if (gathering == null)
        {
            gathering = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Gathering/GatheringUI"), cavas.transform);
            UI_gathering = gathering.GetComponent<UI_Gathering>();
            gathering.SetActive(false);
        }

        if (inventoryUI == null)
        {
            inventoryUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Inventory/Inventory"), cavas.transform);
            inventoryUI.SetActive(false);
        }

        if (itemCraftingUI == null)
            itemCraftingUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/ItemCraftingUI"), cavas.transform);

        if (questManager == null)
            questManager = Instantiate(Resources.Load<GameObject>("Prefabs/Manager/QuestManager"));

        if (talkManager == null)
            talkManager = Instantiate(Resources.Load<GameObject>("Prefabs/Manager/TalkManager"));

        if (dialog == null)
        {
            dialog = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/UI_Dialog"));
            dialog.GetComponent<Dialog>().questManager = questManager.GetComponent<QuestManager>();
        }

        if (tradingUI == null)
        {
            tradingUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Trading/UI_Trading"), cavas.transform);
            tradingUI.SetActive(false);
        }

        if (optionUI == null)
        {
            optionUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Option"), cavas.transform);
            option = optionUI.GetComponent<UI_Option>();
        }
        if (tutorialUI == null)
        {
            tutorialUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/TutorialUI"), cavas.transform);
            tutorialUI.SetActive(false);
        }
    }

    private void SetIsNotUIInputPossible()
    {
        isNotUIInputPossible = !isNotUIInputPossible;
    }

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

    public void CallUI_TradingTurnOn()
    {
        UI_TradingTurnOnEvent?.Invoke();
    }

    public void CallUI_TradingTurnOff()
    {
        UI_TradingTurnOffEvent?.Invoke();
    }

    public void CallUI_OptionTurnOn()
    {
        if (!option.IsDisplay && !isPlaying)
        {
            UI_OptionTurnOnEvent?.Invoke();
            SetIsNotUIInputPossible();
        }
    }

    public void CallUI_OptionTurnOff()
    {
        if (option.IsDisplay)
        {
            UI_OptionTurnOffEvent?.Invoke();
            SetIsNotUIInputPossible();
        }
    }

}
