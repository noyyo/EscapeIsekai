using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotInfo
{
    public int id;
    public int count;
    public int index;

    public ItemSlotInfo(int newID, int newindex, int newCount = 1)
    {
        id = newID;
        count = newCount;
        index = newindex;
    }
}

public enum DisplayType
{
    Equipment,
    USE,
    Material,
    ETC
}

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int _dataLength;

    private List<ItemSlotInfo> _equipmentItemList = new List<ItemSlotInfo>();
    private List<ItemSlotInfo> _consumableItemList = new List<ItemSlotInfo>();
    private List<ItemSlotInfo> _materialItemList = new List<ItemSlotInfo>();
    private List<ItemSlotInfo> _etcItemList = new List<ItemSlotInfo>();
    private UI_Inventory _uI_Inventory;
    private ItemDB _itemDB;
    private int[] _ItemCount;
    private DisplayType _displayType;

    public int dataLength { get { return _dataLength; } }

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        _uI_Inventory = GetComponent<UI_Inventory>();
        InitInventory();
    }

    private void InitInventory()
    {
        _equipmentItemList.Clear();
        _consumableItemList.Clear();
        _materialItemList.Clear();
        _etcItemList.Clear();
        _ItemCount = new int[4];
        _displayType = DisplayType.Equipment;
    }

    public bool AddItem(int id, int count, out int errorItemCount)
    {
        errorItemCount = count;
        bool isAddItem = false;
        if (_itemDB.GetItemData(id, out ItemData newItem))
        {
            switch (newItem.ItemType)
            {
                case ItemType.Equipment:
                    if(count >= 0)
                    {
                        if(AddList(_equipmentItemList, count, (int)ItemType.Equipment, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    else
                    {
                        if(SubList(_equipmentItemList, count, (int)ItemType.Equipment, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    break;
                case ItemType.Consumable:
                    if(count >= 0)
                    {
                        if (AddList(_consumableItemList, count, (int)ItemType.Consumable, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_consumableItemList, count, (int)ItemType.Consumable, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    break;
                case ItemType.Material:
                    if(count >= 0)
                    {
                        if (AddList(_materialItemList, count, (int)ItemType.Material, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_materialItemList, count, (int)ItemType.Material, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    break;
                default:
                    if(count>= 0)
                    {
                        if (AddList(_etcItemList, count, (int)ItemType.ETC, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_etcItemList, count, (int)ItemType.ETC, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    break;
            }
        }

        return isAddItem;
    }

    private bool AddList(List<ItemSlotInfo> itemList, int count, int slotType, in ItemData newItem, out int errorItemCount)
    {
        
        int itemListCount = itemList.Count;
        if (itemListCount == _dataLength)
        {
            errorItemCount = count;
            return false;
        }

        errorItemCount = 0;
        int id = newItem.ID;

        if(slotType != (int)ItemType.Equipment)
        {
            for(int i = 0; i < itemListCount; i++)
            {
                if (itemList[i].id == id)
                {
                    int newItemMaxCount = newItem.MaxCount;
                    int nowItemCount = itemList[i].count;
                    if (newItemMaxCount != nowItemCount)
                    {
                        nowItemCount += count;
                        if (nowItemCount > newItemMaxCount)
                        {
                            count = nowItemCount - newItemMaxCount;
                            ItemSlotInfo iteminfo = new ItemSlotInfo(id, itemList.Count, count);

                            _uI_Inventory.slotArray[itemList.Count].AddItem(newItem, itemList.Count, count);
                            _uI_Inventory.slotArray[i].SetSlotCount(count);
                            itemList.Add(iteminfo);
                            _ItemCount[slotType]++;
                            if (_ItemCount[slotType] == _dataLength)
                            {
                                errorItemCount = count;
                                return false;
                            }
                        }
                        _uI_Inventory.slotArray[i].SetSlotCount(count);
                        return true;
                    }
                }
            }
        }

        //장비류는 최대가 1개
        for(int i = 0; i < count; i++)
        {
            ItemSlotInfo itemInfo = new ItemSlotInfo(id, itemList.Count, 1);
            itemList.Add(itemInfo);
            _uI_Inventory.slotArray[itemList.Count].AddItem(newItem, itemList.Count, 1);
            _ItemCount[slotType]++;
            if (_ItemCount[slotType] == _dataLength)
            {
                errorItemCount = count - (i + 1);
                return false;
            }
        }
        return true;
    }

    private bool SubList(List<ItemSlotInfo> itemList, int count, int slotType, in ItemData newItem, out int ErrorItemCount)
    {
        
        if (itemList.Count == 0)
        {
            ErrorItemCount = count;
            return false;
        }

        ErrorItemCount = 0;
        bool isSub = false;
        int id = newItem.ID;
        Stack stack = new Stack();
        for(int i = 0; i < _ItemCount[slotType]; i++)
        {
            if (itemList[i].id == id)
            {
                itemList[i].count += count;
                if (itemList[i].count <= 0)
                {
                    count = -itemList[i].count;
                    _ItemCount[slotType]--;
                    stack.Push(i);
                    continue;
                }
                _uI_Inventory.slotArray[i].SetSlotCount(count);
                isSub = true;
                break;
            }
        }
        for(int i = 0; i < stack.Count;)
        {
            int n = (int)stack.Pop();
            _uI_Inventory.slotArray[n].SetSlotCount(-9999);
            itemList.RemoveAt(n);
        }
        return isSub;
    }

    public void SortInventory()
    {
        _equipmentItemList.Sort((n1,n2) => n1.id.CompareTo(n2.id));
    }

    public void OnDisplaySlot()
    {
        
        switch (_displayType)
        {
            case DisplayType.Equipment:
                DisplayClear();
                foreach (ItemSlotInfo item in _equipmentItemList)
                {
                    _uI_Inventory.slotArray[item.index].AddItem(item);
                }
                break;
            case DisplayType.USE:
                DisplayClear();
                foreach (ItemSlotInfo item in _consumableItemList)
                {
                    _uI_Inventory.slotArray[item.index].AddItem(item);
                }
                break;
            case DisplayType.Material:
                DisplayClear();
                foreach (ItemSlotInfo item in _materialItemList)
                {
                    _uI_Inventory.slotArray[item.index].AddItem(item);
                }
                break;
            default:
                DisplayClear();
                foreach (ItemSlotInfo item in _etcItemList)
                {
                    _uI_Inventory.slotArray[item.index].AddItem(item);
                }
                break;
        }
    }

    private void DisplayClear()
    {
        int slotArrayCount = _uI_Inventory.slotArray.Length;
        for (int i = 0; i < slotArrayCount; i++)
        {
            _uI_Inventory.slotArray[i].ClearSlot();
        }

    }
}