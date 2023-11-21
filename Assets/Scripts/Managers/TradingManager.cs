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

    [SerializeField] private int queueMaxCount = 10;
    private GameManager gameManager;
    private TradingSystem playerEconomicSystem;
    private List<ItemsSoldByUser> repurchaseItem;
    private int playerMoney = 0;

    public List<ItemsSoldByUser> RepurchaseItem { get { return repurchaseItem; } }
    public int PlayerMoney { get { return playerMoney; } }

    private Func<int, int, int> sellItem;
    private Func<int, int, int> byitem;
    private Func<int, int, int> repurchase;

    public Func<int, int, bool> trySellItem;
    public Func<int, int, bool> tryByitem;
    public Func<int, int, bool> tryRepurchase;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        repurchaseItem = new List<ItemsSoldByUser>();
        playerEconomicSystem = new TradingSystem();
        playerEconomicSystem.Init(sellItem, byitem, repurchase);
    }

    private void Start()
    {
        trySellItem = (itemID, itemCount) => SellItem(sellItem(itemID, itemCount));
        tryByitem = (itemID, itemCount) => Byitem(byitem(itemID, itemCount));
        tryRepurchase = (itemID, itemCount) => Repurchase(repurchase(itemID, itemCount));
    }

    private bool SellItem(int addMoney)
    { 
        if (addMoney != 0)
        {
            playerMoney += addMoney;
            return true;
        }
        return false;  
    }

    private bool Byitem(int addMoney)
    {
        if (addMoney != 0)
        {
            playerMoney -= addMoney;
            return true;
        }
        return false; 
    }

    private bool Repurchase(int addMoney)
    {
        if (addMoney != 0)
        {
            playerMoney -= addMoney;
            return true;
        }
        return false;
    }
}
