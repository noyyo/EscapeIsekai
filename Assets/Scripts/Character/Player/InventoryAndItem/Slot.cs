using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image _item2DImage;
    [SerializeField] private TMP_Text _text_Count;

    private ItemData _itemData;
    private ItemSlotInfo _slotInfo;
    private ItemDB _itemDB;
    private int _itemCount;

    public ItemData ItemData { get { return _itemData; } }
    public ItemSlotInfo SlotInfo { get { return _slotInfo; } }

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
    }

    private void Start()
    {

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
            _slotInfo.id = slotInfo.id;
            _slotInfo.index = slotInfo.index;
            _slotInfo.count = slotInfo.count;
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
}
