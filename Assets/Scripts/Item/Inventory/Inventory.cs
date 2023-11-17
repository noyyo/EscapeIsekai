﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSlotInfo
{
    public int id;
    public int count;
    public int index;
    public bool equip;

    public ItemSlotInfo(int newID, int newindex, int newCount = 1)
    {
        id = newID;
        count = newCount;
        index = newindex;
    }
    public ItemSlotInfo() { }
}

public class Inventory : MonoBehaviour
{   
    [SerializeField] private int _inventroySlotCount = 60;
    private GameObject _slotPrefab;
    private GameObject _slotSpawn;
    private InventoryManager _inventoryManager;
    private UI_Manager _ui_Manager;
    private List<Slot> _slotArray;
    private List<ItemSlotInfo>[] _ItemList;
    private ItemDB _itemDB;
    private ItemType _displayType;
    private ItemSlotInfo _clickItem;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _ui_Manager = UI_Manager.Instance;
        _itemDB = ItemDB.Instance;
        _inventoryManager = InventoryManager.Instance;
        _slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/Slot");
    }

    private void Start()
    {
        InitInventory();
        CreateSlot();
        DisplaySlotAllClear();
        _ui_Manager.UI_InventoryTurnOnEvent += OnDisplaySlot;
    }

    private void InitInventory()
    {
        _slotArray = _inventoryManager.SlotList;
        _ItemList = _inventoryManager.ItemList;
        _displayType = ItemType.Equipment;
        _slotSpawn = _ui_Manager.Inventory_UI.transform.GetChild(4).GetChild(0).GetChild(0).gameObject;
    }

    private void CreateSlot()
    {
        for (int i = 0; i < _inventroySlotCount; i++)
        {
            GameObject obj = Instantiate(_slotPrefab);
            obj.transform.SetParent(_slotSpawn.transform, false);
            obj.GetComponent<Slot>().UniqueIndex = i;
            _slotArray.Add(obj.GetComponent<Slot>());
        }
    }

    public bool[] TryAddItems(int[] id, int[] count, out int[] errorItemCount)
    {
        int idCount = id.Length;
        bool[] boolArray = new bool[idCount];
        errorItemCount = new int[idCount];
        for (int i = 0; i < idCount; i++)
        {
            boolArray[i] = TryAddItem(id[i], count[i], out errorItemCount[i]);
        }
        return boolArray;
    }

    public bool[] TryAddItems(ItemRecipe itemRecipe, out int[] errorItemCounts)
    {
        int idCount = itemRecipe.Materials.Length;
        bool[] boolArray = new bool[idCount];
        bool isCraftingItem = true;
        errorItemCounts = new int[idCount];
        for (int i = 0; i < idCount; i++)
        {
            boolArray[i] = IsCheckItem(itemRecipe.Materials[i], itemRecipe.MaterialsCount[i], out int sum);
            if (!boolArray[i])
                isCraftingItem = false;
        }
        if (isCraftingItem)
        {
            for (int i = 0; i < idCount; i++)
                TryAddItem(itemRecipe.Materials[i], -(itemRecipe.MaterialsCount[i]), out errorItemCounts[i]);
            TryAddItem(itemRecipe.CraftingID, 1, out int errorItemCount);
        }

        return boolArray;
    }

    public bool TryAddItem(int id, int count, out int errorItemCount)
    {
        errorItemCount = count;
        bool isAddItem = false;
        if (_itemDB.GetItemData(id, out ItemData_Test newItem))
        {
            switch (id)
            {
                case >= 10300000:
                    
                    break;
                case >= 10200000:
                    if (count >= 0)
                        if (AddList(_ItemList[2], count, ItemType.Material, in newItem, out errorItemCount))
                            isAddItem = true;
                    else
                        if (SubList(_ItemList[2], count, ItemType.Material, in newItem, out errorItemCount))
                            isAddItem = true;
                    break;
                case >= 10100000:
                    if (count >= 0)
                        if (AddList(_ItemList[1], count, ItemType.Consumable, in newItem, out errorItemCount))
                            isAddItem = true;
                    else
                        if (SubList(_ItemList[1], count, ItemType.Consumable, in newItem, out errorItemCount))
                            isAddItem = true;
                    break;
                default:
                    if (count >= 0)
                        if (AddList(_ItemList[0], count, ItemType.Equipment, in newItem, out errorItemCount))
                            isAddItem = true;
                    else
                        if (SubList(_ItemList[0], count, ItemType.Equipment, in newItem, out errorItemCount))
                            isAddItem = true;
                    break;
            }
        }
        return isAddItem;
    }

    private bool AddList(List<ItemSlotInfo> itemList, int count, ItemType slotType, in ItemData_Test newItem, out int errorItemCount)
    {
        int itemListCount = itemList.Count;
        if (itemListCount == _inventroySlotCount)
        {
            errorItemCount = count;
            return false;
        }

        errorItemCount = 0;
        int id = newItem.ID;
        bool isDisplay = false;

        if (slotType == _displayType)
            isDisplay = true;

        if (slotType != ItemType.Equipment)
        {
            if (itemListCount == 0)
            {
                if (count > newItem.MaxCount)
                {
                    while (true)
                    {
                        count -= newItem.MaxCount;
                        if (count <= 0)
                        {
                            if (isDisplay)
                                _slotArray[itemListCount].AddItem(newItem, itemListCount, newItem.MaxCount + count);
                            itemList.Add(new ItemSlotInfo(id, itemListCount, newItem.MaxCount + count));
                            itemListCount++;
                            break;
                        }
                        else
                        {
                            if (isDisplay)
                                _slotArray[itemListCount].AddItem(newItem, itemListCount, newItem.MaxCount);
                            itemList.Add(new ItemSlotInfo(id, itemListCount, newItem.MaxCount));
                            itemListCount++;
                        }
                    }
                }
                else
                {
                    if (isDisplay)
                        _slotArray[0].AddItem(newItem, 0, count);
                    itemList.Add(new ItemSlotInfo(id, 0, count));
                    itemListCount++;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < itemListCount; i++)
                {
                    if (itemList[i].id == id)
                    {
                        int newItemMaxCount = newItem.MaxCount;
                        int nowItemCount = itemList[i].count;
                        if (nowItemCount == newItemMaxCount)
                        {
                            if (nowItemCount + count > newItemMaxCount)
                            {
                                count = count - (newItemMaxCount - nowItemCount);
                                itemList[i].count = newItemMaxCount;
                                if (isDisplay)
                                    _slotArray[i].SetSlotCount(newItemMaxCount);
                                while (true)
                                {
                                    count -= newItemMaxCount;
                                    if (count <= 0) break;
                                    if (isDisplay)
                                        _slotArray[itemListCount].AddItem(newItem, itemListCount, newItemMaxCount);
                                    itemList.Add(new ItemSlotInfo(id, itemListCount, newItemMaxCount));
                                    itemListCount++;
                                    if (itemListCount == _inventroySlotCount)
                                    {
                                        errorItemCount = count;
                                        return false;
                                    }
                                }
                                itemList.Add(new ItemSlotInfo(id, itemListCount, newItemMaxCount + count));
                                itemListCount++;
                                if (isDisplay)
                                    _slotArray[itemListCount - 1].AddItem(newItem, itemListCount - 1, newItemMaxCount + count);
                            }
                            else
                            {
                                itemList[i].count += count;
                                if (isDisplay)
                                    _slotArray[i].SetSlotCount(itemList[i].count);
                            }
                        }
                    }
                }
                return true;
            }
        }
        for (int i = 0; i < count; i++)
        {
            itemList.Add(new ItemSlotInfo(id, itemListCount, 1));
            itemListCount++;
            if (isDisplay)
                _slotArray[(itemListCount - 1)].AddItem(newItem, (itemListCount - 1), 1);
            if (itemListCount == _inventroySlotCount)
            {
                errorItemCount = count - (i + 1);
                return false;
            }
        }
        return true;
    }

    private bool SubList(List<ItemSlotInfo> itemList, int count, ItemType slotType, in ItemData_Test newItem, out int ErrorItemCount)
    {
        int itemListCount = itemList.Count;
        if (itemListCount == 0)
        {
            ErrorItemCount = count;
            return false;
        }

        bool isSub = false;
        int id = newItem.ID;
        Stack stack = new Stack();

        bool isDisplay = false;

        if (slotType == _displayType)
            isDisplay = true;

        for (int i = 0; i < itemListCount; i++)
        {
            if (itemList[i].id == id)
            {
                int nowItemCount = itemList[i].count + count;
                if (nowItemCount <= 0)
                {
                    count = nowItemCount;
                    stack.Push(i);
                    if(nowItemCount != 0)
                        continue;
                }
                count = 0;
                if (isDisplay)
                    _slotArray[i].SetSlotCount(nowItemCount);
                for (int j = 0; j < stack.Count;)
                {
                    int n = (int)stack.Pop();
                    _slotArray[n].ClearSlot();
                    itemList.RemoveAt(n);
                    itemListCount--;
                }
                isSub = true;
                break;
            }
        }
        ErrorItemCount = count;
        return isSub;
    }

    public void SortInventory()
    {
        DisplaySlotAllClear();
        for(int i = 0; i < _ItemList.Length; i++)
        {
            _ItemList[i].Sort((n1, n2) => n1.id.CompareTo(n2.id));
            int maxIndex = _ItemList[i].Count;
            for (int j = 0; j < maxIndex; j++)
                _ItemList[i][j].index = j;
        }
        OnDisplaySlot();
    }
    public void SetDisplayType(ItemType itemType)
    {
        DisplaySlotClear();
        _displayType = itemType;
        OnDisplaySlot();
    }
    private void OnDisplaySlot()
    {
        switch (_displayType)
        {
            case ItemType.Equipment:
                foreach (ItemSlotInfo item in _ItemList[0])
                {
                    _slotArray[item.index].AddItem(item);
                }
                break;
            case ItemType.Consumable:
                foreach (ItemSlotInfo item in _ItemList[1])
                {
                    _slotArray[item.index].AddItem(item);
                }
                break;
            case ItemType.Material:
                foreach (ItemSlotInfo item in _ItemList[2])
                {
                    _slotArray[item.index].AddItem(item);
                }
                break;
            default:
                foreach (ItemSlotInfo item in _ItemList[3])
                {
                    _slotArray[item.index].AddItem(item);
                }
                break;
        }
    }
    private void DisplaySlotAllClear()
    {
        for (int i = 0; i < _inventroySlotCount; i++)
        {
            _slotArray[i].ClearSlot();
        }

    }
    private void DisplaySlotClear()
    {
        switch (_displayType)
        {
            case ItemType.Equipment:
                foreach (ItemSlotInfo item in _ItemList[0])
                    _slotArray[item.index].ClearSlot();
                break;
            case ItemType.Consumable:
                foreach (ItemSlotInfo item in _ItemList[1])
                    _slotArray[item.index].ClearSlot();
                break;
            case ItemType.Material:
                foreach (ItemSlotInfo item in _ItemList[2])
                    _slotArray[item.index].ClearSlot();
                break;
            default:
                foreach (ItemSlotInfo item in _ItemList[3])
                    _slotArray[item.index].ClearSlot();
                break;
        }
    }
    public void UseItem()
    {
        _clickItem = _inventoryManager.ClickItem;
        if (_displayType == ItemType.Equipment)
            EquipItem(_clickItem);
        else
            TryAddItem(_clickItem.id, -1, out int errorCount);
    }

    public bool IsCheckItem(int id, int count, out int itemSum)
    {
        bool isItemCount = false;
        itemSum = 0;
        if (_itemDB.GetItemData(id, out ItemData_Test newItem))
        {
            switch (id)
            {
                case >= 10300000:
                    isItemCount = IsCheckItemCount(_ItemList[3], newItem, count, ref itemSum);
                    break;
                case >= 10200000:
                    isItemCount = IsCheckItemCount(_ItemList[2], newItem, count, ref itemSum);
                    break;
                case >= 10100000:
                    isItemCount = IsCheckItemCount(_ItemList[1], newItem, count, ref itemSum);
                    break;
                default:
                    isItemCount = IsCheckItemCount(_ItemList[0], newItem, count, ref itemSum);
                    break;
            }
        }
        return isItemCount;
    }
    public bool[] IsCheckItems(int[] id, int[] count, out int[] itemSum)
    {
        bool isItemCount = false;
        int idLenght = id.Length;
        itemSum = new int[idLenght];
        bool[] boolArray = new bool[idLenght];

        for (int i  = 0; i < idLenght; i++)
        {
            if (_itemDB.GetItemData(id[i], out ItemData_Test newItem))
            {
                switch (id[i])
                {
                    case >= 10300000:
                        isItemCount = IsCheckItemCount(_ItemList[3], newItem, count[i], ref itemSum[i]);
                        break;
                    case >= 10200000:
                        isItemCount = IsCheckItemCount(_ItemList[2], newItem, count[i], ref itemSum[i]);
                        break;
                    case >= 10100000:
                        isItemCount = IsCheckItemCount(_ItemList[1], newItem, count[i], ref itemSum[i]);
                        break;
                    default:
                        isItemCount = IsCheckItemCount(_ItemList[0], newItem, count[i], ref itemSum[i]);
                        break;
                }
            }
            boolArray[i] = isItemCount;
        }
        return boolArray;
    }

    public Sprite[] IsCheckItems(in ItemRecipe newRecipe, out int[] itemSum)
    {
        int idLenght = newRecipe.Materials.Length;
        int[] materials = newRecipe.Materials;
        int[] materialsCount = newRecipe.MaterialsCount;
        itemSum = new int[idLenght];
        Sprite[] icons = new Sprite[idLenght + 1];
        _itemDB.GetImage(newRecipe.CraftingID, out icons[0]);
        for (int i = 0; i < idLenght; i++)
        {
            if (_itemDB.GetItemData(materials[i], out ItemData_Test newItem))
            {
                switch (materials[i])
                {
                    case >= 10300000:
                        IsCheckItemCount(_ItemList[3], newItem, materialsCount[i], ref itemSum[i]);
                        break;
                    case >= 10200000:
                        IsCheckItemCount(_ItemList[2], newItem, materialsCount[i], ref itemSum[i]);
                        break;
                    case >= 10100000:
                        IsCheckItemCount(_ItemList[1], newItem, materialsCount[i], ref itemSum[i]);
                        break;
                    default:
                        IsCheckItemCount(_ItemList[0], newItem, materialsCount[i], ref itemSum[i]);
                        break;
                }
                icons[i+1] = newItem.Icon;
            }
        }
        return icons;
    }

    private bool IsCheckItemCount(List<ItemSlotInfo> itemList, ItemData_Test newItem, int count, ref int sum)
    {
        int listCount = itemList.Count;
        sum = 0;
        for (int i = 0; i < listCount; i++)
        {
            if (itemList[i].id == newItem.ID)
            {
                sum += itemList[i].count;
            }
        }
        return sum >= count ? true : false;
    }

    public bool TryAddItem(int id, int count)
    {
        bool isAddItem = false;
        if (_itemDB.GetItemData(id, out ItemData_Test newItem))
        {
            switch (id)
            {
                case >= 10300000:
                    if (count >= 0)
                    {
                        if (AddList(_ItemList[3], count, ItemType.ETC, in newItem))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_ItemList[3], count, ItemType.ETC, in newItem))
                            isAddItem = true;
                    }
                    break;
                case >= 10200000:
                    if (count >= 0)
                    {
                        if (AddList(_ItemList[2], count, ItemType.Material, in newItem))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_ItemList[2], count, ItemType.Material, in newItem))
                            isAddItem = true;
                    }
                    break;
                case >= 10100000:
                    if (count >= 0)
                    {
                        if (AddList(_ItemList[1], count, ItemType.Consumable, in newItem))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_ItemList[1], count, ItemType.Consumable, in newItem))
                            isAddItem = true;
                    }
                    break;
                default:
                    if (count >= 0)
                    {
                        if (AddList(_ItemList[0], count, ItemType.Equipment, in newItem))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_ItemList[0], count, ItemType.Equipment, in newItem))
                            isAddItem = true;
                    }
                    break;
            }
        }
        return isAddItem;
    }

    public bool[] TryAddItems(int[] id, int[] count)
    {
        int idCount = id.Length;
        bool[] boolArray = new bool[idCount];
        for (int i = 0; i < idCount; i++)
        {
            boolArray[i] = TryAddItem(id[i], count[i]);
        }
        return boolArray;
    }

    public bool[] TryAddItems(ItemRecipe itemRecipe)
    {
        int idCount = itemRecipe.Materials.Length;
        bool[] boolArray = new bool[idCount];
        bool isCraftingItem = true;
        for (int i = 0; i < idCount; i++)
        {
            boolArray[i] = IsCheckItem(itemRecipe.Materials[i], itemRecipe.MaterialsCount[i]);
            if (!boolArray[i])
            {
                isCraftingItem = false;
            }
        }
        if (isCraftingItem)
        {
            for (int i = 0; i < idCount; i++)
            {
                TryAddItem(itemRecipe.Materials[i], -(itemRecipe.MaterialsCount[i]));
            }
            TryAddItem(itemRecipe.CraftingID, 1);
        }

        return boolArray;
    }

    private bool AddList(List<ItemSlotInfo> itemList, int count, ItemType slotType, in ItemData_Test newItem)
    {
        int itemListCount = itemList.Count;
        if (itemListCount == _inventroySlotCount)
            return false;

        int id = newItem.ID;
        bool isDisplay = false;

        if (slotType == _displayType)
            isDisplay = true;

        if (slotType != ItemType.Equipment)
        {
            if (itemListCount == 0)
            {
                if (count > newItem.MaxCount)
                {
                    while (true)
                    {
                        count -= newItem.MaxCount;
                        if (count <= 0)
                        {
                            if (isDisplay)
                                _slotArray[itemListCount].AddItem(newItem, itemListCount, newItem.MaxCount + count);
                            itemList.Add(new ItemSlotInfo(id, itemListCount, newItem.MaxCount + count));
                            itemListCount++;
                            break;
                        }
                        else
                        {
                            if (isDisplay)
                                _slotArray[itemListCount].AddItem(newItem, itemListCount, newItem.MaxCount);
                            itemList.Add(new ItemSlotInfo(id, itemListCount, newItem.MaxCount));
                            itemListCount++;
                        }
                    }
                }
                else
                {
                    if (isDisplay)
                        _slotArray[0].AddItem(newItem, 0, count);
                    itemList.Add(new ItemSlotInfo(id, 0, count));
                    itemListCount++;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < itemListCount; i++)
                {
                    if (itemList[i].id == id)
                    {
                        int newItemMaxCount = newItem.MaxCount;
                        int nowItemCount = itemList[i].count;
                        if (nowItemCount == newItemMaxCount)
                        {
                            if (nowItemCount + count > newItemMaxCount)
                            {
                                count = count - (newItemMaxCount - nowItemCount);
                                itemList[i].count = newItemMaxCount;
                                if (isDisplay)
                                    _slotArray[i].SetSlotCount(newItemMaxCount);
                                while (true)
                                {
                                    count -= newItemMaxCount;
                                    if (count <= 0) break;
                                    if (isDisplay)
                                        _slotArray[itemListCount].AddItem(newItem, itemListCount, newItemMaxCount);
                                    itemList.Add(new ItemSlotInfo(id, itemListCount, newItemMaxCount));
                                    itemListCount++;
                                    if (itemListCount == _inventroySlotCount)
                                        return false;
                                }
                                itemList.Add(new ItemSlotInfo(id, itemListCount, newItemMaxCount + count));
                                itemListCount++;
                                if (isDisplay)
                                    _slotArray[itemListCount - 1].AddItem(newItem, itemListCount - 1, newItemMaxCount + count);
                            }
                            else
                            {
                                itemList[i].count += count;
                                if (isDisplay)
                                    _slotArray[i].SetSlotCount(itemList[i].count);
                            }
                        }
                    }
                }
                return true;
            }
        }
        for (int i = 0; i < count; i++)
        {
            itemList.Add(new ItemSlotInfo(id, itemListCount, 1));
            itemListCount++;
            if (isDisplay)
                _slotArray[(itemListCount - 1)].AddItem(newItem, (itemListCount - 1), 1);
            if (itemListCount == _inventroySlotCount)
                return false;
        }
        return true;
    }

    private bool SubList(List<ItemSlotInfo> itemList, int count, ItemType slotType, in ItemData_Test newItem)
    {
        int itemListCount = itemList.Count;
        if (itemListCount == 0)
            return false;

        bool isSub = false;
        int id = newItem.ID;
        Stack stack = new Stack();

        bool isDisplay = false;

        if (slotType == _displayType)
            isDisplay = true;

        for (int i = 0; i < itemListCount; i++)
        {
            if (itemList[i].id == id)
            {
                int nowItemCount = itemList[i].count + count;
                if (nowItemCount <= 0)
                {
                    count = nowItemCount;
                    stack.Push(i);
                    if (nowItemCount != 0)
                        continue;
                }
                count = 0;
                if (isDisplay)
                    _slotArray[i].SetSlotCount(nowItemCount);
                for (int j = 0; j < stack.Count;)
                {
                    int n = (int)stack.Pop();
                    _slotArray[n].ClearSlot();
                    itemList.RemoveAt(n);
                    itemListCount--;
                }
                isSub = true;
                break;
            }
        }
        return isSub;
    }

    public bool IsCheckItem(int id, int count)
    {
        bool isItemCount = false;
        if (_itemDB.GetItemData(id, out ItemData_Test newItem))
        {
            switch (id)
            {
                case >= 10300000:
                    isItemCount = IsCheckItemCount(_ItemList[3], newItem, count);
                    break;
                case >= 10200000:
                    isItemCount = IsCheckItemCount(_ItemList[2], newItem, count);
                    break;
                case >= 10100000:
                    isItemCount = IsCheckItemCount(_ItemList[1], newItem, count);
                    break;
                default:
                    isItemCount = IsCheckItemCount(_ItemList[0], newItem, count);
                    break;
            }
        }
        return isItemCount;
    }

    public bool[] IsCheckItems(int[] id, int[] count)
    {
        bool isItemCount = false;
        int idLenght = id.Length;
        bool[] boolArray = new bool[idLenght];

        for (int i = 0; i < idLenght; i++)
        {
            if (_itemDB.GetItemData(id[i], out ItemData_Test newItem))
            {
                switch (id[i])
                {
                    case >= 10300000:
                        isItemCount = IsCheckItemCount(_ItemList[3], newItem, count[i]);
                        break;
                    case >= 10200000:
                        isItemCount = IsCheckItemCount(_ItemList[2], newItem, count[i]);
                        break;
                    case >= 10100000:
                        isItemCount = IsCheckItemCount(_ItemList[1], newItem, count[i]);
                        break;
                    default:
                        isItemCount = IsCheckItemCount(_ItemList[0], newItem, count[i]);
                        break;
                }
            }
            boolArray[i] = isItemCount;
        }
        return boolArray;
    }

    public Sprite[] IsCheckItems(in ItemRecipe newRecipe)
    {
        int idLenght = newRecipe.Materials.Length;
        int[] materials = newRecipe.Materials;
        int[] materialsCount = newRecipe.MaterialsCount;
        Sprite[] icons = new Sprite[idLenght + 1];
        _itemDB.GetImage(newRecipe.CraftingID, out icons[0]);
        for (int i = 0; i < idLenght; i++)
        {
            if (_itemDB.GetItemData(materials[i], out ItemData_Test newItem))
            {
                switch (materials[i])
                {
                    case >= 10300000:
                        IsCheckItemCount(_ItemList[3], newItem, materialsCount[i]);
                        break;
                    case >= 10200000:
                        IsCheckItemCount(_ItemList[2], newItem, materialsCount[i]);
                        break;
                    case >= 10100000:
                        IsCheckItemCount(_ItemList[1], newItem, materialsCount[i]);
                        break;
                    default:
                        IsCheckItemCount(_ItemList[0], newItem, materialsCount[i]);
                        break;
                }
                icons[i + 1] = newItem.Icon;
            }
        }
        return icons;
    }

    private bool IsCheckItemCount(List<ItemSlotInfo> itemList, ItemData_Test newItem, int count)
    {
        int listCount = itemList.Count;
        int sum = 0;
        for (int i = 0; i < listCount; i++)
        {
            if (itemList[i].id == newItem.ID)
            {
                sum += itemList[i].count;
            }
        }
        return sum >= count ? true : false;
    }

    private void EquipItem(ItemSlotInfo itemSlotInfo)
    {
        if (!itemSlotInfo.equip)
        {
            _itemDB.GetStats(itemSlotInfo.id, out ItemStats equipItemData);

            Debug.Log("장비를 장착합니다..");

            itemSlotInfo.equip = true;

            _slotArray[itemSlotInfo.index].DisplayEquip();
            _inventoryManager.CalOnTextChangeUnEquipEvent();
            
        }
        else
        {
            _itemDB.GetStats(itemSlotInfo.id, out ItemStats equipItemData);

            Debug.Log("장비를 해제합니다.");

            itemSlotInfo.equip = false;
            _slotArray[itemSlotInfo.index].UnDisplayEquip();
            _inventoryManager.CallOnTextChangeEquipEvent();
        }
        
    }
    public void Drop()
    {
        _clickItem = _inventoryManager.ClickItem;

        _itemDB.GetItemData(_clickItem.id, out ItemData_Test newItem);
        Instantiate(newItem.DropPrefab);

        TryAddItem(_clickItem.id, -1, out int errorCount);
    }

    public void ChangeSlot(ItemSlotInfo newSlot, ItemSlotInfo oldSlot, int newIndex, int oldIndex)
    {
        if (newSlot == null)
        {
            oldSlot.index = newIndex;
            _slotArray[newIndex].AddItem(oldSlot);
            _slotArray[oldIndex].ClearSlot();
            return;
        }

        newSlot.index = oldIndex;
        oldSlot.index = newIndex;

        _slotArray[newIndex].AddItem(oldSlot);
        _slotArray[oldIndex].AddItem(newSlot);
    }
}