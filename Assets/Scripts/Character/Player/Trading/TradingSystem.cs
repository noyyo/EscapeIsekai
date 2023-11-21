using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingSystem
{
    private TradingManager economyManager;
    private ItemDB itemDB;
    private List<ItemsSoldByUser> repurchaseItem;
    private InventoryManager inventoryManager;

    public void Init(Func<int,int,int> sellItem , Func<int,int,int> byItem, Func<int,int,int> repurchase)
    {
        economyManager = TradingManager.Instance;
        itemDB = ItemDB.Instance;
        inventoryManager = InventoryManager.Instance;
        repurchaseItem = economyManager.RepurchaseItem;
        sellItem = SellItem;
        byItem = ByItem;
        repurchase = Repurchase;
    }

    private int SellItem(int itemID, int itemCount)
    {
        int sum = 0;
        if (itemDB.GetItemData(itemID, out ItemData_Test itemData))
        {
            if(inventoryManager.CallTryAddItem(itemID, -itemCount))
            {
                sum = itemCount * itemData.Price;
                repurchaseItem.Add(new ItemsSoldByUser(itemID, itemCount));
            }
        }
        else
            Debug.LogError("ID를 확인해 주세요.");
        return sum;
    }

    private int ByItem(int itemID, int itemCount)
    {
        int sum = 0;
        if (itemDB.GetItemData(itemID, out ItemData_Test itemData))
        {
            if (itemCount * itemData.Price < economyManager.PlayerMoney)
                if (inventoryManager.CallTryAddItem(itemID, itemCount))
                    sum = itemCount * itemData.Price;
        }  
        else
            Debug.LogError("ID를 확인해 주세요.");
        return sum;
    }

    private int Repurchase(int index, int itemCount)
    {
        if (itemCount > repurchaseItem[index].itemCount)
        {
            Debug.Log("Repurchase의 itemCount를 제대로 입력해주세요");
            return 0;
        }
            
        int sum = 0;
        itemDB.GetItemData(repurchaseItem[index].itemID, out ItemData_Test itemData);
        if(itemCount * itemData.Price <= economyManager.PlayerMoney)
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
