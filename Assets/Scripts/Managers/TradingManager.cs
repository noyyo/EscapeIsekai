using System;
using System.Collections.Generic;
using UnityEngine;
public class ItemsSoldByUser
{
    public int itemID;
    public int itemCount;

    public ItemsSoldByUser(int id, int count)
    {
        itemID = id;
        itemCount = count;
    }
}

public class TradingManager : CustomSingleton<TradingManager>
{
    protected TradingManager() { }

    //[Tooltip("상한값 설정")][Range(1f,5f)][SerializeField] private float supremum = 1.5f;
    //[Tooltip("하한값 설정")][Range(0f, 1f)][SerializeField] private float infimum = 0.5f;
    [SerializeField] private int repurchaseItemMaxCount = 10;
    [Tooltip("마지막 재구매는 제외합니다.")]
    [SerializeField]
    private int shopCategoryCount = 3;
    private UI_Manager ui_Manager;
    private GameManager gameManager;
    private SoundManager soundManager;
    private Transform playerTransform;
    private List<UI_TradingSlot>[] tradingSlotList;
    private List<int>[] shopItemIDList;
    private List<ItemsSoldByUser> repurchaseItem;
    private int displayPlayerItemCategory;
    private int displayShopItemCategory;
    private bool isPlayerSlotClick;
    private bool isPreviousPlayerSlotClick;
    private int currentClickID;
    private int currentClickIndex;
    private int previousClickID;
    private int previousClickIndex;
    private int playerMoney = 0;
    private readonly string coinSoundName = "CoinSound";

    public List<UI_TradingSlot>[] TradingSlotList { get { return tradingSlotList; } }
    public List<ItemsSoldByUser> RepurchaseItem { get { return repurchaseItem; } }
    public List<int>[] ShopItemIDList { get { return shopItemIDList; } }
    public int PlayerMoney { get { return playerMoney; } }
    public int RepurchaseItemMaxCount { get { return repurchaseItemMaxCount; } }
    public int ShopCategoryCount { get { return shopCategoryCount; } }
    public int CurrentClickID { get { return currentClickID; } }
    public int CurrentClickIndex { get { return currentClickIndex; } }
    public int PreviousClickID { get { return previousClickID; } }
    public int PreviousClickIndex { get { return previousClickIndex; } }
    public int DisplayPlayerItemCategory { get { return displayPlayerItemCategory; } }
    public int DisplayShopItemCategory { get { return displayShopItemCategory; } }
    public bool IsPlayerSlotClick { get { return isPlayerSlotClick; } }
    public bool IsPreviousPlayerSlotClick { get { return isPreviousPlayerSlotClick; } }

    private Func<int, int, int> sellItem;
    private Func<int, int, int> byitem;
    private Func<int, int, int> repurchase;

    public Func<int, int, bool> trySellItem;
    public Func<int, int, bool> tryByitem;
    public Func<int, int, bool> tryRepurchase;
    public Action<int> addShopItem;
    public event Action<int> displayPlayerSlotEvent;
    public event Action<int> displayShopSlotEvent;
    public event Action clickSlotButtonEvent;
    public event Action clickBuyButtonEvent;
    public event Action moneyTextUpdateEvent;
    public Action<int> addMoney;
    public Action<string, string, string> itemExplanationText;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        ui_Manager = UI_Manager.Instance;
        repurchaseItem = new List<ItemsSoldByUser>();
        tradingSlotList = new List<UI_TradingSlot>[2];
        for (int i = 0; i < 2; i++)
            tradingSlotList[i] = new List<UI_TradingSlot>();
        shopItemIDList = new List<int>[shopCategoryCount];
        for (int i = 0; i < shopCategoryCount; i++)
            shopItemIDList[i] = new List<int>();
    }

    private void Start()
    {
        playerTransform = gameManager.Player.transform;
        trySellItem = (itemID, itemCount) => SellItem(sellItem(itemID, itemCount));
        tryByitem = (itemID, itemCount) => Byitem(byitem(itemID, itemCount));
        tryRepurchase = (itemID, itemCount) => Repurchase(repurchase(itemID, itemCount));
        ui_Manager.UI_TradingTurnOnEvent += CallOnDisplayPlayerSlot;
        ui_Manager.UI_TradingTurnOnEvent += CallOnDisplayShopSlot;
        addMoney += AddMoney;
        defaultAddShopItem();
    }

    public void Init(Func<int, int, int> newSellItem, Func<int, int, int> newByItem, Func<int, int, int> newRepurchase)
    {
        sellItem = newSellItem;
        byitem = newByItem;
        repurchase = newRepurchase;
    }

    private void defaultAddShopItem()
    {
        addShopItem(10114101);
        addShopItem(10202001);
        addShopItem(10202002);
        addShopItem(10202003);
        addShopItem(10202004);
        addShopItem(10202005);
        addShopItem(10202006);
        addShopItem(10202007);
        addShopItem(10202008);
    }

    private bool SellItem(int addMoney)
    {
        if (addMoney != 0)
        {
            playerMoney += addMoney;
            InitClickID();
            CallOnMoneyTextUpdate();
            return true;
        }
        return false;
    }

    private bool Byitem(int addMoney)
    {
        if (addMoney != 0)
        {
            playerMoney -= addMoney;
            InitClickID();
            CallOnMoneyTextUpdate();
            return true;
        }
        return false;
    }

    private bool Repurchase(int addMoney)
    {
        if (addMoney != 0)
        {
            playerMoney -= addMoney;
            InitClickID();
            CallOnMoneyTextUpdate();
            return true;
        }
        return false;
    }

    public void CallOnClickSlotButton()
    {
        clickSlotButtonEvent?.Invoke();
    }

    public void SetClickID(int id)
    {
        previousClickID = currentClickID;
        currentClickID = id;
    }

    public void SetClickIndex(int index)
    {
        previousClickIndex = currentClickIndex;
        currentClickIndex = index;
    }

    public void CallOnDisplayPlayerSlot()
    {
        displayPlayerSlotEvent?.Invoke(DisplayPlayerItemCategory);
    }

    public void CallOnDisplayShopSlot()
    {
        displayShopSlotEvent?.Invoke(DisplayShopItemCategory);
    }

    public void CallOnClickBuyButton()
    {
        clickBuyButtonEvent?.Invoke();
    }

    public void CallOnMoneyTextUpdate()
    {
        moneyTextUpdateEvent?.Invoke();
    }

    private void AddMoney(int moeny)
    {
        playerMoney += moeny;
        CallOnMoneyTextUpdate();
    }

    public void PlayCoinSound()
    {
        soundManager.CallPlaySFX(ClipType.UISFX, coinSoundName, playerTransform, false);
    }

    private void InitClickID()
    {
        currentClickID = -1;
        currentClickIndex = -1;
    }

    public void SetIsPlayerSlotClick(bool newValue)
    {
        isPreviousPlayerSlotClick = isPlayerSlotClick;
        isPlayerSlotClick = newValue;
    }

    public void SetDisplayPlayerItemCategory(int newValue)
    {
        displayPlayerItemCategory = newValue;
    }

    public void SetDisplayShopItemCategory(int newValue)
    {
        displayShopItemCategory = newValue;
    }
}
