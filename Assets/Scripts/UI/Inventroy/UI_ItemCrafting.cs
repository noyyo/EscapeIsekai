using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemCrafting : MonoBehaviour
{
    [SerializeField] private Button _craftingButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private TMP_Text _craftPriceText;
    [SerializeField] private TMP_Text[] _itemText;
    private UI_Manager _ui_Manager;
    private ItemDB _itemDB;
    private ItemCraftingManager _craftingManager;
    private InventoryManager _inventoryManager;
    private Dictionary<string, float> stats;
    

    private void Awake()
    {
        _craftingManager = ItemCraftingManager.Instance;
        _ui_Manager = UI_Manager.Instance;
        _itemDB = ItemDB.Instance;
        _inventoryManager = InventoryManager.Instance;
    }

    public void Start()
    {
        Init();
        _craftingButton.onClick.AddListener(_craftingManager.CallOnCrafting);
        _backButton.onClick.AddListener(_ui_Manager.CallUI_ItemCraftingTurnOff);
        _inventoryButton.onClick.AddListener(() => { _ui_Manager.CallUI_ItemCraftingTurnOff(); _ui_Manager.CallUI_InventoryTurnOn(); });
        _craftingManager.onUpdateUIEvent += UpdatePriceText;
        _craftingManager.onUpdateUIEvent += UpdateItemExplanationText;
        _craftingManager.onUpdateUIEvent += AddMaterialsSlot;
    }

    private void Init()
    {
        if (_craftingButton == null)
            _craftingButton = this.transform.GetChild(2).GetChild(2).GetComponent<Button>();
        if (_craftPriceText == null)
            _craftPriceText = this.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>();
        if (_itemText.Length == 0)
            _itemText = this.transform.GetChild(3).GetComponentsInChildren<TMP_Text>();
        if (_backButton == null)
            _backButton = this.transform.GetChild(4).GetChild(0).GetChild(2).GetComponent<Button>();
        if (_inventoryButton == null)
            _inventoryButton = this.transform.GetChild(4).GetChild(0).GetChild(1).GetComponent<Button>();
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

    //¾ÆÀÌÅÛ ½½·ÔÀ» ´­·¶À»¶§
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
