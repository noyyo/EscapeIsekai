using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class GatheringObject : MonoBehaviour
{
    [SerializeField] private GameObject _descriptionPanel;
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private int _itemId;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _descriptionPanel.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _descriptionPanel.SetActive(false);
        }
    }
    private void Start()
    {
        DataManager._dataInstance.LoadDatas();
        ItemData data = DataManager.GatInstance().dicItemDatas[_itemId];
        //Debug.LogFormat("{0}, {1}", data.name, data.des);

        _itemName.text = data.name;

    }

    public void Gathering()
    {
        //채집버튼 누르면 바로 인벤토리로
        Destroy(gameObject);
    }
    
}
