using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//public enum ItemEquipmentType
//{   //방어구는 0 ~ 9, 무기 10 ~ 19, 장신구 20 ~ 29
//    Head,
//    Top,
//    Bottoms,
//    Shoes,
//    Gloves,
//    OneHandedWeapon = 10,
//    TwoHandedWeapon = 11,
//    Accessories = 20
//}

public class ItemCraftingController : MonoBehaviour
{
    [SerializeField] private GameObject _ItemCraftingUI;
    [SerializeField] private int _itemTypeListCount;
    [SerializeField] private GameObject _craftingSlotPrefab;
    [SerializeField] private GameObject _craftingItemTypeListPrefab;

    private ItemCraftingManager _itemCraftingManager;
    private ItemDB _itemDB;
    private List<List<int>> _itemList = new List<List<int>>();

    public event Action OnItemCaftingMaterialsEvent;
    public event Action OffItemCaftingMaterialsEvent;

    private void Awake()
    {
        _itemCraftingManager = ItemCraftingManager.Instance;
        _itemCraftingManager.CraftingController = this;
        _itemDB = ItemDB.Instance;
        
        if (_craftingSlotPrefab == null)
            _craftingSlotPrefab = Resources.Load<GameObject>("Resources/Prefabs/UI/ItemCrafting/CreftingSlot");
        if (_craftingItemTypeListPrefab == null)
            _craftingItemTypeListPrefab = Resources.Load<GameObject>("Resources/Prefabs/UI/ItemCrafting/ItemTypeList");
    }
    
    public void CallOnItemCaftingMaterials()
    {
        OnItemCaftingMaterialsEvent?.Invoke();
    }

    public void CallOffItemCaftingMaterials()
    {
        OffItemCaftingMaterialsEvent?.Invoke();
    }

    public void GetRecipe(int id)
    {
        if (_itemDB.GetItemData(id, out ItemData_Test newRecipe))
        {
            int typeNumber = (int)newRecipe.ItemType;
            if (_itemList[typeNumber].Contains(id)) return;

            _itemList[typeNumber].Add(id);
        }
    }

    public void CreateItemListAndSlot(ItemData_Test newRecipe)
    {

    }
}
