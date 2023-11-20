using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingController : MonoBehaviour
{
    [SerializeField] private int craftingItemListCount = 3;
    private GameObject craftingItemTypeListPrefab;
    private Transform craftingItemListSpawn;
    private ItemDB itemDB;
    private Dictionary<int,int> itemEquipmentID;
    private Dictionary<int, int> itemConsumableID;
    private Dictionary<int, int> itemMaterialID;
    private List<ItemCraftingItemTypeList> itemTypeLists;
    private UI_Manager ui_Manager;

    private void Awake()
    {
        itemDB = ItemDB.Instance;
        ui_Manager = UI_Manager.Instance;

        itemEquipmentID = new Dictionary<int, int>();
        itemConsumableID = new Dictionary<int, int>();
        itemMaterialID = new Dictionary<int, int>();
        itemTypeLists = new List<ItemCraftingItemTypeList>();

        craftingItemTypeListPrefab = Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/ItemTypeList");
        craftingItemListSpawn = ui_Manager.ItemCrafting_UI.transform.GetChild(1).GetChild(0).GetChild(0);
        CreateItemList();
    }

    private void CreateItemList()
    {
        string[] str = { "���", "�Ҹ�ǰ", "���" };
        for(int i = 0;  i < craftingItemListCount; i++)
        {
            GameObject obj = Instantiate(craftingItemTypeListPrefab);
            obj.transform.SetParent(craftingItemListSpawn, false);
            itemTypeLists.Add(obj.GetComponent<ItemCraftingItemTypeList>());
            itemTypeLists[i].listName.text = str[i];
        }

    }

    public void AddRecipe(int id)
    {
        if (10120000 > id && id >= 10110000)
        {
            if (itemDB.GetRecipe(id, out ItemRecipe newRecipe))
            {
                int craftingItemIndex= (newRecipe.CraftingID / 1000) % 1000;
                switch (craftingItemIndex)
                {
                    case >= 200:
                        if (itemMaterialID.ContainsKey(craftingItemIndex)) return;
                        itemMaterialID.Add(craftingItemIndex % 100, newRecipe.CraftingID / 1000);
                        //���
                        break;
                    case >= 100:
                        if (itemConsumableID.ContainsKey(craftingItemIndex)) return;
                        itemConsumableID.Add(craftingItemIndex % 100, newRecipe.CraftingID / 1000);
                        //�Һ�
                        break;
                    default:
                        if (itemEquipmentID.ContainsKey(craftingItemIndex)) return;
                        itemEquipmentID.Add(craftingItemIndex % 100, newRecipe.CraftingID / 1000);
                        //���
                        break;
                }
                itemTypeLists[craftingItemIndex / 100].AddRecipe(newRecipe);
            }
        }
        return;
    }
}
