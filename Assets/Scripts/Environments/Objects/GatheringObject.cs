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
    [SerializeField] private TMP_Text _itemDes;
    [SerializeField] private int _itemId;
    private bool _gathering = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _descriptionPanel.SetActive(true);
            _gathering = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _descriptionPanel.SetActive(false);
            _gathering = false;
        }
    }
    private void Start()
    {
        DataManager.Instance.LoadDatas();
        ItemData data = DataManager.Instance.dicItemDatas[_itemId];

        _itemName.text = data.name;
        _itemDes.text = data.des;
    }

    public void Gathering() //플레이어 상호작용 버튼 이벤트 구독 필요
    {
        if( _gathering)
        {
            //채집버튼 누르면 바로 인벤토리로
            Destroy(gameObject);
        }
    }
    
}
