using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _slotSpawn;
    [SerializeField] private GameObject _inventoryTypeGroup;

    private UI_Manager _ui_manager;
    private GameObject _slotPrefab;
    private Inventory _playerInventory;
    private GameObject _inventory_UI;
    private bool _isInventoryUI;
    private Button[] _inventoryTypeButton;

    public Slot[] slotArray;
    public event Action OnInventoryEvent;

    private void Awake()
    {
        _ui_manager = UI_Manager.Instance;
        Init();
        CreateSlot();
        _inventoryTypeButton = _inventoryTypeGroup.transform.GetComponentsInChildren<Button>();
    }

    private void Start()
    {
        OnInventoryEvent += SetActiveInventroyUI;
        _inventoryTypeButton[0].onClick.AddListener(() => CallItemSlots(DisplayType.Equipment));
        _inventoryTypeButton[1].onClick.AddListener(() => CallItemSlots(DisplayType.USE));
        _inventoryTypeButton[2].onClick.AddListener(() => CallItemSlots(DisplayType.Material));
        _inventoryTypeButton[3].onClick.AddListener(() => CallItemSlots(DisplayType.ETC));
    }

    private void Init()
    {
        _slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/Slot");
        _playerInventory = GetComponent<Inventory>();
        slotArray = new Slot[_playerInventory.dataLength];
        _inventory_UI = _ui_manager.Inventory_UI;
    }


    private void CreateSlot()
    {
        int slotCount = _playerInventory.dataLength;
        for (int i = 0; i < slotCount; i++)
        {
            GameObject obj = Instantiate(_slotPrefab);
            obj.transform.SetParent(_slotSpawn.transform, false);
            slotArray[i] = obj.GetComponent<Slot>();
        }
    }

    //액션에 걸린 이벤트( 인벤토리 창만 띄움)
    public void SetActiveInventroyUI()
    {
        _isInventoryUI = !_isInventoryUI;
        _inventory_UI.SetActive(_isInventoryUI);
    }

    //Action을 통해 호출
    public void OnInventory()
    {
        OnInventoryEvent?.Invoke();
        _playerInventory.SetDisPlayNowType();
    }

    private void CallItemSlots(DisplayType displayType)
    {
        _playerInventory.SetDisplayType(displayType);
    }
}
