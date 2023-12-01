using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : CustomSingleton<InventoryManager>
{
    protected InventoryManager() { }

    [SerializeField] private int itemKategorieCount = 4;
    [SerializeField] private int inventroySlotCount = 60;
    //추가 및 삭제를 위해 List를 생성
    private List<Slot> slotList;
    //위와 비슷한 이유로 List로 생성
    private Dictionary<int, Item>[] itemDics;

    private GameManager gameManager;
    private UI_Manager ui_Manager;
    private UI_Inventory ui_Inventory;
    private Inventory inventory;

    private int clickSlotIndex;
    private int dropSlotIndex;
    private bool isDrop;
    private ItemType displayType;

    public Inventory Inventory { get { return inventory; }}
    public List<Slot> SlotList { get { return slotList; }}
    public Dictionary<int, Item>[] ItemDics { get { return itemDics; }}
    public int InventroySlotCount { get { return inventroySlotCount; }}
    public int ClickSlotIndex { get { return clickSlotIndex; }}

    public event Action OnTextChangeEquipEvent;
    public event Action OnTextChangeUnEquipEvent;
    public event Action<Item> OnItemExplanationPopUpEvent;
    public event Action<ItemType> OnSetDisplayTypeEvent;

    public event Action<Item> OnEquipItemEvent;
    public event Action<Item> UnEquipItemEvent;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        ui_Manager = UI_Manager.Instance;
        itemDics = new Dictionary<int, Item>[itemKategorieCount];
        for (int i = 0; i < itemKategorieCount; i++)
            itemDics[i] = new Dictionary<int, Item>();
        slotList = new List<Slot>();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        displayType = ItemType.Equipment;
        inventory = gameManager.Player.GetComponent<Inventory>();
        ui_Inventory = ui_Manager.Inventory_UI.GetComponent<UI_Inventory>();
    }

    //버튼을 클릭했을 호출
    public void SetClickItem(int index)
    {
        if(clickSlotIndex != index)
            CallTurnOffItemClick();
        clickSlotIndex = index;

    }

    public void CallDisplayInventoryTailUI(bool isEquip)
    {
        ui_Inventory.DisplayInventoryTailUI(isEquip);
    }

    public void CallTurnOffItemClick()
    {
        SlotList[clickSlotIndex].TurnOffItemClick();
    }

    //드래그앤드롭했을때 바꾸기 위한 값 저장
    public void SaveNewChangedSlot(int slotIndex)
    {
        dropSlotIndex = slotIndex;
        isDrop = true;
    }

    public void CallChangeSlot(int slotIndex)
    {
        //해당 객체가 슬롯이여만 바꾸기 전환(슬롯이 아니면 OnDrop이 호출되지 않아서
        //SaveNewChangedSlot가 호출되지 않음)
        if (isDrop)
        {
            inventory.ChangeSlot(dropSlotIndex, slotIndex);
            ui_Inventory.InventoryUITurnOff();
            isDrop = false;
        }
    }

    public void CallOnTextChangeEquipEvent()
    {
        OnTextChangeEquipEvent?.Invoke();
    }

    public void CalOnTextChangeUnEquipEvent()
    {
        OnTextChangeUnEquipEvent?.Invoke();
    }

    public void CallOnItemExplanationPopUp()
    {
        OnItemExplanationPopUpEvent?.Invoke(itemDics[(int)displayType][clickSlotIndex]);
    }

    public void CallOnSetDisplayType(ItemType itemtype)
    {
        displayType = itemtype;
        OnSetDisplayTypeEvent?.Invoke(itemtype);
    }

    //아이템 추가를 위해 Inventroy에서 호출
    public bool CallTryAddItems(int[] id, int[] count)
    {
        return inventory.TryAddItems(id, count);
    }

    public bool CallTryAddItem(int id, int count)
    {
        return inventory.TryAddItem(id, count);
    }

    public bool CallIsCheckItem(int id, int count, out int sum)
    {
        return inventory.IsCheckItem(id, count, out sum);
    }
    public bool[] CallIsCheckItems(int[] id, int[] count, out int[] sum)
    {
        return inventory.IsCheckItems(id, count, out sum);
    }

    /// <summary>
    /// 제작될 이미지[0]와 재료의 이미지[1~]를 반환합니다.
    /// </summary>
    /// <param name="newRecipe"></param>
    /// <param name="sum"></param>
    /// <returns></returns>
    public Sprite[] CallIsCheckItems(in ItemRecipe newRecipe, out int[] sum)
    {
        return inventory.IsCheckItems(newRecipe, out sum);
    }
    public bool CallAddItems(ItemRecipe itemRecipe)
    {
        return inventory.TryAddItems(itemRecipe);
    }

    public bool CallAddItems(int[] id, int[] count)
    {
        return inventory.TryAddItems(id, count);
    }

    public bool CallAddItem(int id, int count)
    {
        return inventory.TryAddItem(id, count);
    }

    public bool CallIsCheckItem(int id, int count)
    {
        return inventory.IsCheckItem(id, count);
    }

    public bool[] CallIsCheckItems(int[] id, int[] count)
    {
        return inventory.IsCheckItems(id, count);
    }

    /// <summary>
    /// 제작될 이미지[0]와 재료의 이미지[1~]를 반환합니다.
    /// </summary>
    /// <param name="newRecipe"></param>
    /// <returns></returns>
    public Sprite[] CallIsCheckItems(in ItemRecipe newRecipe)
    {
        return inventory.IsCheckItems(newRecipe);
    }

    public void CallOnEquipItemEvent(Item item)
    {
        OnEquipItemEvent?.Invoke(item);
    }
    public void CallUnEquipItemEvent(Item item)
    {
        UnEquipItemEvent?.Invoke(item);
    }
}
