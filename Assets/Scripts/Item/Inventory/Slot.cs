using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private GameObject _itemImage;
    [SerializeField] private TMP_Text _text_Count;
    [SerializeField] private GameObject _outLine;
    private Image _backGround;

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

    private InventoryManager _inventoryManager;
    private Color _defaultColor;
    private string _delimiter;
    private string _lineBreaking;

    private float _mousePosY;
    private Vector3 _defaultContentPos;
    private Transform _content;

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
        _inventoryManager = InventoryManager.Instance;
        _inventory_UI = UI_Manager.Instance.Inventory_UI;
        _button = GetComponent<Button>();
        _backGround = GetComponent<Image>();
        _defaultColor = _backGround.color;
        _delimiter = " : ";
        _lineBreaking = "\n";
    }

    private void Start()
    {
        _content = this.transform.parent;
        _button.onClick.AddListener(SlotClick);
        _itemExplanationPopup = _inventory_UI.transform.GetChild(3).gameObject;
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
        UnDisplayEquip();
    }

    public void SlotDisplay()
    {
        _item2DImage.enabled = true;
        _item2DImage.sprite = _itemData.Icon;
        _button.enabled = true;
        if(_slotInfo != null )
        {
            if(_slotInfo.equip)
                DisplayEquip();
            else
                UnDisplayEquip();
        }

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
        _inventoryManager.SetClickItem(_slotInfo, this);

        //클릭한 모습 표시
        DisplayItemClick();

        //클릭했을때 인벤토리 아래쪽에 있는 버튼 활성화
        _inventoryManager.CallDisplayInventoryTailUI();
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
        else
        {
            _defaultContentPos = _content.transform.position;
            _mousePosY = eventData.position.y;  
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isData)
            _itemImage.transform.position = eventData.position;
        else
        {
            _defaultContentPos.y += eventData.position.y - _mousePosY;
            _mousePosY = eventData.position.y;
            _content.position = _defaultContentPos;
        }
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
        else
        {
            _defaultContentPos = Vector3.zero;
            _mousePosY = 0;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryManager.Instance.SaveNewChangedSlot(_slotInfo, _uniqueIndex);
    }

    public void SetActiveItemExplanationPopup(bool isActive, ItemData_Test itemData)
    {
        if(_itemDB.GetStats(itemData.ID, out ItemStats itemStats))
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, float> i in itemStats.Stats)
            {
                if (i.Value > 0)
                    sb.Append(i.Key + _delimiter + (int)i.Value + _lineBreaking);
            }
            _itemText[1].text = sb.ToString();
        }
        else
            _itemText[1].text = "";
        
        _itemExplanationPopup.SetActive(isActive);
        _itemText[0].text = itemData.ItemName;
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
        _backGround.color = Color.blue;
    }

    public void UnDisplayEquip()
    {
        _backGround.color = _defaultColor;
    }
}
