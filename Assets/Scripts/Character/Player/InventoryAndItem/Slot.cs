using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler ,IEndDragHandler
{
    [SerializeField] private GameObject _itemImage;
    [SerializeField] private TMP_Text _text_Count;
    private Transform _itemImageTransform;
    private Image _item2DImage;
    private ItemData _itemData;
    private ItemSlotInfo _slotInfo;
    private ItemDB _itemDB;
    private int _itemCount;
    private Vector3 defaultPos;
    private GameObject _inventory;

    public ItemData ItemData { get { return _itemData; } }
    public ItemSlotInfo SlotInfo { get { return _slotInfo; } }

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        _slotInfo = new ItemSlotInfo();
        _item2DImage = _itemImage.GetComponent<Image>();
        _itemImageTransform = _itemImage.transform;
        //_inventory = UI_Inventory.Inventory_UI;
    }

    //// 아이템 이미지의 투명도 조절
    //private void SetColor(float _alpha)
    //{
    //    Color color = _item2DImage.color;
    //    color.a = _alpha;
    //    _item2DImage.color = color;
    //}

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(ItemData itemData, int index, int count)
    {
        _slotInfo = new ItemSlotInfo();
        _itemData = itemData;
        _slotInfo.id = itemData.ID;
        _slotInfo.index = index;
        _slotInfo.count = count;
        SlotDisplay();

        //SetColor(1);
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

    // 해당 슬롯의 아이템 갯수 업데이트
    public void SetSlotCount(int count)
    {
        _slotInfo.count += count;
        _text_Count.text = String.Format("x {0}", _slotInfo.count);

        if (_itemCount <= 0)
            ClearSlot();
    }

    // 해당 슬롯 하나 삭제
    public void ClearSlot()
    {
        _itemData = null;
        _slotInfo = null;
        _item2DImage.sprite = null;
        _item2DImage.enabled = false;
        //SetColor(0);

        _text_Count.text = "";
        _text_Count.gameObject.SetActive(false);
    }

    public void SlotDisplay()
    {
        _item2DImage.enabled = true;
        _item2DImage.sprite = _itemData.Icon;

        if (_itemData.ItemType != ItemType.Equipment)
        {
            _text_Count.gameObject.SetActive(true);
            _text_Count.text = String.Format("x {0}", _slotInfo.count);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        defaultPos = _itemImageTransform.position;
        _itemImageTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _itemImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _itemImage.transform.position = defaultPos;
    }

    
}
