using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingItemTypeList : MonoBehaviour
{
    public TMP_Text listName;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _craftingItemSlotSpawn;
    private bool _isDisplay;
    private List<ItemCraftingSlot> _slotList = new List<ItemCraftingSlot>();
    private ItemCraftingManager _craftingManager;
    private GameObject _prefabs;
    private int _slotListLength = -1;

    public event Action slotActiveEvent;

    private void Awake()
    {
        if (_button == null)
        {
            _button = GetComponentInChildren<Button>();
        }
        _isDisplay = false;
        _craftingManager = ItemCraftingManager.Instance;
        _prefabs = _craftingManager.CraftingSlotPrefab;

        if(_craftingItemSlotSpawn == null)
        {
            _craftingItemSlotSpawn = this.gameObject;
        }
    }

    private void Start()
    {
        _button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        //_isDisplay = !_isDisplay;
        //if (_isDisplay)
        //{
        //    _arrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        //}
        //else
        //{
        //    _arrow.transform.rotation = Quaternion.identity;
        //}
        //slotActiveEvent?.Invoke();
    }

    public void AddRecipe(in ItemRecipe newRecipe)
    {
        CreateItemCraftingSlot();
        _slotList[_slotListLength].SetSlot(newRecipe);
    }

    private void CreateItemCraftingSlot()
    {
        ItemCraftingSlot newSlot = Instantiate(_prefabs, this.transform).GetComponent<ItemCraftingSlot>();
        slotActiveEvent += newSlot.TurnOnOffSlot;
        _slotList.Add(newSlot);
        _slotListLength++;
    }
}
