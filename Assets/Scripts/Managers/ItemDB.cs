using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : CustomSingleton<ItemDB>
{
    //[SerializeField] private ItemExcel _itemExcel;
    [SerializeField] private bool _test;
    private ItemData[] itemDatas;

    public ItemData[] ItemDatas { get { return itemDatas; } }

    private void Start()
    {
        if( _test)
        {
            itemDatas = new ItemData[10];
            itemDatas[0] = new ItemData(0, "Test1", "Test1111", ItemType.Equipment, 10, 1, "Prefabs/Entities/DropItem/0", "Sprite/Icon/0");
            itemDatas[1] = new ItemData(1, "Test2", "Test2222", ItemType.Consumable, 10, 99, "Prefabs/Entities/DropItem/1", "Sprite/Icon/1");
        }
    }





    //public bool GetItemData(int id, out ItemData itemData)
    //{
    //    if (id > _itemExcel.ItemDatas.Count)
    //    {
    //        Debug.Log("Error");
    //        itemData = null;
    //        return false;
    //    }
    //    else
    //    {
    //        itemData = _itemExcel.ItemDatas[id];
    //        return true;
    //    }
    //}
}
