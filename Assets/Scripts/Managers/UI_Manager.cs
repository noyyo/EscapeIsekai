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
    private GameObject _player;
    private GameObject _quickSlot_UI;
    

    public GameObject Canvas { get { return _cavas; } }
    public GameObject quickSlot_UI { get { return _quickSlot_UI; } }
    public GameObject Player { get { return _player; } }
    public event Action TurnOnQuickSlotEvent;
    public event Action TurnOffQuickSlotEvent;
    public bool Test = true;

    private void Awake()
    {
        _cavas = GameObject.FindGameObjectWithTag("Canvas");

        if (_cavas == null)
        {
            _cavas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas"));
        }
        _quickSlot_UI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpecialAbilities/QuickSlot_UI"), _cavas.transform);
        _player = GameObject.FindGameObjectWithTag("Player");
        if (Test)
        {
            _player = Instantiate(Resources.Load<GameObject>("Prefabs/Test/Player"));
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
