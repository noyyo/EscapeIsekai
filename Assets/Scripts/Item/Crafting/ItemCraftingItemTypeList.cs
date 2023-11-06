using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingItemTypeList : MonoBehaviour
{
    [SerializeField] private TMP_Text _listName;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private Button _button;
    private bool _isDisplay;
    private List<ItemCraftingSlot> _slotList = new List<ItemCraftingSlot>();

    public event Action slotActiveEvent;

    private void Awake()
    {
        if (_button == null)
        {
            _button = GetComponentInChildren<Button>();
        }
        _isDisplay = false;
    }

    private void Start()
    {
        _button.onClick.AddListener(OnClickButton);
    }
    
    public void AddSlot(ItemCraftingSlot newSlot)
    {
        _slotList.Add(newSlot);
        slotActiveEvent += newSlot.TurnOnOffSlot;
    }

    private void OnClickButton()
    {
        _isDisplay = !_isDisplay;
        if (_isDisplay)
        {
            _arrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
            _arrow.transform.rotation = Quaternion.identity;
        }
        slotActiveEvent?.Invoke();
    }
}
