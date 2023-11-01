using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private GameObject _itemImage;
    [SerializeField] private TMP_Text _text_Count;
    private Transform _itemImageTransform;
    private Image _item2DImage;
    private ItemData_Test _itemData;
    private ItemSlotInfo _slotInfo;
    private ItemDB _itemDB;
    private Vector3 defaultPos;
    private GameObject _inventory_UI;
    private Transform _startParent;
    private Button _button;

    private UI_Slot _ui_Slot;
    private bool isData;

    private int _uniqueIndex = -1;
    public int UniqueIndex 
    {
        get { return _uniqueIndex; }
        set 
        { 
           if(_uniqueIndex == -1)
                _uniqueIndex = value;
        } 
    }

    public ItemData_Test ItemData { get { return _itemData; } }
    public ItemSlotInfo SlotInfo { get { return _slotInfo; } }

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        _slotInfo = new ItemSlotInfo();
        _item2DImage = _itemImage.GetComponent<Image>();
        _itemImageTransform = _itemImage.transform;
        _inventory_UI = InventoryManager.Instance.Inventory_UI;
        _button = GetComponent<Button>();
        _ui_Slot = GetComponent<UI_Slot>();
    }

    private void Start()
    {
        _button.onClick.AddListener(DisplayerItemExplanationPopup);
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(ItemData_Test itemData, int index, int count)
    {
        _slotInfo = new ItemSlotInfo();
        _itemData = itemData;
        _slotInfo.id = itemData.ID;
        _slotInfo.index = index;
        _slotInfo.count = count;
        SlotDisplay();
    }

    public void AddItem(ItemSlotInfo slotInfo)
    {
        if(_itemDB.GetItemData(slotInfo.id, out _itemData))
        {
            _slotInfo = slotInfo;
            SlotDisplay();
        }
        else
        {
            ClearSlot();
        }
        
    }

    /// <summary>
    /// 입력된 값(count)으로 변경 더하기나 빼기가 아닌 =입니다.
    /// </summary>
    /// <param name="count"></param>
    public void SetSlotCount(int count)
    {
        _slotInfo.count = count;
        _text_Count.text = String.Format("x {0}", _slotInfo.count);
    }

    // 해당 슬롯 하나 삭제
    public void ClearSlot()
    {
        _itemData = null;
        _slotInfo = null;
        _item2DImage.sprite = null;
        _item2DImage.enabled = false;
        _button.enabled = false;

        _text_Count.text = "";
        _text_Count.gameObject.SetActive(false);
    }

    public void SlotDisplay()
    {
        _item2DImage.enabled = true;
        _item2DImage.sprite = _itemData.Icon;
        _button.enabled = true;

        if (_itemData.ItemType != ItemType.Equipment)
        {
            _text_Count.gameObject.SetActive(true);
            _text_Count.text = String.Format("x {0}", _slotInfo.count);
        }
    }

    public void DisplayerItemExplanationPopup()
    {
        _ui_Slot.SetActiveItemExplanationPopup(true, _itemData);

        //클릭한 정보 매니저한태 전달
        InventoryManager.Instance.SetClickItem(_slotInfo, _ui_Slot);

        //클릭한 모습 표시
        _ui_Slot.DisplayItemClick();

        //클릭했을때 인벤토리 아래쪽에 있는 버튼 활성화
        InventoryManager.Instance.CallDisplayInventoryTailUI();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_slotInfo != null) 
        {
            isData = true;
            defaultPos = _itemImageTransform.position;
            _startParent = _itemImageTransform.parent;
            _itemImageTransform.SetParent(_inventory_UI.transform, false);
            //_itemImageTransform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(isData)
            _itemImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isData)
        {
            _itemImageTransform.SetParent(_startParent, false);
            _itemImageTransform.SetAsFirstSibling();
            _itemImage.transform.position = defaultPos;

            defaultPos = Vector3.zero;
            _startParent = null;
            isData = false;

            InventoryManager.Instance.CallChangeSlot(_slotInfo, _uniqueIndex);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryManager.Instance.SaveNewChangedSlot(_slotInfo, _uniqueIndex);
    }


    public void DisplayEquip()
    {
        _ui_Slot.DisPlayEquip();
    }
}
