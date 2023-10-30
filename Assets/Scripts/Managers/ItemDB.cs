using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDB : CustomSingleton<ItemDB>
{
    //[SerializeField] private ItemExcel _itemExcel;
    [SerializeField] private bool _test;
    private ItemData[] itemDatas;
    private int ItemDatasCount;
    [SerializeField] private Inventory _inventory;

    public ItemData[] ItemDatas { get { return itemDatas; } }

    private void Start()
    {
        if( _test)
        {
            itemDatas = new ItemData[2];
            itemDatas[0] = new ItemData(0, "Test1", "Test1111", ItemType.Equipment, 10, 1, "Prefabs/Entities/DropItem/0", "Sprite/Icon/0");
            itemDatas[1] = new ItemData(1, "Test2", "Test2222", ItemType.Consumable, 10, 99, "Prefabs/Entities/DropItem/1", "Sprite/Icon/1");
            ItemDatasCount = itemDatas.Length;

            _inventory.AddItem(0, 2, out int i);
            Debug.Log(i);
            _inventory.AddItem(1, 22, out i);
            Debug.Log(i);
        }
        
    }
    private void Update()
    {
        if (_test)
        {
            _test = false;
            _inventory.OnDisplaySlot();
        }
    }

    public bool GetItemData(int id, out ItemData itemData)
    {
        if (id > ItemDatasCount)
        {
            Debug.Log("Error");
            itemData = null;
            return false;
        }
        else
        {
            itemData = ItemDatas[id];
            return true;
        }
    }
}
