using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingController : MonoBehaviour
{
    [SerializeField] private GameObject _itemCraftingUI;
    [SerializeField] private GameObject _craftingItemTypeListPrefab;
    [SerializeField] private int _craftingItemListCount = 3;
    [SerializeField] private Transform _craftingItemListSpawn;

    private ItemCraftingManager _itemCraftingManager;
    private ItemDB _itemDB;
    private Dictionary<int,int> _itemEquipmentID;
    private Dictionary<int, int> _itemConsumableID;
    private Dictionary<int, int> _itemMaterialID;
    private List<ItemCraftingItemTypeList> _itemTypeLists;
    private Button _backButton;
    private Button _inventoryButton;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _itemCraftingManager = ItemCraftingManager.Instance;
        _itemCraftingManager.CraftingController = this;
        _itemDB = ItemDB.Instance;

        _itemEquipmentID = new Dictionary<int, int>();
        _itemConsumableID = new Dictionary<int, int>();
        _itemMaterialID = new Dictionary<int, int>();
        _itemTypeLists = new List<ItemCraftingItemTypeList>();

        _inventoryManager = InventoryManager.Instance;

    }

    private void Start()
    {
        if (_craftingItemTypeListPrefab == null)
            _craftingItemTypeListPrefab = Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/ItemTypeList");
        _itemCraftingUI = _itemCraftingManager.ItemCraftingUI;

        _backButton = _itemCraftingUI.transform.GetChild(4).GetChild(0).GetChild(2).GetComponent<Button>();
        _inventoryButton = _itemCraftingUI.transform.GetChild(4).GetChild(0).GetChild(1).GetComponent<Button>();

        if (_craftingItemListSpawn == null)
        {
            _craftingItemListSpawn = _itemCraftingUI.transform.GetChild(1).GetChild(0).GetChild(0);
        }

        CreateItemList();

        _backButton.onClick.AddListener( _itemCraftingManager.CallOffCraftingUIEvent);
        _inventoryButton.onClick.AddListener(_inventoryManager.CallOnInventoryDisplayEvent);
    }

    private void CreateItemList()
    {
        string[] str = { "���", "�Ҹ�ǰ", "���" };
        for(int i = 0;  i < _craftingItemListCount; i++)
        {
            GameObject obj = Instantiate(_craftingItemTypeListPrefab);
            obj.transform.SetParent(_craftingItemListSpawn, false);
            _itemTypeLists.Add(obj.GetComponent<ItemCraftingItemTypeList>());
            _itemTypeLists[i].listName.text = str[i];
        }

    }

    public void AddRecipe(int id)
    {
        if (10120000 > id && id >= 10110000)
        {
            if (_itemDB.GetRecipe(id, out ItemRecipe newRecipe))
            {
                int craftingItemIndex= (newRecipe.CraftingID / 1000) % 1000;
                switch (craftingItemIndex)
                {
                    case >= 200:
                        if (_itemMaterialID.ContainsKey(craftingItemIndex)) return;
                        _itemMaterialID.Add(craftingItemIndex % 100, newRecipe.CraftingID / 1000);
                        //���
                        break;
                    case >= 100:
                        if (_itemConsumableID.ContainsKey(craftingItemIndex)) return;
                        _itemConsumableID.Add(craftingItemIndex % 100, newRecipe.CraftingID / 1000);
                        //�Һ�
                        break;
                    default:
                        if (_itemEquipmentID.ContainsKey(craftingItemIndex)) return;
                        _itemEquipmentID.Add(craftingItemIndex % 100, newRecipe.CraftingID / 1000);
                        //���
                        break;
                }
                _itemTypeLists[craftingItemIndex / 100].AddRecipe(newRecipe);
            }
        }
        return;
    }
}
