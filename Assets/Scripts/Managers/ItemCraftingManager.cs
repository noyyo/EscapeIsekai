using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemCraftingManager : CustomSingleton<ItemCraftingManager>
{
    [SerializeField] private GameObject _ItemCraftingUI;
    [SerializeField] private ItemCraftingController _craftingController;
    
    private UI_Manager _uiManager;
    private ItemCraftingSlot _clickSlot;
    
    public ItemCraftingController CraftingController 
    { 
        get { return _craftingController; }
        set
        {
            if(_craftingController == null)
                _craftingController = value;
        }
    }

    private void Awake()
    {
        _uiManager = UI_Manager.Instance;
        if (_ItemCraftingUI == null)
        {
            _ItemCraftingUI = Resources.Load<GameObject>("Resources/Prefabs/UI/ItemCrafting/ItemCraftingUI");
            _ItemCraftingUI = Instantiate(_ItemCraftingUI, _uiManager.Canvas.transform);
        }
    }
    
}
