using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GatheringObject : MonoBehaviour
{
    [SerializeField] private GameObject _descriptionPanel;
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemDes;
    [SerializeField] private int _itemId;
    private bool _gathering = false;
    private PlayerInput _playerInput;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _descriptionPanel.SetActive(true);
            _gathering = true;
            _playerInput = other.gameObject.GetComponent<PlayerInput>();
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

    private void OnInteraction() //inputAction Ű��� �ʿ�
    {
        if (_gathering)
        {
            //ä����ư ������ �ٷ� �κ��丮��
            Destroy(gameObject);
        }
    }
    
}
