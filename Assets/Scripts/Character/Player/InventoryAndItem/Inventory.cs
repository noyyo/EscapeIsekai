using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

struct Item
{
    int id;
    int count;
}


public class Inventory : MonoBehaviour
{
    [SerializeField] private int _equipmentSlotCount;
    [SerializeField] private int _consumableSlotCount;
    [SerializeField] private int _etcSlotCount;
    [SerializeField] private int _itemMaxCount;

    private List<Item> _equipmentItems;
    private List<Item> _consumableItems;
    private List<Item> _etcItems;

    private int _equipmentItemsIndexCount;
    private int _consumableItemsIndexCount;
    private int _etcItemsIndexCount;

    private void Awake()
    {
        InitInventory();
    }

    private void InitInventory()
    {
        _equipmentItemsIndexCount = 0;
        _consumableItemsIndexCount = 0;
        _etcItemsIndexCount = 0;

        _equipmentItems = new List<Item>();
        _consumableItems = new List<Item>();
        _etcItems = new List<Item>();
    }

    /// <summary>
    /// �������� ������ �Ѱ��ָ� �ش� �������� ���� ������ Ȯ�� �� ��������� �߰��� �ݴϴ�.
    /// </summary>
    /// <param name="newItem"></param>
    /// <returns></returns>
    public bool AddItem(ItemSO newItem)
    {
        switch (newItem.ItemType)
        {
            case ItemType.Equipment:
                if (_equipmentItemsIndexCount == _equipmentSlotCount) return false;

                break;
            case ItemType.Consumable:
                if (_consumableItemsIndexCount == _consumableSlotCount) return false;
                break;
            case ItemType.ETC:
                if (_etcItemsIndexCount == _etcSlotCount) return false;
                break;
            case ItemType.QuestItem:
                if (_etcItemsIndexCount == _etcSlotCount) return false;
                break;
            default:
                Debug.Log("���ο� ������ Ÿ���Դϴ�. �������ּ���!");
                return false;
        }

        

        return true;
    }
}
