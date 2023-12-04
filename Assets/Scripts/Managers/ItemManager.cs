using UnityEngine;

public class ItemManager : CustomSingleton<ItemManager>
{
    private ItemDB itemDB;

    private void Awake()
    {
        itemDB = ItemDB.Instance;
    }

    public Item InstantiateItemObject(int id, int count)
    {
        itemDB.GetItemData(id, out ItemData itemData);
        itemDB.GetStats(id, out ItemStats itemStats);
        return new Item(itemData, itemStats, count);
    }

    public void Drop(Item item, int count, Vector3 pos)
    {
        GameObject go = Instantiate(item.DropPrefab, pos, Quaternion.identity);
        go.AddComponent<ItemObject>().SetItem(new Item(item, count));
    }
}
