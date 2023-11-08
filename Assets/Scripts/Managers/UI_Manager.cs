using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Manager : CustomSingleton<UI_Manager>
{
    protected UI_Manager() { }
    [SerializeField] private GameObject _cavas;
    
    private ItemDB _itemDB;
    private InventoryManager _inventoryManager;

    public GameObject Canvas { get { return _cavas; } }

    public event Action TurnOnQuickSlotEvent;
    public event Action TurnOffQuickSlotEvent;

    [Header("테스트용")]
    public GameObject player;
    public GameObject quickSlot_UI;
    public bool Test = true;

    private void Awake()
    {
        _cavas = GameObject.FindGameObjectWithTag("Canvas");

        if (_cavas == null)
        {
            _cavas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas"));
        }

        _inventoryManager = InventoryManager.Instance;
        _itemDB = ItemDB.Instance;
        if (Test)
        {
            player = Instantiate(Resources.Load<GameObject>("Prefabs/Test/Player"));
            quickSlot_UI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpecialAbilities/QuickSlot_UI"), _cavas.transform);
        }
    }

    public void CallTurnOnQuickSlot()
    {
        TurnOnQuickSlotEvent?.Invoke();
    }

    public void CallTurnOffQuickSlot()
    {
        TurnOffQuickSlotEvent?.Invoke();
    }
}
