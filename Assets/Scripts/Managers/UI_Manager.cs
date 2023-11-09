using System;
using UnityEngine;

public class UI_Manager : CustomSingleton<UI_Manager>
{
    protected UI_Manager() { }
    [SerializeField] private GameObject _cavas;
    
    private ItemDB _itemDB;
    private InventoryManager _inventoryManager;
    private ItemCraftingManager _itemCraftingManager;
    private GameObject _player;
    private GameObject _quickSlot_UI;
    
    public GameObject Canvas { get { return _cavas; } }
    public GameObject quickSlot_UI { get { return _quickSlot_UI; } }
    public GameObject Player { get { return _player; } }
    public event Action TurnOnQuickSlotEvent;
    public event Action TurnOffQuickSlotEvent;

    private void Awake()
    {
        _cavas = GameObject.FindGameObjectWithTag("Canvas");
        if (_cavas == null)
        {
            _cavas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas"));
        }
        _quickSlot_UI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpecialAbilities/QuickSlot_UI"), _cavas.transform);

        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
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
