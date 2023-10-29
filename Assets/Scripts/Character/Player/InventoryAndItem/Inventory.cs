using PolyAndCode.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotInfo
{
    public int id;
    public int count;
    public int slotIdex;
}


public class Inventory : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;

    [SerializeField]
    private int _dataLength;

    private List<ItemSlotInfo> _contactList = new List<ItemSlotInfo>();
    private ItemDB _itemDB;

    private int _equipmentItemsIndexCount;
    private int _consumableItemsIndexCount;
    private int _materialItemsIndexCount;
    private int _etcItemsIndexCount;

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        InitInventory();
        InitData();
        _recyclableScrollRect.DataSource = this;
    }

    private void InitInventory()
    {
        _equipmentItemsIndexCount = 0;
        _consumableItemsIndexCount = 0;
        _materialItemsIndexCount = 0;
        _etcItemsIndexCount = 0;
    }

    private void InitData()
    {
        if (_contactList != null) _contactList.Clear();

        string[] genders = { "Male", "Female" };
        for (int i = 0; i < _dataLength; i++)
        {
            //ContactInfo obj = new ContactInfo();
            //obj.Name = i + "_Name";
            //obj.Gender = genders[Random.Range(0, 2)];
            //obj.id = "item : " + i;
            //_contactList.Add(obj);
        }
    }

    public bool AddItem(int id, int count)
    {
        //if (_itemDB.GetItemData(id, out ItemData newItem))
        //{
        //    switch (newItem.ItemType)
        //    {
        //        case ItemType.Equipment:
        //            foreach (Item item in _equipmentSlots)
        //            {
        //                if (item.id == id)
        //                {
        //                    //if (item.count == )
        //                    //{

        //                    //}
        //                }
        //            }
        //            break;
        //        case ItemType.Consumable:
        //            break;
        //        default:
        //            break;
        //    }

        //}


        return true;
    }

    public int GetItemCount()
    {
        return _contactList.Count;
    }

    public void SetCell(ICell cell, int index)
    {
        Slot item = cell as Slot;
        //item.ConfigureCell(_contactList[index], index);
    }
}
