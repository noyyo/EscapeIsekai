using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : CustomSingleton<InventoryManager>
{
    protected InventoryManager() { }
    private UI_Inventory _ui_Inventory;
    private Inventory _inventory;
    private UI_Manager _ui_Manager;
    private GameObject _itemExplanationPopup;
    private GameObject _inventory_UI;
    private ItemCraftingManager _inventoryCraftingManager;
    private GameManager _gameManager;
    private ItemSlotInfo _clickItem;
    private Slot _ClickSlot;
    private ItemSlotInfo _newSlot;
    private int _temporaryStorageindex;
    private bool isDrop;
    private PlayerInputSystem _playerInputSystem;

    public UI_Inventory UI_Inventory { get { return _ui_Inventory; } }
    public Inventory Inventory { get { return _inventory; }}
    public GameObject ItemExplanationPopup { get { return _itemExplanationPopup; } }
    public GameObject Inventory_UI { get { return _inventory_UI; } }
    public ItemSlotInfo ClickItem { get { return _clickItem; } }
    public event Action onTextChangeEquipEvent;
    public event Action onTextChangeUnEquipEvent;
    

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _ui_Manager = UI_Manager.Instance;
        _inventoryCraftingManager = ItemCraftingManager.Instance;
    }

    private void Start()
    {
        Init();
        _playerInputSystem.InputActions.UI.Inventory.started += OnInventory;
    }

    private void Init()
    {
        _inventory_UI = _ui_Manager.Inventory_UI;
        _itemExplanationPopup = _inventory_UI.transform.GetChild(3).gameObject;
        _inventory = _gameManager.Player.GetComponent<Inventory>();
        _ui_Inventory = _gameManager.Player.GetComponent<UI_Inventory>();
        _playerInputSystem = _gameManager.Player.GetComponent<PlayerInputSystem>();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!_ui_Manager.IsTurnOnInventory)
            _ui_Manager.CallUI_InventoryTurnOn();
        else
            _ui_Manager.CallUI_InventoryTurnOff();
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

    public void CallOnTextChangeEquipEvent()
    {
        onTextChangeEquipEvent?.Invoke();
    }

    public void CalOnTextChangeUnEquipEvent()
    {
        onTextChangeUnEquipEvent?.Invoke();
    }


    //아이템 추가를 위해 Inventroy에서 호출
    public bool CallAddItems(ItemRecipe itemRecipe, out int[] errorItemCount)
    {
        bool[] array = _inventory.TryAddItems(itemRecipe, out errorItemCount);
        return !Array.Exists(array, x => x == false);
    }

    public bool CallAddItems(int[] id, int[] count, out int[] errorItemCount)
    {
        bool[] array = _inventory.TryAddItems(id, count, out errorItemCount);
        return !Array.Exists(array, x => x == false);
    }

    public bool CallAddItem(int id, int count, out int errorItemCount)
    {
        return _inventory.TryAddItem(id, count, out errorItemCount);
    }

    public bool CallIsCheckItem(int id, int count, out int sum)
    {
        return _inventory.IsCheckItem(id, count, out sum);
    }
    public bool[] CallIsCheckItems(int[] id, int[] count, out int[] sum)
    {
        return _inventory.IsCheckItems(id, count, out sum);
    }

    /// <summary>
    /// 제작될 이미지[0]와 재료의 이미지[1~]를 반환합니다.
    /// </summary>
    /// <param name="newRecipe"></param>
    /// <param name="sum"></param>
    /// <returns></returns>
    public Sprite[] CallIsCheckItems(in ItemRecipe newRecipe, out int[] sum)
    {
        return _inventory.IsCheckItems(newRecipe, out sum);
    }
    public bool CallAddItems(ItemRecipe itemRecipe)
    {
        bool[] array = _inventory.TryAddItems(itemRecipe);
        return !Array.Exists(array, x => x == false);
    }

    public bool CallAddItems(int[] id, int[] count)
    {
        bool[] array = _inventory.TryAddItems(id, count);
        return !Array.Exists(array, x => x == false);
    }

    public bool CallAddItem(int id, int count)
    {
        return _inventory.TryAddItem(id, count);
    }

    public bool CallIsCheckItem(int id, int count)
    {
        return _inventory.IsCheckItem(id, count);
    }

    public bool[] CallIsCheckItems(int[] id, int[] count)
    {
        return _inventory.IsCheckItems(id, count);
    }

    /// <summary>
    /// 제작될 이미지[0]와 재료의 이미지[1~]를 반환합니다.
    /// </summary>
    /// <param name="newRecipe"></param>
    /// <returns></returns>
    public Sprite[] CallIsCheckItems(in ItemRecipe newRecipe)
    {
        return _inventory.IsCheckItems(newRecipe);
    }
}
