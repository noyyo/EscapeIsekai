using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingItemTypeList : MonoBehaviour
{
    public TMP_Text listName;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Button button;
    [SerializeField] private GameObject craftingItemSlotSpawn;
    private bool _isDisplay;
    private List<ItemCraftingSlot> slotList = new List<ItemCraftingSlot>();
    private ItemCraftingManager craftingManager;
    private GameObject prefabs;
    private int slotListLength = -1;
    private ContentSizeFitter contentSizeFitter;
    private UI_Manager ui_Manager;

    public event Action slotActiveEvent;

    private void Awake()
    {
        ui_Manager = UI_Manager.Instance;
        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }
        _isDisplay = false;
        craftingManager = ItemCraftingManager.Instance;
        prefabs = craftingManager.CraftingSlotPrefab;

        if (craftingItemSlotSpawn == null)
        {
            craftingItemSlotSpawn = this.gameObject;
        }
        contentSizeFitter = GetComponent<ContentSizeFitter>();
    }

    private void Start()
    {
        button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        _isDisplay = !_isDisplay;
        if (_isDisplay)
            arrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        else
            arrow.transform.rotation = Quaternion.identity;
        slotActiveEvent?.Invoke();
        ui_Manager.PlayClickBtnSound();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentSizeFitter.transform);
    }

    public void AddRecipe(in ItemRecipe newRecipe)
    {
        CreateItemCraftingSlot();
        slotList[slotListLength].SetSlot(newRecipe);
    }

    private void CreateItemCraftingSlot()
    {
        ItemCraftingSlot newSlot = Instantiate(prefabs, this.transform).GetComponent<ItemCraftingSlot>();
        slotActiveEvent += newSlot.TurnOnOffSlot;
        slotList.Add(newSlot);
        slotListLength++;
    }
}
