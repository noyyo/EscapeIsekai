using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Item
{
    public int id;
    public int count;
    public int slotIdex;
}


public class Inventory : MonoBehaviour
{
    [SerializeField] private int _equipmentMaxSlot;
    [SerializeField] private int _consumableMaxSlot;
    [SerializeField] private int _etcSlotMaxSlot;

    private List<Item> _equipmentSlots;
    private List<Item> _consumableSlots;
    private List<Item> _etcSlots;
    private ItemDB _itemDB;

    private int _equipmentItemsIndexCount;
    private int _consumableItemsIndexCount;
    private int _etcItemsIndexCount;

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        InitInventory();
    }

    private void InitInventory()
    {
        _equipmentItemsIndexCount = 0;
        _consumableItemsIndexCount = 0;
        _etcItemsIndexCount = 0;

        _equipmentSlots = new List<Item>();
        _consumableSlots = new List<Item>();
        _etcSlots = new List<Item>();
    }

    public bool AddItem(int id, int count)
    {
        if (_itemDB.GetItemData(id, out ItemData newItem))
        {
            switch (newItem.ItemType)
            {
                case ItemType.Equipment:
                    foreach (Item item in _equipmentSlots)
                    {
                        if (item.id == id)
                        {
                            //if (item.count == )
                            //{

                            //}
                        }
                    }
                    break;
                case ItemType.Consumable:
                    break;
                default:
                    break;
            }

        }


        return true;
    }
}
