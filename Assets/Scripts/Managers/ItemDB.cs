using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDB : CustomSingleton<ItemDB>
{
    //[SerializeField] private ItemExcel _itemExcel;
    [SerializeField] private bool _test;
    private ItemData_Test[] itemDatas;
    private int ItemDatasCount;
    [SerializeField] private Inventory _inventory;

    public ItemData_Test[] ItemDatas { get { return itemDatas; } }

    private void Start()
    {
        if( _test)
        {
            itemDatas = new ItemData_Test[2];
            itemDatas[0] = new ItemData_Test(0, "Test1", "Test1111", ItemType.Equipment, 10, 1, "Prefabs/Entities/DropItem/0", "Sprite/Icon/0");
            itemDatas[1] = new ItemData_Test(1, "Test2", "Test2222", ItemType.Consumable, 10, 99, "Prefabs/Entities/DropItem/1", "Sprite/Icon/1");
            ItemDatasCount = itemDatas.Length;

            _inventory.AddItem(0, 2, out int i);
            _inventory.AddItem(1, 22, out i);
        }
        
    }

    public bool GetItemData(int id, out ItemData_Test itemData)
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
