using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    private int slotMaxCount;
    private GameObject slotPrefab;
    private GameObject slotSpawn;
    private InventoryManager inventoryManager;
    private UI_Manager ui_Manager;
    private ItemManager itemManager;
    private List<Slot> slotList;
    private Dictionary<int, Item>[] itemDics;
    private ItemDB itemDB;
    private ItemType displayType;
    private int clickSlotIndex;
    private PlayerInputSystem playerInputSystem;
    private Player player;

    public ItemType DisplayType { get { return displayType; } }

    public event Action<int,int> AddItem;
    private void Awake()
    {
        player = GetComponent<Player>();
        ui_Manager = UI_Manager.Instance;
        itemDB = ItemDB.Instance;
        inventoryManager = InventoryManager.Instance;
        itemManager = ItemManager.Instance;
        slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/Slot");
        playerInputSystem = GetComponent<PlayerInputSystem>();
        InitInventory();
        CreateSlot();
    }

    private void Start()
    {
        DisplaySlotAllClear();
        ui_Manager.UI_InventoryTurnOnEvent += OnDisplaySlot;
        playerInputSystem.InputActions.UI.Inventory.started += OnInventory;
        inventoryManager.OnSetDisplayTypeEvent += SetDisplayType;
    }

    private void InitInventory()
    {
        slotList = inventoryManager.SlotList;
        itemDics = inventoryManager.ItemDics;
        slotMaxCount = inventoryManager.InventroySlotCount;
        displayType = ItemType.Equipment;
        slotSpawn = ui_Manager.Inventory_UI.transform.GetChild(4).GetChild(0).GetChild(0).gameObject;
    }

    private void CreateSlot()
    {
        for (int i = 0; i < slotMaxCount; i++)
        {
            GameObject obj = Instantiate(slotPrefab);
            obj.transform.SetParent(slotSpawn.transform, false);
            obj.GetComponent<Slot>().UniqueIndex = i;
            slotList.Add(obj.GetComponent<Slot>());
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!ui_Manager.IsTurnOnInventory)
        {
            ui_Manager.CallUI_InventoryTurnOn();
        }
        else
        {
            ui_Manager.CallUI_InventoryTurnOff();
        }  
    }

    /// <summary>
    /// 우선 더하고 초과되거나 부족한 값은 errorItemCount로 반환
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <param name="errorItemCount"></param>
    /// <returns></returns>
    public bool Add(int id, int count, out int errorItemCount)
    {
        errorItemCount = count;
        bool isAddItem = false;
        if (itemDB.GetItemData(id, out ItemData newItem))
        {
            int newItemType = (id / 100000) % 10;
            if (count >= 0)
                isAddItem = AddList(itemDics[newItemType], count, newItemType, in newItem, out errorItemCount);
            else
                isAddItem = SubList(itemDics[newItemType], count, newItemType, in newItem, out errorItemCount);
        }
        return isAddItem;
    }

    public bool Add(int id, int count)
    {
        bool isAddItem = false;
        if (itemDB.GetItemData(id, out ItemData newItem))
        {
            int newItemType = (id / 100000) % 10;
            if (count >= 0)
                isAddItem = AddList(itemDics[newItemType], count, newItemType, in newItem);
            else
                isAddItem = SubList(itemDics[newItemType], count, newItemType, in newItem);
        }
        return isAddItem;
    }

    /// <summary>
    /// 우선 더하고 초과되거나 부족한 값은 errorItemCount로 반환
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <param name="errorItemCount"></param>
    /// <returns></returns>
    public bool[] Add(int[] id, int[] count, out int[] errorItemCount)
    {
        int idCount = id.Length;
        bool[] boolArray = new bool[idCount];
        errorItemCount = new int[idCount];
        for (int i = 0; i < idCount; i++)
            boolArray[i] = Add(id[i], count[i], out errorItemCount[i]);
        return boolArray;
    }

    public bool[] Add(int[] id, int[] count)
    {
        int idCount = id.Length;
        bool[] boolArray = new bool[idCount];
        for (int i = 0; i < idCount; i++)
            boolArray[i] = Add(id[i], count[i]);
        return boolArray;
    }

    /// <summary>
    /// 값을 확인 후 문제없으면 추가 및 삭제 진행
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool TryAddItem(int id, int count)
    {
        bool isTrue;
        if (count > 0)
            isTrue =  IsCheckSpace(id, count);
        else
            isTrue = IsCheckItem(id, count);

        if(!isTrue)
            return isTrue;

        if (itemDB.GetItemData(id, out ItemData newItem))
        {
            int newItemType = (id / 100000) % 10;
            if (count >= 0)
                isTrue = AddList(itemDics[newItemType], count, newItemType, in newItem);
            else
                isTrue = SubList(itemDics[newItemType], count, newItemType, in newItem);
        }
        if (isTrue)
        {
            AddItem?.Invoke(id, count);
        }
        return isTrue;
    }

    /// <summary>
    /// 값을 확인 후 문제없으면 추가 및 삭제 진행
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool TryAddItems(int[] id, int[] count)
    {
        int idCount = id.Length;
        bool isTrue = true;
        for (int i = 0; i < idCount; i++)
        {
            if (count[i] > 0)
                isTrue = IsCheckSpace(id[i], count[i]);
            else if (count[i] < 0)
                isTrue = IsCheckItem(id[i], count[i]);

            if (!isTrue)
                return isTrue;
        }

        for (int i = 0; i < idCount; i++)
            TryAddItem(id[i], count[i]);
        return isTrue;
    }

    public bool TryAddItems(ItemRecipe itemRecipe)
    {
        int idCount = itemRecipe.Materials.Length;
        if (!IsCheckSpace(itemRecipe.CraftingID, 1))
            return false;
        for (int i = 0; i < idCount; i++)
        {
            if (!IsCheckItem(itemRecipe.Materials[i], itemRecipe.MaterialsCount[i]))
                return false;
        }

        for (int i = 0; i < idCount; i++)
            TryAddItem(itemRecipe.Materials[i], -(itemRecipe.MaterialsCount[i]));
        TryAddItem(itemRecipe.CraftingID, itemRecipe.AvailableCount);
        return true;
    }

    private bool AddList(Dictionary<int, Item> itemDic, int count, int slotType, in ItemData newItem, out int errorItemCount)
    {
        if (itemDic.Count == slotMaxCount)
        {
            errorItemCount = count;
            return false;
        }

        errorItemCount = 0;
        int ID = newItem.ID;
        bool isDisplay = false;
        int newItemMaxCount = newItem.MaxCount;

        if (slotType == (int)displayType)
            isDisplay = true;

        //아이템 리스트에서 확인 후 값을 수정하는 부분
        if(itemDic.Count > 0)
        {
            foreach (KeyValuePair<int, Item> i in itemDic)
            {
                if (i.Value.ID == ID && !(i.Value.IsMax))
                {
                    if (!(i.Value.TryAddItem(count, out errorItemCount)))
                        count = errorItemCount;
                    else
                    {
                        if (isDisplay)
                            slotList[i.Key].SetSlotCount(i.Value.Count);
                        return true;
                    }
                        
                    if (isDisplay)
                        slotList[i.Key].SetSlotCount(i.Value.Count);
                }
            }
        }
        //새롭게 추가해주는 부분
        bool isBreak = true;
        while (isBreak)
        {
            int addCount;
            if (count >= newItemMaxCount)
            {
                addCount = newItemMaxCount;
                count -= newItemMaxCount;
            }
            else if(count == 0)
                return true;
            else
            {
                addCount = count;
                isBreak = false;
            }

            int index = CheckSlotIndex(slotType);
            if (index == -1)
            {
                errorItemCount = count;
                return false;
            }
            //추가
            itemDic.Add(index, itemManager.InstantiateItemObject(ID, addCount));
            //표시
            if (isDisplay)
                slotList[index].AddItem(itemDic[index]);
        }
        return true;
    }

    private bool AddList(Dictionary<int, Item> itemDic, int count, int slotType, in ItemData newItem)
    {
        if (itemDic.Count == slotMaxCount)
            return false;

        int ID = newItem.ID;
        bool isDisplay = false;
        int newItemMaxCount = newItem.MaxCount;

        if (slotType == (int)displayType)
            isDisplay = true;

        int outCount = 0;
        //아이템 리스트에서 확인 후 값을 수정하는 부분
        if (itemDic.Count > 0)
        {
            foreach (KeyValuePair<int, Item> i in itemDic)
            {
                if (i.Value.ID == ID && !(i.Value.IsMax))
                {
                    if (!(i.Value.TryAddItem(count, out outCount)))
                        count = outCount;
                    else
                    {
                        if (isDisplay)
                            slotList[i.Key].SetSlotCount(i.Value.Count);
                        return true;
                    }

                    if (isDisplay)
                        slotList[i.Key].SetSlotCount(i.Value.Count);
                }
            }
        }
        //새롭게 추가해주는 부분
        bool isBreak = true;
        while (isBreak)
        {
            int addCount;
            if (count >= newItemMaxCount)
            {
                addCount = newItemMaxCount;
                count -= newItemMaxCount;
            }
            else if (count == 0)
                return true;
            else
            {
                addCount = count;
                isBreak = false;
            }

            int index = CheckSlotIndex(slotType);
            if (index == -1)
                return false;
            //추가
            itemDic.Add(index, itemManager.InstantiateItemObject(ID, addCount));
            //표시
            if (isDisplay)
                slotList[index].AddItem(itemDic[index]);
        }
        return true;
    }

    private bool SubList(Dictionary<int, Item> itemDic, int count, int slotType, in ItemData newItem, out int ErrorItemCount)
    {
        if (itemDic.Count == 0)
        {
            ErrorItemCount = count;
            return false;
        }

        int ID = newItem.ID;
        Stack stack = new Stack();
        ErrorItemCount = 0;
        bool isDisplay = false;

        if (slotType == (int)displayType)
            isDisplay = true;

        //인벤토리에 있는 값 확인후 뺴기
        foreach (KeyValuePair<int, Item> i in itemDic)
        {
            if (i.Value.ID == ID && !(i.Value.IsEquip) )
            {
                if (!(i.Value.TryAddItem(count, out ErrorItemCount)))
                {
                    count = ErrorItemCount;
                    stack.Push(i.Key);
                }   
                else
                {
                    if (i.Value.Count == 0)
                        stack.Push(i.Key);
                    else
                        if (isDisplay)
                        slotList[i.Key].SetSlotCount(i.Value.Count);
                    break;
                }
            }
        }

        for(int i = 0; i < stack.Count;)
        {
            int n = (int)stack.Pop();
            if (isDisplay)
                slotList[n].ClearSlot();
            itemDic.Remove(n);
        }

        if(ErrorItemCount > 0)
            return false;
        else
            return true;
    }

    private bool SubList(Dictionary<int, Item> itemDic, int count, int slotType, in ItemData newItem)
    {
        if (itemDic.Count == 0)
            return false;

        int ID = newItem.ID;
        Stack stack = new Stack();
        bool isDisplay = false;

        if (slotType == (int)displayType)
            isDisplay = true;

        int outCount = 0;
        //인벤토리에 있는 값 확인후 뺴기
        foreach (KeyValuePair<int, Item> i in itemDic)
        {
            if (i.Value.ID == ID && !(i.Value.IsEquip))
            {
                if (!(i.Value.TryAddItem(count, out outCount)))
                {
                    count = outCount;
                    stack.Push(i.Key);
                }
                else
                {
                    if (i.Value.Count == 0)
                        stack.Push(i.Key);
                    else
                        if (isDisplay)
                        slotList[i.Key].SetSlotCount(i.Value.Count);
                    break;
                }
            }
        }

        for (int i = 0; i < stack.Count;)
        {
            int n = (int)stack.Pop();
            if (isDisplay)
                slotList[n].ClearSlot();
            itemDic.Remove(n);
        }

        if (outCount > 0)
            return false;
        else
            return true;
    }

    private int CheckSlotIndex(int itemType)
    {
        for(int i = 0; i < slotMaxCount; i++)
        {
            if (!(itemDics[itemType].ContainsKey(i)))
                return i;
        }
        return -1;
    }

    public void SortInventory()
    {
        DisplaySlotAllClear();
        Queue<int> queue = new Queue<int>();
        foreach(KeyValuePair<int,Item> i in itemDics[(int)displayType])
            queue.Enqueue(i.Key);
        int queueCount = queue.Count;
        for (int i = 0; i < queueCount; i++)
        {
            int n = queue.Dequeue();
            if (i != n)
            {
                itemDics[(int)displayType].Add(i, itemDics[(int)displayType][n]);
                itemDics[(int)displayType].Remove(n);
            }
        }
        OnDisplaySlot();
    }

    public void UseItem()
    {
        clickSlotIndex = inventoryManager.ClickSlotIndex;
        if (displayType == ItemType.Equipment)
        {
            EquipItem();
        }
        else
        {
            player.Playerconditions.Eat((float)itemDics[(int)DisplayType][inventoryManager.ClickSlotIndex].DefaultHunger);
            player.Playerconditions.Heal((float)itemDics[(int)DisplayType][inventoryManager.ClickSlotIndex].DefaultHP);
            TryAddItem(itemDics[(int)DisplayType][inventoryManager.ClickSlotIndex].ID, -1);
        }
            
    }

    private void EquipItem()
    {
        if (!(itemDics[(int)displayType][clickSlotIndex].IsEquip))
        {
            Debug.Log("장비를 장착합니다..");
            itemDics[(int)displayType][clickSlotIndex].IsEquip = true;
            inventoryManager.CallOnEquipItemEvent(itemDics[(int)displayType][clickSlotIndex]);
            slotList[clickSlotIndex].DisplayEquip();
            //사용 버튼의 텍스트를 장비해제으로 수정
            inventoryManager.CalOnTextChangeUnEquipEvent();
        }
        else
        {
            Debug.Log("장비를 해제합니다.");
            itemDics[(int)displayType][clickSlotIndex].IsEquip = false;
            inventoryManager.CallUnEquipItemEvent(itemDics[(int)displayType][clickSlotIndex]);
            slotList[clickSlotIndex].UnDisplayEquip();
            //사용 버튼의 텍스트를 장비착용으로 수정
            inventoryManager.CallOnTextChangeEquipEvent();
        }

    }

    public void Drop()
    {
        TryAddItem(itemDics[(int)DisplayType][inventoryManager.ClickSlotIndex].ID, -1);
        itemManager.Drop(itemDics[(int)DisplayType][inventoryManager.ClickSlotIndex], 1, this.transform.position);
    }

    public void SetDisplayType(ItemType itemType)
    {
        DisplaySlotClear();
        displayType = itemType;
        OnDisplaySlot();
    }
    private void OnDisplaySlot()
    {
        //AddItem으로 초기화된 또는 변경된 슬롯에 값을 넣어줘야됨
        foreach (KeyValuePair<int, Item> item in itemDics[(int)displayType])
            slotList[item.Key].AddItem(item.Value);
    }
    private void DisplaySlotAllClear()
    {
        for (int i = 0; i < slotMaxCount; i++)
            slotList[i].ClearSlot();
    }
    private void DisplaySlotClear()
    {
        foreach (KeyValuePair<int, Item> item in itemDics[(int)displayType])
            slotList[item.Key].ClearSlot();
    }

    public void ChangeSlot(int dropSlotIndex, int dragSlotIndex)
    {
        Item itemObject;
        if (itemDics[(int)displayType].ContainsKey(dropSlotIndex))
        {
            itemObject = itemDics[(int)displayType][dropSlotIndex];
            itemDics[(int)displayType][dropSlotIndex] = itemDics[(int)displayType][dragSlotIndex];
            itemDics[(int)displayType][dragSlotIndex] = itemObject;
        }
        else
        {
            itemDics[(int)displayType].Add(dropSlotIndex, itemDics[(int)displayType][dragSlotIndex]);
            itemDics[(int)displayType].Remove(dragSlotIndex);
        }
        DisplaySlotAllClear();
        OnDisplaySlot();
    }

    public bool IsCheckItem(int id, int count, out int itemSum)
    {
        bool isItemCount = false;
        itemSum = 0;
        if (itemDB.GetItemData(id, out ItemData newItem))
            isItemCount = IsCheckItemCount(itemDics[(id / 100000) % 10], id, count, ref itemSum);
        return isItemCount;
    }

    public bool[] IsCheckItems(int[] id, int[] count, out int[] itemSum)
    {
        bool isItemCount = false;
        int idLenght = id.Length;
        itemSum = new int[idLenght];
        bool[] boolArray = new bool[idLenght];

        for (int i = 0; i < idLenght; i++)
        {
            if (itemDB.GetItemData(id[i], out ItemData newItem))
                isItemCount = IsCheckItemCount(itemDics[(id[i] / 100000) % 10], id[i], count[i], ref itemSum[i]);
            boolArray[i] = isItemCount;
        }
        return boolArray;
    }

    /// <summary>
    /// 제작용
    /// </summary>
    /// <param name="newRecipe"></param>
    /// <param name="itemSum">재료의 갯수배열 만큼 반환합니다.</param>
    /// <returns></returns>
    public Sprite[] IsCheckItems(in ItemRecipe newRecipe, out int[] itemSum)
    {
        int idLenght = newRecipe.Materials.Length;
        int[] materials = newRecipe.Materials;
        int[] materialsCount = newRecipe.MaterialsCount;
        itemSum = new int[idLenght];
        Sprite[] icons = new Sprite[idLenght + 1];
        itemDB.GetImage(newRecipe.CraftingID, out icons[0]);
        for (int i = 0; i < idLenght; i++)
        {
            if (itemDB.GetItemData(materials[i], out ItemData newItem))
            {
                IsCheckItemCount(itemDics[(materials[i] / 100000) % 10], materials[i], materialsCount[i], ref itemSum[i]);
                icons[i + 1] = newItem.Icon;
            }
        }
        return icons;
    }

    private bool IsCheckItemCount(Dictionary<int, Item> itemDic, int id, int count, ref int sum)
    {
        foreach (KeyValuePair<int, Item> i in itemDic)
        {
            if (i.Value.ID == id)
                sum += i.Value.Count;
        }
        return sum >= count ? true : false;
    }
    
    /// <summary>
    /// 갯수만큼 있는지 확인
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool IsCheckItem(int id, int count)
    {
        bool isItemCount = false;
        if (itemDB.GetItemData(id, out ItemData newItem))
            isItemCount = IsCheckItemCount(itemDics[(id / 100000) % 10], id, count);
        return isItemCount;
    }
    /// <summary>
    /// 갯수만큼 있는지 확인
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool[] IsCheckItems(int[] id, int[] count)
    {
        bool isItemCount = false;
        int idLenght = id.Length;
        bool[] boolArray = new bool[idLenght];

        for (int i = 0; i < idLenght; i++)
        {
            if (itemDB.GetItemData(id[i], out ItemData newItem))
                isItemCount = IsCheckItemCount(itemDics[(id[i] / 100000) % 10], id[i], count[i]);
            boolArray[i] = isItemCount;
        }
        return boolArray;
    }

    public Sprite[] IsCheckItems(in ItemRecipe newRecipe)
    {
        int idLenght = newRecipe.Materials.Length;
        int[] materials = newRecipe.Materials;
        int[] materialsCount = newRecipe.MaterialsCount;
        Sprite[] icons = new Sprite[idLenght + 1];
        itemDB.GetImage(newRecipe.CraftingID, out icons[0]);
        for (int i = 0; i < idLenght; i++)
        {
            if (itemDB.GetItemData(materials[i], out ItemData newItem))
            {
                IsCheckItemCount(itemDics[(materials[i] / 100000) % 10], materials[i], materialsCount[i]);
                icons[i + 1] = newItem.Icon;
            }
        }
        return icons;
    }

    private bool IsCheckItemCount(Dictionary<int, Item> itemDic, int id, int count)
    {
        int listCount = itemDic.Count;
        int sum = 0;
        foreach (KeyValuePair<int, Item> i in itemDic)
        {
            if (i.Value.ID == id)
                sum += i.Value.Count;
        }
        return sum >= count ? true : false;
    }

    /// <summary>
    /// 갯수만큼 넣을수있는가?
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool IsCheckSpaces(int[] id, int[] count)
    {
        int idLength = id.Length;
        if (idLength != count.Length)
            return false;
        bool isTrue = true;
        for (int i = 0; i < idLength; i++)
        {
            if(!IsCheckSpace(id[i], count[i]))
                isTrue = false;
        }
        return isTrue;
    }
    /// <summary>
    /// 갯수만큼 넣을수있는가?
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count">양수만 입력해주세요</param>
    /// <returns></returns>
    public bool IsCheckSpace(int id, int count)
    {
        if(count <= 0)
        {
            Debug.LogError("양수만 입력해주세요");
            return false;
        }

        if (itemDB.GetItemData(id, out ItemData newItem))
        {
            int addSlotCount = count / newItem.MaxCount;
            int remainder = count % newItem.MaxCount;
            if(remainder > 0)
            {
                addSlotCount++;
                int nowItemCount = CheckItemCount(id);
                if (nowItemCount > 0)
                    if ((nowItemCount % newItem.MaxCount) + remainder <= newItem.MaxCount)
                        addSlotCount--;
            }
            if (itemDics[(id / 100000) % 10].Count + addSlotCount > slotMaxCount)
                    return false;
        }
        return true;
    }
    /// <summary>
    /// 몇개있는지 확인
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int CheckItemCount(int id)
    {
        int itemSum = 0;
        if (itemDB.GetItemData(id, out ItemData newItem))
            itemSum = ItemCount(itemDics[(id / 100000) % 10], id);
        return itemSum;
    }
    /// <summary>
    /// 몇개있는지 확인
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int[] CheckItemsCount(int[] id)
    {
        int idLenght = id.Length;
        int[] itemSum = new int[idLenght];
        for (int i = 0; i < idLenght; i++)
        {
            if (itemDB.GetItemData(id[i], out ItemData newItem))
                itemSum[i] = ItemCount(itemDics[(id[i] / 100000) % 10], id[i]);
        }
        return itemSum;
    }

    private int ItemCount(Dictionary<int, Item> itemDic, int id)
    {
        int sum = 0;
        foreach (KeyValuePair<int, Item> i in itemDic)
        {
            if (i.Value.ID == id)
                sum += i.Value.Count;
        }
        return sum;
    }
}