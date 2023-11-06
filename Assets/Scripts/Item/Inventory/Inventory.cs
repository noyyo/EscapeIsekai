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
    //���� ����
    [Header("���� ����")]
    [SerializeField] private int _inventroySlotCount = 60;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private GameObject _slotSpawn;

    private UI_Inventory _ui_Inventory;
    private InventoryManager _inventoryManager;

    //���Ե��� ��� ����Ʈ
    private List<Slot> _slotArray = new List<Slot>();

    //���� ���Ե鿡 ���� ����������, ����, ������ġ(Slot�� Index)��� ����
    private List<ItemSlotInfo> _equipmentItemList = new List<ItemSlotInfo>();
    private List<ItemSlotInfo> _consumableItemList = new List<ItemSlotInfo>();
    private List<ItemSlotInfo> _materialItemList = new List<ItemSlotInfo>();
    private List<ItemSlotInfo> _etcItemList = new List<ItemSlotInfo>();

    private ItemDB _itemDB;

    //���, �Һ�, ���, ��Ÿ�� ������ ������ ������
    //private int[] _itemCount;
    //���� ���õ� ī�װ��� ������
    private ItemType _displayType;
    //���� ���õ� �������� ������ ������
    private ItemSlotInfo _clickItem;

    //Inventory_UI �¿����� �̺�Ʈ
    public event Action OnInventoryDisplayEvent;

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        _inventoryManager = InventoryManager.Instance;
        InitInventory();
        CreateSlot();
    }

    /// <summary>
    /// �ʱ�ȭ
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
            _slotSpawn = InventoryManager.Instance.Inventory_UI.transform.GetChild(4).GetChild(0).GetChild(0).gameObject;
    }

    /// <summary>
    /// ���� ����
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

    private void Start()
    {
        DisplaySlotAllClear();
    }

    public void OnInventory()
    {
        OnInventoryDisplayEvent?.Invoke();
        OnDisplaySlot();
    }

    /// <summary>
    /// �ܺο��� ȣ���Ͽ� �������� �ְų� ���ݴϴ�.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count">������ ����</param>
    /// <param name="errorItemCount">Slot�� ������ �ʰ��Ͽ� ���� �������� ������ ��ȯ�մϴ�. count�� �����϶� �������� ���� ��ȯ</param>
    /// <returns></returns>
    public bool TryAddItem(int id, int count, out int errorItemCount)
    {
        errorItemCount = count;
        bool isAddItem = false;
        if (_itemDB.GetItemData(id, out ItemData_Test newItem))
        {
            switch (newItem.ItemType)
            {
                case ItemType.Equipment:
                    if (count >= 0)
                    {
                        if (AddList(_equipmentItemList, count, (int)ItemType.Equipment, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    else
                    {
                        if (SubList(_equipmentItemList, count, (int)ItemType.Equipment, in newItem, out errorItemCount))
                            isAddItem = true;
                    }
                    break;
                case ItemType.Consumable:
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
                case ItemType.Material:
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
                default:
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
            }
        }

        return isAddItem;
    }

    /// <summary>
    /// ���������� ���ϴ� �޼���
    /// </summary>
    /// <param name="itemList">�������� id, ����, slot��ġ���� ����� ����Ʈ</param>
    /// <param name="count">������ ����</param>
    /// <param name="slotType">�������� Ÿ��</param>
    /// <param name="newItem">������ ����</param>
    /// <param name="errorItemCount">�ʰ��� ������ ����</param>
    /// <returns></returns>
    private bool AddList(List<ItemSlotInfo> itemList, int count, ItemType slotType, in ItemData_Test newItem, out int errorItemCount)
    {
        //â�� ����á���� Ȯ��
        int itemListCount = itemList.Count;
        if (itemListCount == _inventroySlotCount)
        {
            errorItemCount = count;
            return false;
        }

        errorItemCount = 0;
        int id = newItem.ID;
        bool isDisplay = false;

        if (newItem.ItemType == _displayType)
        {
            isDisplay = true;
        }

        // �������� ����ΰ�?
        if (slotType != ItemType.Equipment)
        {
            //�� â���� ���� �����°��ΰ�?
            if (itemListCount == 0)
            {
                // ���� ������ ��ǰ�� �� ���Դ� �������� ������?
                if (count > newItem.MaxCount)
                {
                    // ���� for������ ���� ����
                    while (true)
                    {
                        count -= newItem.MaxCount;

                        if (count <= 0)
                        {
                            //������ �ý����̿��� Slot�� ������ ������ ����Ʈ�� ���� �� �����ص� �������
                            //Slot�� ���� �� ǥ��
                            if (isDisplay)
                                _slotArray[itemListCount].AddItem(newItem, itemListCount, newItem.MaxCount + count);

                            //����
                            itemList.Add(new ItemSlotInfo(id, itemListCount, newItem.MaxCount + count));
                            itemListCount++;
                            break;
                        }
                        else
                        {
                            //Slot�� ���� �� ǥ��
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
                        //ĳ��
                        int newItemMaxCount = newItem.MaxCount;
                        int nowItemCount = itemList[i].count;

                        //���罽���� ���� á���� Ȯ��
                        if (nowItemCount == newItemMaxCount)
                        {
                            if (nowItemCount + count > newItemMaxCount)
                            {
                                //������ ���� ���
                                count = count - (newItemMaxCount - nowItemCount);
                                itemList[i].count = newItemMaxCount;

                                //Slot�� ���� �� ���
                                if (isDisplay)
                                    _slotArray[i].SetSlotCount(newItemMaxCount);

                                while (true)
                                {
                                    // ��
                                    count -= newItemMaxCount;
                                    if (count <= 0) break;

                                    //���
                                    if (isDisplay)
                                        _slotArray[itemListCount].AddItem(newItem, itemListCount, newItemMaxCount);

                                    //����
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
        //������ �ִ밡 1��
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
    /// �������� �Ҹ��ϴ� �޼���
    /// </summary>
    /// <param name="itemList">�������� id, ����, slot��ġ���� ����� ����Ʈ</param>
    /// <param name="count">������ ����(����)</param>
    /// <param name="slotType">�������� Ÿ��</param>
    /// <param name="newItem">������ ����</param>
    /// <param name="ErrorItemCount">���̻� �Ҹ��� �� ���� ������ ����</param>
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

        if (newItem.ItemType == _displayType)
            isDisplay = true;

        for (int i = 0; i < itemListCount; i++)
        {
            if (itemList[i].id == id)
            {
                int nowItemCount = itemList[i].count + count;

                // ���� ���ߵ� ���� ������
                if (nowItemCount < 0)
                {
                    count = nowItemCount;
                    stack.Push(i);
                    continue;
                }

                count = 0;
                // ��δ� ���������� �Q����
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

    //UI�κ����� �Űܾߵ�...
    public void SetDisplayType(ItemType itemType)
    {
        DisplaySlotClear();
        _displayType = itemType;
        OnDisplaySlot();
    }

    //UI�κ����� �Űܾߵ�...
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

    //UI�κ����� �Űܾߵ�...
    private void DisplaySlotAllClear()
    {
        for (int i = 0; i < _inventroySlotCount; i++)
        {
            _slotArray[i].ClearSlot();
        }

    }
    //UI�κ����� �Űܾߵ�...
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
            //����
            EquipItem(_clickItem);
        }
        else
        {
            TryAddItem(_clickItem.id, -1, out int errorCount);
        }
    }

    /// <summary>
    /// id�� count ���� �̻����� ������ true�� �ƴҽ� false�� ��ȯ�մϴ�.
    /// sum�� �ش� id�� ������ ��ȯ�մϴ�. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <param name="sum"></param>
    /// <returns></returns>
    public bool IsCheckItem(int id, int count, out int sum)
    {
        bool isItemCount = false;
        sum = 0;
        if (_itemDB.GetItemData(id, out ItemData_Test newItem))
        {
            switch (newItem.ItemType)
            {
                case ItemType.Equipment:
                    isItemCount = IsCheckItemCount(_equipmentItemList, newItem, count, ref sum);
                    break;
                case ItemType.Consumable:
                    isItemCount = IsCheckItemCount(_consumableItemList, newItem, count, ref sum);
                    break;
                case ItemType.Material:
                    isItemCount = IsCheckItemCount(_materialItemList, newItem, count, ref sum);
                    break;
                default:
                    isItemCount = IsCheckItemCount(_etcItemList, newItem, count, ref sum);
                    break;
            }
        }
        return isItemCount;
    }

    /// <summary>
    /// id[i] �� count[i] ���� �̻����� ������ true�� �ƴҽ� false�� bool�迭�� ��� ��ȯ�մϴ�.
    /// sum[i]�� �ش� id[i]�� ������ ��ȯ�մϴ�. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <param name="sum"></param>
    /// <returns></returns>
    public bool[] IsCheckItems(int[] id, int[] count, out int[] sum)
    {
        bool isItemCount = false;
        int idLenght = id.Length;
        sum = new int[idLenght];
        bool[] boolArray = new bool[idLenght];

        for (int i  = 0; i < idLenght; i++)
        {
            if (_itemDB.GetItemData(id[i], out ItemData_Test newItem))
            {
                switch (newItem.ItemType)
                {
                    case ItemType.Equipment:
                        isItemCount = IsCheckItemCount(_equipmentItemList, newItem, count[i], ref sum[i]);
                        break;
                    case ItemType.Consumable:
                        isItemCount = IsCheckItemCount(_consumableItemList, newItem, count[i], ref sum[i]);
                        break;
                    case ItemType.Material:
                        isItemCount = IsCheckItemCount(_materialItemList, newItem, count[i], ref sum[i]);
                        break;
                    default:
                        isItemCount = IsCheckItemCount(_etcItemList, newItem, count[i], ref sum[i]);
                        break;
                }
            }
            boolArray[i] = isItemCount;
        }
        return boolArray;
    }

    /// <summary>
    /// IsCheckItem�� ���α���� �����ϴ� �޼���
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

    //���߿� ��� ����� UI�� ���ݵ��� ��������ߵ�
    private void EquipItem(ItemSlotInfo itemSlotInfo)
    {
        _itemDB.GetItemData(itemSlotInfo.id, out ItemData_Test equipItemData);

        //���߿� �÷��̾�� �����Ҷ� �ۼ��ؾ��� �ڵ�
        Debug.Log("��� �����մϴ�. ������� �÷��̾ ����");

        itemSlotInfo.equip = true;

        _slotArray[itemSlotInfo.index].DisplayEquip();
    }

    public void Drop()
    {
        _clickItem = InventoryManager.Instance.GetClickItem();

        _itemDB.GetItemData(_clickItem.id, out ItemData_Test newItem);
        Instantiate(newItem.DropPrefab);

        TryAddItem(_clickItem.id, -1, out int errorCount);
    }

    //Drag And Drop���� ������ ��ȯ�ϱ� ���� �޼���
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