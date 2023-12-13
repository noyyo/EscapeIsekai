using System;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Manager : CustomSingleton<UI_Manager>
{
    protected UI_Manager() { }
    [SerializeField] private GameObject cavas;
    private GameManager gameManager;
    private SoundManager soundManager;

    public GameObject gathering;
    public GameObject talkManager;
    public GameObject questManager;
    public GameObject dialog;
    public GameObject tutorialUI;
    private GameObject inventoryUI;
    private GameObject itemCraftingUI;
    private GameObject tradingUI;
    private GameObject optionUI;
    private GameObject bossHPBarUI;
    private GameObject confirmationWindow;
    public UI_Gathering UI_gathering;

    private UI_Option option;
    private Transform playerTransform;

    private bool isNotUIInputPossible = false;
    private bool isTurnOnInventory;
    private readonly string buttonSoundName = "ButtonSound";
    private readonly string clickSoundName = "Click";
    private readonly string wrongName = "Wrong";

    public GameObject Canvas { get { return cavas; } }
    public GameObject Inventory_UI { get { return inventoryUI; } }
    public GameObject ItemCrafting_UI { get { return itemCraftingUI; } }
    public GameObject Trading_UI { get { return tradingUI; } }
    public GameObject Option_UI { get { return optionUI; } }
    public GameObject BossHPBarUI { get { return bossHPBarUI; } }
    public GameObject MonsterHPBarMain { get; private set; }
    public GameObject ConfirmationWindow { get { return confirmationWindow; } }
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
    public event Action<Enemy> enemyHPBarUITurnOnEvent;
    public event Action<Enemy> enemyHPBarUITurnOffEvent;

    [HideInInspector] public string itemName;
    [HideInInspector] public string itemExplanation;
    [HideInInspector] public bool isPlaying = false;

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
        playerTransform = gameManager.Player.transform;
    }

    public void Init()
    {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
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
        {
            itemCraftingUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/ItemCraftingUI"), cavas.transform);
            itemCraftingUI.SetActive(false);
        }
            
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

        if(bossHPBarUI == null)
        {
            bossHPBarUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/EnemyHPBar/UI_BossHPBar"), cavas.transform);
            bossHPBarUI.SetActive(false);
        }
        if (MonsterHPBarMain == null)
        {
            MonsterHPBarMain = Instantiate(Resources.Load<GameObject>("Prefabs/UI/EnemyHPBar/HPBarMain"), cavas.transform);
        }
        if(confirmationWindow == null)
        {
            confirmationWindow = Instantiate(Resources.Load<GameObject>("Prefabs/UI/ConfirmationWindow"), cavas.transform);
            confirmationWindow.SetActive(false);
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

    public void CallEnemyHPBarUITurnOnEvent(Enemy enemy)
    {
        enemyHPBarUITurnOnEvent?.Invoke(enemy);
    }

    public void CallEnemyHPBarUITurnOffEvent(Enemy enemy)
    {
        enemyHPBarUITurnOffEvent?.Invoke(enemy);
    }

    public void PlayClickBtnSound()
    {
        soundManager.CallPlaySFX(ClipType.UISFX, buttonSoundName, playerTransform, false, soundValue: 0.01f);
    }

    public void PlayClickSound()
    {
        soundManager.CallPlaySFX(ClipType.UISFX, clickSoundName, playerTransform, false, soundValue: 0.01f);
    }

    public void PlayWrongSound()
    {
        soundManager.CallPlaySFX(ClipType.UISFX, wrongName, playerTransform, false, soundValue: 0.2f);
    }
}
