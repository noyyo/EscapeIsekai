using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCraftingController : MonoBehaviour
{
    [SerializeField] private int _itemTypeListCount;

    private ItemDB _itemDB;
    private List<int[]> _itemList = new List<int[]>();

    public event Action OnItemCaftingMaterialsEvent;
    public event Action OffItemCaftingMaterialsEvent;

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
    }

    public void CallOnItemCaftingMaterials()
    {
        OnItemCaftingMaterialsEvent?.Invoke();
    }

    public void CallOffItemCaftingMaterials()
    {
        OffItemCaftingMaterialsEvent?.Invoke();
    }

    public void GetRecipe(int id)
    {
        //for(int i = 0; i < _itemList.Count; i++)
        //{

        //}
        //if (!_itemList.Contains(id)) return;
        //else
        //{
        //    if (_itemDB.GetItemData(id, out ItemData_Test newRecipe))
        //    {


        //    }
        //}
        

    }
}
