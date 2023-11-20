using System.Collections.Generic;
using UnityEngine;

public class ItemDB : CustomSingleton<ItemDB>
{
    protected ItemDB() { }
    private ItemExcel itemList;
    private Dictionary<int, ItemData_Test> itemDataDic;
    private Dictionary<int, ItemRecipe> itemRecipeDic;
    private Dictionary<int, ItemStats> itemStatsDic;

    private Inventory inventory;
    private GameManager gameManager;
    private ItemCraftingManager itemCraftingManager;

    private void Awake()
    {
        itemList = Resources.Load<ItemExcel>("ItemData/ItemExcel");
        gameManager = GameManager.Instance;
        itemCraftingManager = ItemCraftingManager.Instance;
        ItemDataInit();
    }

    private void Start()
    {
        if (inventory == null)
            inventory = gameManager.Player.GetComponent<Inventory>();
        DefaultItem();
    }

    private void ItemDataInit()
    {
        itemDataDic = new Dictionary<int, ItemData_Test>();
        itemRecipeDic = new Dictionary<int, ItemRecipe>();
        itemStatsDic = new Dictionary<int, ItemStats>();

        int count = itemList.ItemDatas.Count;
        for (int i = 0; i< count; i++)
        {
            itemDataDic.Add(itemList.ItemDatas[i].ID, itemList.ItemDatas[i]);
        }

        count = itemList.Recipe.Count;
        for (int i = 0; i < count; i++)
        {
            itemRecipeDic.Add(itemList.Recipe[i].ID, itemList.Recipe[i]);
        }

        count = itemList.Stats.Count;
        for (int i = 0; i < count; i++)
        {
            itemStatsDic.Add(itemList.Stats[i].ID, itemList.Stats[i]);
        }

    }

    private void DefaultItem()
    {
        inventory.TryAddItem(10010000, 1, out int i);
        Debug.Log("통과");
        inventory.TryAddItem(10200000, 2, out i);
        Debug.Log("통과");
        inventory.TryAddItem(10010000, 1, out i);
        Debug.Log("통과");
        inventory.TryAddItem(10200000, 10, out i);
        Debug.Log("통과");

        itemCraftingManager.CallAddRecipe(10110000);
        itemCraftingManager.CallAddRecipe(10110001);
        itemCraftingManager.CallAddRecipe(10110004);
    }

    public bool GetItemData(int id, out ItemData_Test itemData)
    {
        if (itemDataDic.ContainsKey(id))
        {
            itemData = itemDataDic[id];
            return true;
        }
        itemData = null;
        return false;
    }

    public bool GetRecipe(int id, out ItemRecipe itemRcipe)
    {
        if (itemRecipeDic.ContainsKey(id))
        {
            itemRcipe = itemRecipeDic[id];
            return true;
        }
        itemRcipe = null;
        return false;
    }

    public bool GetRecipeCraftID(int craftID, out ItemRecipe itemRcipe)
    {
        foreach(var recipe in itemRecipeDic.Values)
        {
            if(recipe.CraftingID == craftID)
            {
                itemRcipe = recipe;
                return true;
            }
        }
        itemRcipe = null;
        return false;
    }

    public bool GetImage(int id, out Sprite icon)
    {
        if (itemDataDic.ContainsKey(id))
        {
            icon = itemDataDic[id].Icon;
            return true;
        }
        icon = null;
        return false;
    }

    public bool GetStats(int id, out ItemStats itemStats)
    {
        if (itemStatsDic.ContainsKey(id))
        {
            itemStats = itemStatsDic[id];
            return true;
        }
        itemStats = null;
        return false;
    }
}
