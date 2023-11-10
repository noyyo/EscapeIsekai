using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GatheringObject : MonoBehaviour
{
    [SerializeField] private int _itemId;
    private bool _gathering = false;
    private ItemData_Test itemData;

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _gathering = true;

            UI_Manager.Instance.itemName = itemData.ItemName;
            UI_Manager.Instance.itemExplanation = itemData.ItemExplanation;
            UI_Manager.Instance.gatheringCanvas.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            UI_Manager.Instance.gatheringCanvas.SetActive(false);
            _gathering = false;
        }
    }
    private void Start()
    {
        //DataManager.Instance.LoadDatas();
        //ItemData data = DataManager.Instance.dicItemDatas[_itemId];
        ItemDB.Instance.GetItemData(_itemId, out itemData);
    }

    private void OnInteraction() 
    {
        Debug.Log("asdf");
        if (_gathering)
        {
            UI_Manager.Instance.gatheringCanvas.SetActive(false);
            //채집버튼 누르면 바로 인벤토리로
            //InventoryManager.Instance.CallAddItem(_itemId, 1);
            Destroy(gameObject);
        }
    }
    
}
