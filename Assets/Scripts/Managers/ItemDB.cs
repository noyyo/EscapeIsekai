using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : CustomSingleton<ItemDB>
{
    [SerializeField] private ItemExcel _itemExcel;

    public bool GetItemData(int id, out ItemData itemData)
    {
        if(id > _itemExcel.ItemDatas.Count)
        {
            Debug.Log("Error 아이템의 id값을 확인해 주세요.");
            itemData = null;
            return false;
        }
        else
        {
            itemData = _itemExcel.ItemDatas[id];
            return true;
        }
    }
}
