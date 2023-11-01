using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : CustomSingleton<UI_Manager>
{
    protected UI_Manager () { }

    private ItemDB _itemDB;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        _inventoryManager = InventoryManager.Instance;
        Debug.Log(_itemDB.ToString());
        Debug.Log(_inventoryManager.ToString());
    }
}
