using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_ItemCrafting : MonoBehaviour
{
    private ItemCraftingManager _craftingManager;
    private UI_Manager _uimanager;
    private TMP_Text _craftPriceText;
    private TMP_Text[] _itemText;
    private Dictionary<string, float> stats;
    private ItemDB _itemDB;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _craftingManager = ItemCraftingManager.Instance;
        _uimanager = UI_Manager.Instance;
        _itemDB = ItemDB.Instance;
        _craftPriceText = _craftingManager.ItemCaftingMaterials_UI.transform.GetChild(1).GetComponent<TMP_Text>();
        _inventoryManager = InventoryManager.Instance;
    }

    public void Start()
    {
        _itemText = _craftingManager.ItemExplanation_UI.GetComponentsInChildren<TMP_Text>();
        _craftingManager.onUpdateUIEvent += UpdatePriceText;
        _craftingManager.onUpdateUIEvent += UpdateItemExplanationText;
        _craftingManager.onUpdateUIEvent += AddMaterialsSlot;

    }

    public void UpdatePriceText(ItemRecipe clickSlot)
    {
        _craftPriceText.text = "Crafting Price : " + clickSlot.CraftingPrice;
    }

    public void UpdateItemExplanationText(ItemRecipe clickSlot)
    {
        _itemDB.GetItemData(clickSlot.CraftingID, out ItemData_Test craftingItem);
        _itemText[0].text = craftingItem.ItemName;
        _itemText[1].text = craftingItem.ItemExplanation;

        _itemDB.GetStats(craftingItem.ID, out ItemStats itemStats);
        stats = itemStats.Stats;
        string str = "";
        foreach (var i in stats)
        {
            if ((int)i.Value > 0)
                str += (i.Key + " : "+ (int)i.Value + "\n");
        }

        _itemText[2].text = str;
    }

    //public void UpdateSlotImage(ItemRecipe clickSlot)
    //{
    //    int materialsLength = clickSlot.Materials.Length;
    //    for (int i = 0; i< materialsLength; i++)
    //    {
    //        _itemDB.GetImage(clickSlot.Materials[i], out Sprite icon);
    //    }

    //}

    public void AddMaterialsSlot(ItemRecipe clickSlot)
    {
        Sprite[] sprites = _inventoryManager.CallIsCheckItems(clickSlot, out int[] sum);
        _craftingManager.MaterialsSlots[0].GetItemData(sprites[0], 0, 0);
        int materialsLength = _craftingManager.ClickSlot.Materials.Length;
        for (int i = 1; i <= materialsLength; i++)
        {
            _craftingManager.MaterialsSlots[i].GetItemData(sprites[i], _craftingManager.ClickSlot.MaterialsCount[i - 1], sum[i - 1]);
        }
        if (materialsLength != 7)
        {
            for (int i = materialsLength + 1; i <= 7; i++)
            {
                _craftingManager.MaterialsSlots[i].Init();
            }
        }
    }
}
