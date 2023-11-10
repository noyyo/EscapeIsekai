﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemAvailability;
    [SerializeField] private GameObject _outLine;
    [SerializeField] private Image _icon;

    //슬롯 데이터 저장
    private ItemData_Test _itemData;
    private Inventory _inventory;
    private int[] _materialsCount;
    private bool[] _boolArray;
    private ItemCraftingManager _craftingManager;

    private Button _button;
    private bool isDisplay = false;
    private bool _isMake = true;
    private ItemDB _itemDB;

    private ItemRecipe itemRecipe;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);
        _inventory = InventoryManager.Instance.Inventory;
        _craftingManager = ItemCraftingManager.Instance;
        _itemDB = ItemDB.Instance;
    }

    private void Start()
    {
        _craftingManager.onClickCraftingButtonEvent += MakeCheck;
    }

    //목차버튼 누르면 슬롯을 활성화하기 위한 메서드
    public void TurnOnOffSlot()
    {
        isDisplay = !isDisplay;
        this.gameObject.SetActive(isDisplay);
    }

    //슬롯의 정보를 저장하기 위한 메서드
    public void SetSlot(in ItemRecipe newRecipe)
    {
        if(_itemDB == null)
            _itemDB = ItemDB.Instance;
        _itemDB.GetItemData(newRecipe.CraftingID, out ItemData_Test newItemData);
        _itemData = newItemData;
        itemRecipe = newRecipe;
        _icon.sprite = newItemData.Icon;
        _itemName.text = newItemData.ItemName;
        MakeCheck(newRecipe);
    }

    //버튼을 누르면 실행
    public void OnButtonClick()
    {
        MakeCheck(itemRecipe);
        _craftingManager.CallOnClickCraftingSlotEvent(itemRecipe, _isMake);
        _outLine.SetActive(true);
        _craftingManager.offOutLineEvent += OutLineTrunOff;
    }

    //자동해제를 위한 메서드
    public void OutLineTrunOff()
    {
        _outLine.SetActive(false);
    }

    public void MakeCheck(ItemRecipe newRecipe)
    {
        if (_inventory == null)
            _inventory = InventoryManager.Instance.Inventory;
        _boolArray = _inventory.IsCheckItems(newRecipe.Materials, newRecipe.MaterialsCount, out _materialsCount);

        _isMake = true;
        foreach (bool i in _boolArray)
        {
            if(i == false)
                _isMake = false;
        }
        UpdateIsMakeTextUI();
    }

    public void MakeCheck()
    {
        _boolArray = _inventory.IsCheckItems(itemRecipe.Materials, itemRecipe.MaterialsCount, out _materialsCount);

        _isMake = true;
        foreach (bool i in _boolArray)
        {
            if (i == false)
                _isMake = false;
        }
        UpdateIsMakeTextUI();
    }

    public void UpdateIsMakeTextUI()
    {
        if (_isMake)
        {
            _itemAvailability.text = "제작이 가능합니다.";
            _itemAvailability.color = Color.white;
        }
        else
        {
            _itemAvailability.text = "재료가 부족합니다.";
            _itemAvailability.color = Color.red;
        }
    }
}
