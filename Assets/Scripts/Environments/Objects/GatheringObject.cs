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

    public void Gathering() //�÷��̾� ��ȣ�ۿ� ��ư �̺�Ʈ ���� �ʿ�
    {
        if( _gathering)
        {
            //ä����ư ������ �ٷ� �κ��丮��
            Destroy(gameObject);
        }
    }
    
}
