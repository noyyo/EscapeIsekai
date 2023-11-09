using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSlotInfo
{
    public int id;
    public int count;
    public int index;
    public bool equip;

    public ItemSlotInfo(int newID, int newindex, int newCount = 1)
    {
        id = newID;
        count = newCount;
        index = newindex;
    }
    public ItemSlotInfo() { }
}

public class Inventory : MonoBehaviour
{
    //슬롯 관련
    [Header("슬롯 설정")]
    [SerializeField] private int _inventroySlotCount = 60;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private GameObject _slotSpawn;

    private UI_Inventory _ui_Inventory;
    private InventoryManager _inventoryManager;

    //슬롯들을 담는 리스트
    private List<Slot> _slotArray = new List<Slot>();

    //실제 슬롯들에 무슨 아이템인지, 갯수, 저장위치(Slot의 Index)등등 보관
    private List<ItemSlotInfo> _equipmentItemList = new List<ItemSlotInfo>();
    private List<ItemSlotInfo> _consumableItemList = new List<ItemSlotInfo>();
    private List<ItemSlotInfo> _materialItemList = new List<ItemSlotInfo>();
    private List<ItemSlotInfo> _etcItemList = new List<ItemSlotInfo>();

    private ItemDB _itemDB;

    //현재 선택된 카테고리를 저장함
    private ItemType _displayType;
    //현재 선택된 아이템의 정보를 저장함
    private ItemSlotInfo _clickItem;

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        _inventoryManager = InventoryManager.Instance;
        InitInventory();
        CreateSlot();
    }

    private void Start()
    {
        DisplaySlotAllClear();
        _inventoryManager.OnInventoryDisplayEvent += OnDisplaySlot;
    }

    /// <summary>
    /// 초기화
    /// </summary>
    private void InitInventory()
    {
        _equipmentItemList.Clear();
        _consumableItemList.Clear();
        _materialItemList.Clear();
        _etcItemList.Clear();

        _slotArray.Clear();
        _displayType = ItemType.Equipment;

        if (_slotPrefab == null)
            _slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/Slot");

        if (_ui_Inventory == null)
            _ui_Inventory = GetComponent<UI_Inventory>();

        if (_slotSpawn == null)
        {
            _slotSpawn = InventoryManager.Instance.Inventory_UI.transform.GetChild(4).GetChild(0).GetChild(0).gameObject;
        }
            
    }

    /// <summary>
    /// 슬롯 생성
    /// </summary>
    private void CreateSlot()
    {
        for (int i = 0; i < _inventroySlotCount; i++)
        {
            GameObject obj = Instantiate(_slotPrefab);
            obj.transform.SetParent(_slotSpawn.transform, false);
            obj.GetComponent<Slot>().UniqueIndex = i;
            _slotArray.Add(obj.GetComponent<Slot>());
        }
    }

    //플레이어의 코드를 참조하여 I를 눌렸을시 _inventoryManager.CallOnInventoryDisplayEvent(); 호출해야 됩니다.
    // 인벤토리와 제작창 호출시 마우스 lock해제시켜야됩니다.
    // 인벤토리와 제작창 호출시 시간을 멈춰야 됩니다.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnInventory();
        }
    }

    public void OnInventory()
    {
        _inventoryManager.CallOnInventoryDisplayEvent();
    }
    //


    public bool[] TryAddItems(int[] id, int[] count, out int[] errorItemCount)
    {
        int idCount = id.Length;
        bool[] boolArray = new bool[idCount];
        errorItemCount = new int[idCount];
        for (int i = 0; i < idCount; i++)
        {
            boolArray[i] = TryAddItem(id[i], count[i], out errorItemCount[i]);
        }
        return boolArray;
    }

    public bool[] TryAddItems(ItemRecipe itemRecipe, out int[] errorItemCounts)
    {
        int idCount = itemRecipe.Materials.Length;
        bool[] boolArray = new bool[idCount];
        bool isCraftingItem = true;
        errorItemCounts = new int[idCount];
        for (int i = 0; i < idCount; i++)
        {
            boolArray[i] = IsCheckItem(itemRecipe.Materials[i], itemRecipe.MaterialsCount[i], out int sum);
            if (!boolArray[i])
            {
                isCraftingItem = false;
            }
        }
        if (isCraftingItem)
        {
            for (int i = 0; i < idCount; i++)
            {
                TryAddItem(itemRecipe.Materials[i], -(itemRecipe.MaterialsCount[i]), out errorItemCounts[i]);
            }
            TryAddItem(itemRecipe.CraftingID, 1, out int errorItemCount);
        }

        return boolArray;
    }

    /// <summary>
    /// 외부에서 호출하여 아이템을 넣거나 빼줍니다.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count">아이템 개수</param>
    /// <param name="errorItemCount">Slot의 개수를 초과하여 들어온 아이템의 갯수를 반환합니다. count가 음수일때 오류나면 음수 반환</param>
    /// <returns></returns>
    public bool TryAddItem(int id, int count, out int errorItemCount)
    {
        errorItemCount = count;
        bool isAddItem = false;
        if (_itemDB.GetItemData(id, out ItemData_Test newItem))
        {
            switch (id)
            {
                case >= 10300000:
                    if (count >= 0)
                    {
                        if (AddList(_etcItemList, count, ItemType.ETC, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_etcItemList, count, ItemType.ETC, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    break;
                case >= 10200000:
                    if (count >= 0)
                    {
                        if (AddList(_materialItemList, count, ItemType.Material, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_materialItemList, count, ItemType.Material, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    break;
                case >= 10100000:
                    if (count >= 0)
                    {
                        if (AddList(_consumableItemList, count, ItemType.Consumable, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_consumableItemList, count, ItemType.Consumable, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    break;
                default:
                    if (count >= 0)
                    {
                        if (AddList(_equipmentItemList, count, ItemType.Equipment, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_equipmentItemList, count, ItemType.Equipment, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    break;
            }
        }
        return isAddItem;
    }

    /// <summary>
    /// 내부적으로 더하는 메서드
    /// </summary>
    /// <param name="itemList">아이템의 id, 개수, slot위치등이 저장된 리스트</param>
    /// <param name="count">아이템 개수</param>
    /// <param name="slotType">아이템의 타입</param>
    /// <param name="newItem">아이템 정보</param>
    /// <param name="errorItemCount">초과된 아이템 개수</param>
    /// <returns></returns>
    private bool AddList(List<ItemSlotInfo> itemList, int count, ItemType slotType, in ItemData_Test newItem, out int errorItemCount)
    {
        //창고가 가득찼는지 확인
        int itemListCount = itemList.Count;
        if (itemListCount == _inventroySlotCount)
        {
            errorItemCount = count;
            return false;
        }

        errorItemCount = 0;
        int id = newItem.ID;
        bool isDisplay = false;

        if (slotType == _displayType)
        {
            isDisplay = true;
        }

        // 아이템이 장비인가?
        if (slotType != ItemType.Equipment)
        {
            //빈 창고에서 새로 들어오는것인가?
            if (itemListCount == 0)
            {
                // 새로 들어오는 제품이 한 슬롯당 갯수보다 높은가?
                if (count > newItem.MaxCount)
                {
                    // 무한 for문으로 갯수 생성
                    while (true)
                    {
                        count -= newItem.MaxCount;

                        if (count <= 0)
                        {
                            //별도의 시스템이여서 Slot에 데이터 보내고 리스트에 생성 및 저장해도 상관없음
                            //Slot에 저장 및 표시
                            if (isDisplay)
                                _slotArray[itemListCount].AddItem(newItem, itemListCount, newItem.MaxCount + count);

                            //생성
                            itemList.Add(new ItemSlotInfo(id, itemListCount, newItem.MaxCount + count));
                            itemListCount++;
                            break;
                        }
                        else
                        {
                            //Slot에 저장 및 표시
                            if (isDisplay)
                                _slotArray[itemListCount].AddItem(newItem, itemListCount, newItem.MaxCount);

                            itemList.Add(new ItemSlotInfo(id, itemListCount, newItem.MaxCount));
                            itemListCount++;
                        }
                    }
                }
                else
                {
                    if (isDisplay)
                        _slotArray[0].AddItem(newItem, 0, count);

                    itemList.Add(new ItemSlotInfo(id, 0, count));
                    itemListCount++;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < itemListCount; i++)
                {
                    if (itemList[i].id == id)
                    {
                        //캐싱
                        int newItemMaxCount = newItem.MaxCount;
                        int nowItemCount = itemList[i].count;

                        //현재슬롯이 가득 찼는지 확인
                        if (nowItemCount == newItemMaxCount)
                        {
                            if (nowItemCount + count > newItemMaxCount)
                            {
                                //아이템 개수 계산
                                count = count - (newItemMaxCount - nowItemCount);
                                itemList[i].count = newItemMaxCount;

                                //Slot에 저장 및 출력
                                if (isDisplay)
                                    _slotArray[i].SetSlotCount(newItemMaxCount);

                                while (true)
                                {
                                    // 값
                                    count -= newItemMaxCount;
                                    if (count <= 0) break;

                                    //출력
                                    if (isDisplay)
                                        _slotArray[itemListCount].AddItem(newItem, itemListCount, newItemMaxCount);

                                    //생성
                                    itemList.Add(new ItemSlotInfo(id, itemListCount, newItemMaxCount));
                                    itemListCount++;
                                    if (itemListCount == _inventroySlotCount)
                                    {
                                        errorItemCount = count;
                                        return false;
                                    }
                                }
                                itemList.Add(new ItemSlotInfo(id, itemListCount, newItemMaxCount + count));
                                itemListCount++;
                                if (isDisplay)
                                    _slotArray[itemListCount - 1].AddItem(newItem, itemListCount - 1, newItemMaxCount + count);
                            }
                            else
                            {
                                itemList[i].count += count;

                                if (isDisplay)
                                    _slotArray[i].SetSlotCount(itemList[i].count);
                            }
                        }
                    }
                }
                return true;
            }
        }
        //장비류는 최대가 1개
        for (int i = 0; i < count; i++)
        {
            itemList.Add(new ItemSlotInfo(id, itemListCount, 1));
            itemListCount++;
            if (isDisplay)
                _slotArray[(itemListCount - 1)].AddItem(newItem, (itemListCount - 1), 1);

            if (itemListCount == _inventroySlotCount)
            {
                errorItemCount = count - (i + 1);
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 아이템을 소모하는 메서드
    /// </summary>
    /// <param name="itemList">아이템의 id, 개수, slot위치등이 저장된 리스트</param>
    /// <param name="count">아이템 개수(음수)</param>
    /// <param name="slotType">아이템의 타입</param>
    /// <param name="newItem">아이템 정보</param>
    /// <param name="ErrorItemCount">더이상 소모할 수 없는 아이템 개수</param>
    /// <returns></returns>
    private bool SubList(List<ItemSlotInfo> itemList, int count, ItemType slotType, in ItemData_Test newItem, out int ErrorItemCount)
    {
        int itemListCount = itemList.Count;
        if (itemListCount == 0)
        {
            ErrorItemCount = count;
            return false;
        }

        bool isSub = false;
        int id = newItem.ID;
        Stack stack = new Stack();

        bool isDisplay = false;

        if (slotType == _displayType)
            isDisplay = true;

        for (int i = 0; i < itemListCount; i++)
        {
            if (itemList[i].id == id)
            {
                int nowItemCount = itemList[i].count + count;

                // 아직 빼야될 값이 있을때
                if (nowItemCount <= 0)
                {
                    count = nowItemCount;
                    stack.Push(i);
                    if(nowItemCount != 0)
                        continue;
                }

                count = 0;
                // 모두다 정상적으로 뻈을때
                if (isDisplay)
                    _slotArray[i].SetSlotCount(nowItemCount);

                for (int j = 0; j < stack.Count;)
                {
                    int n = (int)stack.Pop();
                    _slotArray[n].ClearSlot();
                    itemList.RemoveAt(n);
                    itemListCount--;
                }

                isSub = true;
                break;
            }
        }
        ErrorItemCount = count;
        return isSub;
    }

    public void SortInventory()
    {
        DisplaySlotAllClear();
        _equipmentItemList.Sort((n1, n2) => n1.id.CompareTo(n2.id));
        int maxIndex = _equipmentItemList.Count;
        for (int i = 0; i < maxIndex; i++)
        {
            _equipmentItemList[i].index = i;
        }

        _consumableItemList.Sort((n1, n2) => n1.id.CompareTo(n2.id));
        maxIndex = _consumableItemList.Count;
        for (int i = 0; i < maxIndex; i++)
        {
            _consumableItemList[i].index = i;
        }

        _materialItemList.Sort((n1, n2) => n1.id.CompareTo(n2.id));
        maxIndex = _materialItemList.Count;
        for (int i = 0; i < maxIndex; i++)
        {
            _materialItemList[i].index = i;
        }

        _etcItemList.Sort((n1, n2) => n1.id.CompareTo(n2.id));
        maxIndex = _etcItemList.Count;
        for (int i = 0; i < maxIndex; i++)
        {
            _etcItemList[i].index = i;
        }
        OnDisplaySlot();
    }

    //UI부분으로 옮겨야됨...
    public void SetDisplayType(ItemType itemType)
    {
        DisplaySlotClear();
        _displayType = itemType;
        OnDisplaySlot();
    }

    //UI부분으로 옮겨야됨...
    private void OnDisplaySlot()
    {
        switch (_displayType)
        {
            case ItemType.Equipment:
                foreach (ItemSlotInfo item in _equipmentItemList)
                {
                    _slotArray[item.index].AddItem(item);
                }
                break;
            case ItemType.Consumable:
                foreach (ItemSlotInfo item in _consumableItemList)
                {
                    _slotArray[item.index].AddItem(item);
                }
                break;
            case ItemType.Material:
                foreach (ItemSlotInfo item in _materialItemList)
                {
                    _slotArray[item.index].AddItem(item);
                }
                break;
            default:
                foreach (ItemSlotInfo item in _etcItemList)
                {
                    _slotArray[item.index].AddItem(item);
                }
                break;
        }
    }

    //UI부분으로 옮겨야됨...
    private void DisplaySlotAllClear()
    {
        for (int i = 0; i < _inventroySlotCount; i++)
        {
            _slotArray[i].ClearSlot();
        }

    }
    //UI부분으로 옮겨야됨...
    private void DisplaySlotClear()
    {
        switch (_displayType)
        {
            case ItemType.Equipment:
                foreach (ItemSlotInfo item in _equipmentItemList)
                {
                    _slotArray[item.index].ClearSlot();
                }
                break;
            case ItemType.Consumable:
                foreach (ItemSlotInfo item in _consumableItemList)
                {
                    _slotArray[item.index].ClearSlot();
                }
                break;
            case ItemType.Material:
                foreach (ItemSlotInfo item in _materialItemList)
                {
                    _slotArray[item.index].ClearSlot();
                }
                break;
            default:
                foreach (ItemSlotInfo item in _etcItemList)
                {
                    _slotArray[item.index].ClearSlot();
                }
                break;
        }
    }

    public void UseItem()
    {
        _clickItem = InventoryManager.Instance.GetClickItem();
        if (_displayType == ItemType.Equipment)
        {
            //장착
            EquipItem(_clickItem);
        }
        else
        {
            TryAddItem(_clickItem.id, -1, out int errorCount);
        }
    }

    /// <summary>
    /// id중 count 개수 이상으로 보유시 true를 아닐시 false를 반환합니다.
    /// sum은 해당 id의 개수를 반환합니다. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <param name="itemSum"></param>
    /// <returns></returns>
    public bool IsCheckItem(int id, int count, out int itemSum)
    {
        bool isItemCount = false;
        itemSum = 0;
        if (_itemDB.GetItemData(id, out ItemData_Test newItem))
        {
            switch (id)
            {
                case >= 10300000:
                    isItemCount = IsCheckItemCount(_etcItemList, newItem, count, ref itemSum);
                    break;
                case >= 10200000:
                    isItemCount = IsCheckItemCount(_materialItemList, newItem, count, ref itemSum);
                    break;
                case >= 10100000:
                    isItemCount = IsCheckItemCount(_consumableItemList, newItem, count, ref itemSum);
                    break;
                default:
                    isItemCount = IsCheckItemCount(_equipmentItemList, newItem, count, ref itemSum);
                    break;
            }
        }
        return isItemCount;
    }

    /// <summary>
    /// id[i] 중 count[i] 개수 이상으로 보유시 true를 아닐시 false를 bool배열에 담아 반환합니다.
    /// sum[i]은 해당 id[i]의 개수를 반환합니다. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <param name="itemSum"></param>
    /// <returns></returns>
    public bool[] IsCheckItems(int[] id, int[] count, out int[] itemSum)
    {
        bool isItemCount = false;
        int idLenght = id.Length;
        itemSum = new int[idLenght];
        bool[] boolArray = new bool[idLenght];

        for (int i  = 0; i < idLenght; i++)
        {
            if (_itemDB.GetItemData(id[i], out ItemData_Test newItem))
            {
                switch (id[i])
                {
                    case >= 10300000:
                        isItemCount = IsCheckItemCount(_etcItemList, newItem, count[i], ref itemSum[i]);
                        break;
                    case >= 10200000:
                        isItemCount = IsCheckItemCount(_materialItemList, newItem, count[i], ref itemSum[i]);
                        break;
                    case >= 10100000:
                        isItemCount = IsCheckItemCount(_consumableItemList, newItem, count[i], ref itemSum[i]);
                        break;
                    default:
                        isItemCount = IsCheckItemCount(_equipmentItemList, newItem, count[i], ref itemSum[i]);
                        break;
                }
            }
            boolArray[i] = isItemCount;
        }
        return boolArray;
    }

    public Sprite[] IsCheckItems(in ItemRecipe newRecipe, out int[] itemSum)
    {
        int idLenght = newRecipe.Materials.Length;
        int[] materials = newRecipe.Materials;
        int[] materialsCount = newRecipe.MaterialsCount;
        itemSum = new int[idLenght];
        Sprite[] icons = new Sprite[idLenght + 1];
        _itemDB.GetImage(newRecipe.CraftingID, out icons[0]);
        for (int i = 0; i < idLenght; i++)
        {
            if (_itemDB.GetItemData(materials[i], out ItemData_Test newItem))
            {
                switch (materials[i])
                {
                    case >= 10300000:
                        IsCheckItemCount(_etcItemList, newItem, materialsCount[i], ref itemSum[i]);
                        break;
                    case >= 10200000:
                        IsCheckItemCount(_materialItemList, newItem, materialsCount[i], ref itemSum[i]);
                        break;
                    case >= 10100000:
                        IsCheckItemCount(_consumableItemList, newItem, materialsCount[i], ref itemSum[i]);
                        break;
                    default:
                        IsCheckItemCount(_equipmentItemList, newItem, materialsCount[i], ref itemSum[i]);
                        break;
                }
                icons[i+1] = newItem.Icon;
            }
        }
        return icons;
    }

    /// <summary>
    /// IsCheckItem의 내부기능을 수행하는 메서드
    /// </summary>
    /// <param name="itemList"></param>
    /// <param name="newItem"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private bool IsCheckItemCount(List<ItemSlotInfo> itemList, ItemData_Test newItem, int count, ref int sum)
    {
        int listCount = itemList.Count;
        sum = 0;
        for (int i = 0; i < listCount; i++)
        {
            if (itemList[i].id == newItem.ID)
            {
                sum += itemList[i].count;
            }
        }
        return sum >= count ? true : false;
    }

    //나중에 장비 착용시 UI및 스텟등을 설정해줘야됨
    private void EquipItem(ItemSlotInfo itemSlotInfo)
    {
        if (!itemSlotInfo.equip)
        {
            _itemDB.GetStats(itemSlotInfo.id, out ItemStats equipItemData);

            //나중에 플레이어와 연결할때 작성해야할 코드
            Debug.Log("장비를 장착합니다. 플레이어 스텟을 수정합니다.");

            itemSlotInfo.equip = true;

            _slotArray[itemSlotInfo.index].DisplayEquip();
            _inventoryManager.CalOnTextChangeUnEquipEvent();
            
        }
        else
        {
            _itemDB.GetStats(itemSlotInfo.id, out ItemStats equipItemData);

            //나중에 플레이어와 연결할때 작성해야할 코드
            Debug.Log("장비를 해제합니다. 플레이어 스텟을 수정합니다.");

            itemSlotInfo.equip = false;
            _slotArray[itemSlotInfo.index].UnDisplayEquip();
            _inventoryManager.CallOnTextChangeEquipEvent();
        }
        
    }

    public void Drop()
    {
        _clickItem = InventoryManager.Instance.GetClickItem();

        _itemDB.GetItemData(_clickItem.id, out ItemData_Test newItem);
        Instantiate(newItem.DropPrefab);

        TryAddItem(_clickItem.id, -1, out int errorCount);
    }

    //Drag And Drop으로 아이템 전환하기 위한 메서드
    public void ChangeSlot(ItemSlotInfo newSlot, ItemSlotInfo oldSlot, int newIndex, int oldIndex)
    {
        if (newSlot == null)
        {
            oldSlot.index = newIndex;
            _slotArray[newIndex].AddItem(oldSlot);
            _slotArray[oldIndex].ClearSlot();
            return;
        }

        newSlot.index = oldIndex;
        oldSlot.index = newIndex;

        _slotArray[newIndex].AddItem(oldSlot);
        _slotArray[oldIndex].AddItem(newSlot);
    }
}