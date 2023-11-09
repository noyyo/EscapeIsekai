using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : CustomSingleton<InventoryManager>
{
    protected InventoryManager() { }
    [SerializeField] private UI_Inventory _ui_Inventory;
    [SerializeField] private Inventory _inventory;
    private UI_Manager _ui_Manager;
    private GameObject _itemExplanationPopup;
    private GameObject _inventory_UI;
    private ItemCraftingManager _inventoryCraftingManager;

    private ItemSlotInfo _clickItem;
    private Slot _ClickSlot;
    private ItemSlotInfo _newSlot;
    private int _temporaryStorageindex;

    private bool isDrop;
    private bool isDisplay;

    public UI_Inventory UI_Inventory { get { return _ui_Inventory; } }
    public Inventory Inventory { get { return _inventory; }}
    public GameObject ItemExplanationPopup { get { return _itemExplanationPopup; } }
    public GameObject Inventory_UI { get { return _inventory_UI; } }
    public bool IsDisplay { get { return  isDisplay; } }
    public ItemSlotInfo ClickItem { get { return _clickItem; } }
    public event Action OnInventoryDisplayEvent;
    public event Action onTextChangeEquipEvent;
    public event Action onTextChangeUnEquipEvent; 

    private void Awake()
    {
        _ui_Manager = UI_Manager.Instance;
        _inventory_UI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Inventory/Inventory"), _ui_Manager.Canvas.transform);
        _itemExplanationPopup = _inventory_UI.transform.GetChild(3).gameObject;
        _inventoryCraftingManager = ItemCraftingManager.Instance;
    }

    private void Start()
    {
        if (_inventory == null)
            _inventory = _ui_Manager.Player.GetComponent<Inventory>();
        if (_ui_Inventory == null)
            _ui_Inventory = _inventory.GetComponent<UI_Inventory>();
    }
    public void SetClickItem(ItemSlotInfo iteminfo, Slot slot)
    {
        if (_clickItem != null)
        {
            if (_clickItem.index != iteminfo.index)
                CallTurnOffItemClick();
        }
        _clickItem = iteminfo;
        _ClickSlot = slot;
    }

    public ItemSlotInfo GetClickItem()
    {
        return _clickItem;
    }

    public void CallDisplayInventoryTailUI()
    {
        _ui_Inventory.DisplayInventoryTailUI();
    }

    public void CallTurnOffItemClick()
    {
        _ClickSlot?.TurnOffItemClick();
    }

    //드롭했을때 바꾸기 위한 값 저장
    public void SaveNewChangedSlot(ItemSlotInfo newSlot, int uniqueIndex)
    {
        _newSlot = newSlot;
        _temporaryStorageindex = uniqueIndex;
        isDrop = true;
    }

    public void CallChangeSlot(ItemSlotInfo oldSlot, int uniqueIndex)
    {
        //해당 객체가 슬롯이여만 바꾸기 전환( 슬롯이 아니면 OnDrop이 호출되지 않아서
        //SaveNewChangedSlot가 호출되지 않음)
        if (isDrop)
        {
            _inventory.ChangeSlot(_newSlot, oldSlot, _temporaryStorageindex, uniqueIndex);
            _ui_Inventory.InventoryUITurnOff();
            isDrop = false;
        }
    }

    public void CallAddItems(ItemRecipe itemRecipe, out int[] errorItemCount)
    {
        _inventory.TryAddItems(itemRecipe, out errorItemCount);
    }

    public void CallAddItems(int[] id, int[] count, out int[] errorItemCount)
    {
        _inventory.TryAddItems(id, count, out errorItemCount);
    }

    public void CallAddItem(int id, int count, out int errorItemCount)
    {
        _inventory.TryAddItem(id, count, out errorItemCount);
    }

    public void CallAddItems(ItemRecipe itemRecipe)
    {
        _inventory.TryAddItems(itemRecipe);
    }

    public void CallAddItems(int[] id, int[] count)
    {
        _inventory.TryAddItems(id, count);
    }


    public void CallAddItem(int id, int count)
    {
        _inventory.TryAddItem(id, count);
    }

    public bool CallIsCheckItem(int id, int count, out int sum)
    {
        return _inventory.IsCheckItem(id, count, out sum);
    }
    public bool[] CallIsCheckItems(int[] id, int[] count, out int[] sum)
    {
        return _inventory.IsCheckItems(id, count, out sum);
    }

    public Sprite[] CallIsCheckItems(in ItemRecipe newRecipe, out int[] sum)
    {
        return _inventory.IsCheckItems(newRecipe, out sum);
    }

    public bool CallIsCheckItem(int id, int count)
    {
        return _inventory.IsCheckItem(id, count);
    }
    public bool[] CallIsCheckItems(int[] id, int[] count)
    {
        return _inventory.IsCheckItems(id, count);
    }

    public Sprite[] CallIsCheckItems(in ItemRecipe newRecipe)
    {
        return _inventory.IsCheckItems(newRecipe);
    }





    public void CallOnInventoryDisplayEvent()
    {
        isDisplay = !isDisplay;
        if (isDisplay)
        {
            _inventoryCraftingManager.CallOffCraftingUIEvent();
        }
        OnInventoryDisplayEvent?.Invoke();
    }

    public void CallOnTextChangeEquipEvent()
    {
        onTextChangeEquipEvent?.Invoke();
    }

    public void CalOnTextChangeUnEquipEvent()
    {
        onTextChangeUnEquipEvent?.Invoke();
    }
}
