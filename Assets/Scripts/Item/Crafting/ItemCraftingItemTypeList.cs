using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingItemTypeList : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject craftingItemSlotSpawn;
    [SerializeField] private Button button;
    private UI_Manager uiManager;
    private ItemCraftingManager craftingManager;
    private GameObject prefabs;
    private ContentSizeFitter contentSizeFitter;
    private List<ItemCraftingSlot> slotList = new List<ItemCraftingSlot>();
    private int slotListLength = -1;
    private bool isDisplay;
    public event Action slotActiveEvent;

    private void Awake()
    {
        uiManager = UI_Manager.Instance;
        craftingManager = ItemCraftingManager.Instance;
        if (button == null)
            button = GetComponentInChildren<Button>();
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        prefabs = craftingManager.CraftingSlotPrefab;
        if (craftingItemSlotSpawn == null)
            craftingItemSlotSpawn = gameObject;
        isDisplay = false;      
    }

    private void Start()
    {
        button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        isDisplay = !isDisplay;
        if (isDisplay)
            arrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        else
            arrow.transform.rotation = Quaternion.identity;
        slotActiveEvent?.Invoke();
        uiManager.PlayClickBtnSound();
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
