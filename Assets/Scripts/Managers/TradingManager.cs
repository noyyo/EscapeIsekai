using System;
using System.Collections;
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
    [Tooltip("마지막 재구매는 제외합니다.")][SerializeField] 
    private int shopCategoryCount = 3;
    private GameManager gameManager;
    private UI_Manager ui_Manager;
    private List<UI_TradingSlot>[] tradingSlotList;
    private List<int>[] shopItemIDList;
    private List<ItemsSoldByUser> repurchaseItem;
    private int clickID;
    private int clickIndex;
    private int playerMoney = 0;

    public List<UI_TradingSlot>[] TradingSlotList { get { return tradingSlotList; } }
    public List<ItemsSoldByUser> RepurchaseItem { get { return repurchaseItem; } }
    public List<int>[] ShopItemIDList { get { return shopItemIDList; } }
    public int PlayerMoney { get { return playerMoney; } }
    public int RepurchaseItemMaxCount { get { return repurchaseItemMaxCount; } }
    public int ShopCategoryCount { get { return shopCategoryCount; } }
    public int ClickID { get { return clickID; } }
    public int ClickIndex { get { return clickIndex; } }

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
    public Action<string, string, string> itemExplanationText;
   
    public int displayPlayerItemCategory = 0;
    public int displayShopItemCategory = 0;
    public bool isPlayerSlotClick;

    private void Awake()
    {
        gameManager = GameManager.Instance;
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
        trySellItem = (itemID, itemCount) => SellItem(sellItem(itemID, itemCount));
        tryByitem = (itemID, itemCount) => Byitem(byitem(itemID, itemCount));
        tryRepurchase = (itemID, itemCount) => Repurchase(repurchase(itemID, itemCount));
        ui_Manager.UI_TradingTurnOnEvent += CallOnDisplayPlayerSlot;
        ui_Manager.UI_TradingTurnOnEvent += CallOnDisplayShopSlot;
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
        addShopItem(10100000);
        addShopItem(10000000);
        addShopItem(10001000);
        addShopItem(10002000);
        addShopItem(10010000);
    }

    private bool SellItem(int addMoney)
    {
        if (addMoney != 0)
        {
            playerMoney += addMoney;
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
        clickID = id;
    }

    public void SetClickIndex(int index)
    {
        clickIndex = index;
    }

    public void CallOnDisplayPlayerSlot()
    {
        displayPlayerSlotEvent?.Invoke(displayPlayerItemCategory);
    }

    public void CallOnDisplayShopSlot()
    {
        displayShopSlotEvent?.Invoke(displayShopItemCategory);
    }

    public void CallOnClickBuyButton()
    {
        clickBuyButtonEvent?.Invoke();
    }

    public void CallOnMoneyTextUpdate()
    {
        moneyTextUpdateEvent?.Invoke();
    }
}
