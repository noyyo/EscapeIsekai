using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : CustomSingleton<ItemManager>
{
    private ItemDB itemDB;

    private void Awake()
    {
        itemDB = ItemDB.Instance;
    }

    public ItemObject InstantiateItemObject(int id, int count)
    {
        itemDB.GetItemData(id, out ItemData_Test itemData);
        itemDB.GetStats(id, out ItemStats itemStats);
        return new ItemObject(itemData, itemStats, count);
    }

    public void Drop(ItemObject itemObject, int count, Vector3 pos)
    {
        GameObject go = Instantiate(itemObject.DropPrefab, pos, Quaternion.identity);
        if (go.TryGetComponent<ItemObject>(out itemObject))
            itemObject.GetData(itemObject, count);
        else
            Debug.LogError("Drop Prefabs에 itemObject가 없습니다.");
    }
}
