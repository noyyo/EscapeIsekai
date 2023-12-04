using System.Collections.Generic;
using UnityEngine;

public class ItemDB : CustomSingleton<ItemDB>
{
    protected ItemDB() { }
    private ItemExcel itemList;
    private Dictionary<int, ItemData> itemDataDic;
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
        itemDataDic = new Dictionary<int, ItemData>();
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
        itemCraftingManager.CallAddRecipe(10113002);
        itemCraftingManager.CallAddRecipe(10113003);
        itemCraftingManager.CallAddRecipe(10113004);
        itemCraftingManager.CallAddRecipe(10113005);
        itemCraftingManager.CallAddRecipe(10113006);
        itemCraftingManager.CallAddRecipe(10113007);
        itemCraftingManager.CallAddRecipe(10113008);
        itemCraftingManager.CallAddRecipe(10113009);
        itemCraftingManager.CallAddRecipe(10114101);
        itemCraftingManager.CallAddRecipe(10114102);
        itemCraftingManager.CallAddRecipe(10114103);
        itemCraftingManager.CallAddRecipe(10114104);
        itemCraftingManager.CallAddRecipe(10114201);
        itemCraftingManager.CallAddRecipe(10114202);
        itemCraftingManager.CallAddRecipe(10116001);
    }

    public bool GetItemData(int id, out ItemData itemData)
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
