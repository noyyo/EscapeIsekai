using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TradingController : MonoBehaviour
{
    private ItemDB itemDB;
    private InventoryManager inventoryManager;
    private TradingManager tradingManager;
    private UI_Manager ui_Manager;
    
    private Transform shopSlotSpawnTransform;
    private Transform playerSlotSpawnTransform;
    private GameObject slotPrefab;
    // ��� ������ ����ֽ��ϴ�.
    private List<UI_TradingSlot>[] tradingSlotList;
    // �÷��̾� <- InventoryManager���� ����
    private Dictionary<int, Item>[] playerInventoryDatas;
    private List<int>[] shopItemDatas;
    // �籸�Ÿ� ���� ���������� (Manager�� ������)
    private List<ItemsSoldByUser> repurchaseItem;

    private void Awake()
    {
        ui_Manager = UI_Manager.Instance;
        itemDB = ItemDB.Instance;
        tradingManager = TradingManager.Instance;
        tradingManager.Init(SellItem, BuyItem, Repurchase);
        inventoryManager = InventoryManager.Instance;
        slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Trading/TradingSlot");
        Init();
    }

    private void Init()
    {
        shopSlotSpawnTransform = ui_Manager.Trading_UI.transform.GetChild(3).GetChild(0).GetChild(0);
        playerSlotSpawnTransform = ui_Manager.Trading_UI.transform.GetChild(5).GetChild(0).GetChild(0);
        tradingSlotList = tradingManager.TradingSlotList;
        playerInventoryDatas = inventoryManager.ItemDics;
        shopItemDatas = tradingManager.ShopItemIDList;
        repurchaseItem = tradingManager.RepurchaseItem;
        tradingManager.addShopItem += AddShopItem;
        tradingManager.clickSlotButtonEvent += () => ClickSlot(tradingManager.ClickID);
        tradingManager.clickBuyButtonEvent += ClickActionButton;
        CreatePlayerSlot();
    }

    private void CreatePlayerSlot()
    {
        int count = inventoryManager.InventroySlotCount;
        for (int i = 0; i < count; i++)
        {
            UI_TradingSlot slot = Instantiate(slotPrefab, playerSlotSpawnTransform).GetComponent<UI_TradingSlot>();
            slot.isPlayer = true;
            slot.UniqueIndex = i;
            tradingSlotList[0].Add(slot);
        }
    }

    private void CreateShopSlot()
    {
        UI_TradingSlot slot = Instantiate(slotPrefab, shopSlotSpawnTransform).GetComponent<UI_TradingSlot>();
        slot.UniqueIndex = tradingSlotList[1].Count;
        tradingSlotList[1].Add(slot);
    }
    
    private void AddShopItem(int id)
    {
        if (itemDB.GetItemData(id, out ItemData_Test itemData))
        {
            int category = (id / 100000) % 10;
            if (category > 2)
                return;
            int index = shopItemDatas[category].IndexOf(id);
            if(tradingSlotList[1].Count <= shopItemDatas[category].Count)
                CreateShopSlot();
            if (index < 0)
                shopItemDatas[category].Add(id);
        }
        else
            Debug.Log("id�� Ȯ���� �ּ���.");
    }

    private void ClickSlot(int id)
    {
        if(itemDB.GetItemData(id, out ItemData_Test itemData))
        {
            if(itemDB.GetStats(id, out ItemStats itemStats))
            {
                StringBuilder sb = new StringBuilder();

                if (itemStats.HP > 0)
                    sb.Append("HP : " + itemStats.HP + "\n");
                if (itemStats.Temperature > 0)
                    sb.Append("Temperature : " + itemStats.Temperature + "\n");
                if (itemStats.ATK > 0)
                    sb.Append("ATK : " + itemStats.ATK + "\n");
                if (itemStats.DEF > 0)
                    sb.Append("DEF : " + itemStats.DEF + "\n");
                if (itemStats.Speed > 0)
                    sb.Append("Speed : " + itemStats.Speed + "\n");
                if (itemStats.Stamina > 0)
                    sb.Append("Stamina : " + itemStats.Stamina + "\n");

                tradingManager.itemExplanationText(itemData.ItemName, itemData.ItemExplanation, sb.ToString());
            }
            else
            {
                tradingManager.itemExplanationText(itemData.ItemName, itemData.ItemExplanation, "");
            }
        }
    }

    private void ClickActionButton()
    {
        if (tradingManager.isPlayerSlotClick)
        {
            ClickSellButton();
            tradingManager.CallOnDisplayPlayerSlot();
        }
        else
        {
            if (tradingManager.displayShopItemCategory == 3)
                ClickRepurchase();
            else
                ClickBuyButton();
            tradingManager.CallOnDisplayShopSlot();
        }
    }

    private void ClickBuyButton()
    {
        if (!tradingManager.tryByitem(tradingManager.ClickID, 1))
            Debug.Log("���� �����մϴ�.");
        else
            tradingManager.CallOnDisplayPlayerSlot();
    }

    private void ClickSellButton()
    {
        if(!tradingManager.trySellItem(tradingManager.ClickID, 1))
            Debug.Log("�������� �����ϴ�.");
        else
            tradingManager.CallOnDisplayShopSlot();
    }

    private void ClickRepurchase()
    {
        if (!tradingManager.tryRepurchase(tradingManager.ClickIndex, 1))
            Debug.Log("���� �����մϴ�.");
        else
            tradingManager.CallOnDisplayPlayerSlot();
    }

    private int SellItem(int itemID, int itemCount)
    {
        int sum = 0;
        if (itemDB.GetItemData(itemID, out ItemData_Test itemData))
        {
            if (inventoryManager.CallTryAddItem(itemID, -itemCount))
            {
                sum = itemCount * itemData.Price;
                if(repurchaseItem.Count >= tradingManager.RepurchaseItemMaxCount)
                    repurchaseItem.RemoveAt(0);

                repurchaseItem.Add(new ItemsSoldByUser(itemID, itemCount));

                if (tradingSlotList[1].Count <= repurchaseItem.Count)
                    CreateShopSlot(); 
            }
        }
        else
            Debug.LogError("ID�� Ȯ���� �ּ���.");
        return sum;
    }

    private int BuyItem(int itemID, int itemCount)
    {
        int sum = 0;
        if (itemDB.GetItemData(itemID, out ItemData_Test itemData))
        {
            if (itemCount * itemData.Price <= tradingManager.PlayerMoney)
                if (inventoryManager.CallTryAddItem(itemID, itemCount))
                    sum = itemCount * itemData.Price;
        }
        else
            Debug.LogError("ID�� Ȯ���� �ּ���.");
        return sum;
    }

    private int Repurchase(int index, int itemCount)
    {
        if (itemCount > repurchaseItem[index].itemCount)
        {
            Debug.Log("Repurchase�� itemCount�� ����� �Է����ּ���");
            return 0;
        }

        int sum = 0;
        itemDB.GetItemData(repurchaseItem[index].itemID, out ItemData_Test itemData);
        if (itemCount * itemData.Price <= tradingManager.PlayerMoney)
        {
            if (inventoryManager.CallTryAddItem(itemData.ID, itemCount))
            {
                sum = itemCount * itemData.Price;
                if ((repurchaseItem[index].itemCount -= itemCount) == 0)
                    repurchaseItem.RemoveAt(index);
            }
        }
        return sum;
    }
}
