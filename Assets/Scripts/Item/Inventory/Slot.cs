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
    [SerializeField] private GameObject _outLine;

    //슬롯 데이터 저장
    private Image _item2DImage;
    private ItemData_Test _itemData;
    private ItemSlotInfo _slotInfo;
    private ItemType _itemType;

    //클릭을 위한 버튼
    private Button _button;

    //클릭시 설명창을 띄우기위해 캐싱
    private GameObject _itemExplanationPopup;
    private TMP_Text[] _itemText;
    
    //드래그 앤 드롭 구현을 위해 필요한 변수들
    private Transform _itemImageTransform;
    
    private Vector3 defaultPos; // 복귀를 위한 위치 저장
    private GameObject _inventory_UI; // UI맨앞으로 바꾸기 위해 인벤토리 저장
    private Transform _startParent; // 복귀를 위한 transform값 저장
    
    //드래그가 유효한지 저장하기위한 bool값
    private bool isData;

    // 아이템 정보를 받기위해 DB캐싱
    private ItemDB _itemDB;

    //슬롯의 위치 한번 설정한 후 절대 바뀌지 않을 값
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
    }

    private void Start()
    {
        _button.onClick.AddListener(SlotClick);
        _itemExplanationPopup = InventoryManager.Instance.ItemExplanationPopup;
        _itemText = _itemExplanationPopup.transform.GetComponentsInChildren<TMP_Text>();
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(ItemData_Test itemData, int index, int count)
    {
        _slotInfo = new ItemSlotInfo();
        _itemData = itemData;
        _slotInfo.id = itemData.ID;
        _slotInfo.index = index;
        _slotInfo.count = count;
        CheckItemType(_slotInfo.id);
        SlotDisplay();
    }

    public void AddItem(ItemSlotInfo slotInfo)
    {
        if(_itemDB.GetItemData(slotInfo.id, out _itemData))
        {
            _slotInfo = slotInfo;
            CheckItemType(_slotInfo.id);
            SlotDisplay();
        }
        else
        {
            ClearSlot();
        }
        
    }

    private void CheckItemType(int id)
    {
        if (id >= 10300000)
            _itemType = ItemType.ETC;
        else if (id >= 10200000)
            _itemType = ItemType.Material;
        else if (id >= 10100000)
            _itemType = ItemType.Consumable;
        else
            _itemType = ItemType.Equipment;
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
        TurnOffItemClick();
        _text_Count.text = "";
        _text_Count.gameObject.SetActive(false);
    }

    public void SlotDisplay()
    {
        _item2DImage.enabled = true;
        _item2DImage.sprite = _itemData.Icon;
        _button.enabled = true;

        if (_itemType != ItemType.Equipment)
        {
            _text_Count.gameObject.SetActive(true);
            _text_Count.text = String.Format("x {0}", _slotInfo.count);
        }
    }

    public void SlotClick()
    {
        SetActiveItemExplanationPopup(true, _itemData);

        //클릭한 정보 매니저한태 전달
        InventoryManager.Instance.SetClickItem(_slotInfo, this);

        //클릭한 모습 표시
        DisplayItemClick();

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

    public void SetActiveItemExplanationPopup(bool isActive, ItemData_Test itemData)
    {
        _itemExplanationPopup.SetActive(isActive);
        _itemText[0].text = itemData.ItemName;
        _itemText[1].text = "테스트로 직접 입력";
        _itemText[2].text = itemData.ItemExplanation;
    }

    public void DisplayItemClick()
    {
        _outLine.SetActive(true);
    }

    public void TurnOffItemClick()
    {
        _outLine.SetActive(false);
    }

    public void DisplayEquip()
    {
        //장착 UI표시
    }
}
