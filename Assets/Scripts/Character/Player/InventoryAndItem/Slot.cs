using PolyAndCode.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour, ICell
{
    private Image _item2DImage;
    private TMP_Text _text_Count;


    private ItemData _itemData;
    private ItemSlotInfo _slotInfo;
    private int _itemCount;

    private void Start()
    {
        _slotInfo = new ItemSlotInfo();
        //GetComponent<Button>().onClick.AddListener(ButtonListener);
    }

    // 아이템 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = _item2DImage.color;
        color.a = _alpha;
        _item2DImage.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(ItemData item, int count)
    {
        _itemData = item;
        _itemCount = count;
        _item2DImage.enabled = true;
        _item2DImage.sprite = item.Icon;

        if(item.ItemType != ItemType.Equipment)
        {
            _text_Count.gameObject.SetActive(true);
            _text_Count.text = String.Format("x {0}",_itemCount);
        }

        SetColor(1);
    }

    // 해당 슬롯의 아이템 갯수 업데이트
    public void SetSlotCount(int _count)
    {
        _itemCount += _count;
        _text_Count.text = String.Format("x {0}", _itemCount);

        if (_itemCount <= 0)
            ClearSlot();
    }

    // 해당 슬롯 하나 삭제
    private void ClearSlot()
    {
        _itemData = null;
        _itemCount = 0;
        _item2DImage.sprite = null;
        _item2DImage.enabled = false;
        SetColor(0);

        _text_Count.text = "0";
        _text_Count.gameObject.SetActive(false);
    }

    //private void ButtonListener()
    //{
    //    Debug.Log("Index : " + _cellIndex + ", Name : " + _contactInfo.Name + ", Gender : " + _contactInfo.Gender);
    //}
    public void ConfigureCell(ItemSlotInfo _slotInfo)
    {

    }
}
