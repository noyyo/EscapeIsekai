using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : CustomSingleton<InventoryManager>
{
    protected InventoryManager() { }

    [Header("�׽�Ʈ��")]
    [SerializeField][Tooltip("���� �߻��� ���� ���� �� �ڵ����")] private UI_Inventory _ui_Inventory;
    [SerializeField][Tooltip("���� �߻��� ���� ���� �� �ڵ����")] private Inventory _inventory;
    private UI_Manager _ui_Manager;
    private GameObject _itemExplanationPopup;
    private GameObject _inventory_UI;

    private ItemSlotInfo _clickItem;
    private Slot _ClickSlot;
    private ItemSlotInfo _newSlot;

    private ItemSlotInfo _temporaryStorage;
    private int _temporaryStorageindex;

    private bool isDrop;

    public UI_Inventory UI_Inventory { get { return _ui_Inventory; } }
    public Inventory Inventory { get { return _inventory; }}
    public GameObject ItemExplanationPopup { get { return _itemExplanationPopup; } }
    public GameObject Inventory_UI { get { return _inventory_UI; } }

    private void Awake()
    {
        _ui_Manager = UI_Manager.Instance;
        _inventory_UI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Inventory/Inventory"), _ui_Manager.Canvas.transform);
        _itemExplanationPopup = _inventory_UI.transform.GetChild(3).gameObject;
    }

    private void Start()
    {
        if (_inventory == null)
            _inventory = _ui_Manager.player.GetComponent<Inventory>();
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

    //��������� �ٲٱ� ���� �� ����
    public void SaveNewChangedSlot(ItemSlotInfo newSlot, int uniqueIndex)
    {
        _newSlot = newSlot;
        _temporaryStorageindex = uniqueIndex;
        isDrop = true;
    }

    public void CallChangeSlot(ItemSlotInfo oldSlot, int uniqueIndex)
    {
        //�ش� ��ü�� �����̿��� �ٲٱ� ��ȯ( ������ �ƴϸ� OnDrop�� ȣ����� �ʾƼ�
        //SaveNewChangedSlot�� ȣ����� ����)
        if (isDrop)
        {
            _inventory.ChangeSlot(_newSlot, oldSlot, _temporaryStorageindex, uniqueIndex);
            _ui_Inventory.InventoryUITurnOff();
            isDrop = false;
        }
    }
}
