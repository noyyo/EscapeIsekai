using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GatheringObject : MonoBehaviour
{
    public int _itemId;
    [HideInInspector]
    private bool _gathering = false;
    private ItemData_Test itemData;
    private Player _playerInputSystem;
    private UI_Manager _UI_Manager;

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TagsAndLayers.PlayerTag)
        {
            _gathering = true;
            if(_playerInputSystem == null)
            {
                _playerInputSystem = other.GetComponent<Player>();

            }
            _playerInputSystem.Input.PlayerActions.Interaction.started += Gathering;

            _UI_Manager.itemName = itemData.ItemName;
            _UI_Manager.itemExplanation = itemData.ItemExplanation; 
            _UI_Manager.gathering.SetActive(true);
            _UI_Manager.UI_gathering.Setting();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == TagsAndLayers.PlayerTag)
        {
            _playerInputSystem.Input.PlayerActions.Interaction.started -= Gathering;
            UI_Manager.Instance.gathering.SetActive(false);
            _gathering = false;
        }
    }
    private void Start()
    {
        _UI_Manager = UI_Manager.Instance;
        ItemDB.Instance.GetItemData(_itemId, out itemData);
    }

    private void Gathering(InputAction.CallbackContext context)
    {
        if (_gathering)
        {
            _UI_Manager.gathering.SetActive(false);
            InventoryManager.Instance.CallAddItem(_itemId, 1);
            _playerInputSystem.Input.PlayerActions.Interaction.started -= Gathering;
            Destroy(gameObject);
            _gathering=false;
        }
    }
}
