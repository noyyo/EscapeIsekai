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
    /// 아이템의 정보를 넘겨주면 해당 아이템이 들어가는 슬롯은 확인 후 비어있으면 추가해 줍니다.
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
                Debug.Log("새로운 유형의 타입입니다. 설정해주세요!");
                return false;
        }

        

        return true;
    }
}
