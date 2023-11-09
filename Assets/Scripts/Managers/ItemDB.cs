using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDB : CustomSingleton<ItemDB>
{
    protected ItemDB() { }
    [SerializeField] private Inventory _inventory;
    [SerializeField] private ItemExcel _itemList;

    private List<ItemData_Test> _itemDatas;
    private List<ItemStats> _itemStats;
    private List<ItemRecipe> _itemRecipes;
    private int _itemDatasCount;
    private int _itemStatsCount;
    private int _itemRecipesCount;
    
    private ItemCraftingManager _itemCraftingManager;

    public List<ItemData_Test> ItemList { get { return _itemDatas; } }
    public List<ItemStats> ItemStats { get { return _itemStats; } }
    public List<ItemRecipe> ItemRecipes { get { return _itemRecipes; } }
    public bool _test = true;

    private void Awake()
    {
        if (_itemList == null)
            _itemList = Resources.Load<ItemExcel>("ItemData/ItemExcel");
        _itemDatas = new List<ItemData_Test>(_itemList.ItemDatas);
        _itemStats = new List<ItemStats>(_itemList.Stats);
        _itemRecipes = new List<ItemRecipe>(_itemList.Recipe);
        _itemDatasCount = _itemDatas.Count;
        _itemStatsCount = _itemStats.Count;
        _itemRecipesCount = _itemRecipes.Count;

        _itemCraftingManager = ItemCraftingManager.Instance;

        if (_inventory == null)
            _inventory = UI_Manager.Instance.Player.GetComponent<Inventory>();
    }

    private void Update()
    {
        if (_test)
        {
            _inventory.TryAddItem(10010000, 1, out int i);
            _inventory.TryAddItem(10200000, 2, out i);
            _inventory.TryAddItem(10010000, 1, out i);
            _inventory.TryAddItem(10200000, 10, out i);

            _itemCraftingManager.CallAddRecipe(10110000);
            _itemCraftingManager.CallAddRecipe(10110001);
            _itemCraftingManager.CallAddRecipe(10110004);
            _test = false;
        }
    }

    public bool GetItemData(int id, out ItemData_Test itemData)
    {
        for(int i = 0; i < _itemDatasCount; i++)
        {
            if(_itemDatas[i].ID == id)
            {
                itemData = _itemDatas[i];
                return true;
            }
        }
        itemData = null;
        return false;
    }

    public bool GetRecipe(int id, out ItemRecipe itemRcipe)
    {
        for (int i = 0; i < _itemRecipesCount; i++)
        {
            if (_itemRecipes[i].ID == id)
            {
                itemRcipe = _itemRecipes[i];
                return true;
            }
        }
        itemRcipe = null;
        return false;
    }

    public bool GetImage(int id, out Sprite icon)
    {
        for (int i = 0; i < _itemDatasCount; i++)
        {
            if (_itemDatas[i].ID == id)
            {
                icon = _itemDatas[i].Icon;
                return true;
            }
        }
        icon = null;
        return false;
    }

    public bool GetStats(int id, out ItemStats itemStat)
    {
        for (int i = 0; i < _itemStatsCount; i++)
        {
            if (_itemStats[i].ID == id)
            {
                itemStat = _itemStats[i];
                return true;
            }
        }
        itemStat = null;
        return false;
    }

    /// <summary>
    /// º¹»çÇü
    /// </summary>
    /// <param name="id"></param>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public ItemData_Test GetItemData(int id)
    {
        for (int i = 0; i < _itemDatasCount; i++)
        {
            if (_itemDatas[i].ID == id)
                return new ItemData_Test(_itemDatas[i]);
        }
        return null;
    }

    public ItemRecipe GetRecipe(int id)
    {
        for (int i = 0; i < _itemRecipesCount; i++)
        {
            if (_itemRecipes[i].ID == id)
            {
                return new ItemRecipe(_itemRecipes[i]);
            }
        }
        return null;
    }

    public ItemStats GetStats(int id)
    {
        for (int i = 0; i < _itemStatsCount; i++)
        {
            if (_itemStats[i].ID == id)
                return new ItemStats(_itemStats[i]);
        }
        return null;
    }

}
