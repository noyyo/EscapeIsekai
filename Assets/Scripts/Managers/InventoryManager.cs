using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : CustomSingleton<InventoryManager>
{
    protected InventoryManager() { }

    [Header("테스트용")]
    [SerializeField][Tooltip("오류 발생시 직접 참조 및 코드수정")] private UI_Inventory _ui_Inventory;
    [SerializeField][Tooltip("오류 발생시 직접 참조 및 코드수정")] private Inventory _inventory;
    private UI_Manager _ui_Manager;
    private GameObject _itemExplanationPopup;
    private GameObject _inventory_UI;

    private ItemSlotInfo _clickItem;
    private UI_Slot _ClickSlot;
    private ItemSlotInfo _newSlot;

    private ItemSlotInfo _temporaryStorage;
    private int _temporaryStorageindex;

    public UI_Inventory UI_Inventory { get { return _ui_Inventory; } }
    public Inventory Inventory { get { return _inventory; }}
    public GameObject ItemExplanationPopup { get { return _itemExplanationPopup; } }
    public GameObject Inventory_UI { get { return _inventory_UI; } }

    private void Awake()
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");

        if (canvas == null)
        {
            canvas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas"));
        }

        _inventory_UI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Inventory/Inventory"), canvas.transform);
        if (_ui_Inventory == null)
            _ui_Inventory = _inventory_UI.GetComponent<UI_Inventory>();
        if( _inventory == null)
            _inventory = Resources.Load<GameObject>("Prefabs/Test/Player").GetComponent<Inventory>();
        _ui_Manager = UI_Manager.Instance;
        _itemExplanationPopup = _ui_Inventory.ItemExplanationPopup;
    }

    public void SetClickItem(ItemSlotInfo iteminfo, UI_Slot ui_Slot)
    {
        if (_clickItem != null)
        {
            if (_clickItem.index != iteminfo.index)
                CallTurnOffItemClick();
        }


        _clickItem = iteminfo;
        _ClickSlot = ui_Slot;
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

    public void SaveNewChangedSlot(ItemSlotInfo newSlot, int uniqueIndex)
    {
        _newSlot = newSlot;
        _temporaryStorageindex = uniqueIndex;
    }

    public void CallChangeSlot(ItemSlotInfo oldSlot, int uniqueIndex)
    {
        _inventory.ChangeSlot(_newSlot, oldSlot, _temporaryStorageindex, uniqueIndex);
        _ClickSlot?.TurnOffItemClick();
        _ui_Inventory.TurnOffItemExplanationPopup();
        _ui_Inventory.TurnOffInventoryTailUI();
    }
}
