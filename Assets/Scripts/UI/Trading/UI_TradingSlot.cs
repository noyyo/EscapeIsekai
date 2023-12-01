using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TradingSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemCount;
    [SerializeField] private TMP_Text price;
    [SerializeField] private GameObject outLine;
    [SerializeField] private Image icon;

    private ItemDB itemDB;
    private TradingManager tradingManager;
    private Button button;
    private string priceText;
    private string itemCountText;
    private event Action ResetEvent;
    private int itemID;
    private int uniqueIndex = -1;

    public int UniqueIndex
    {
        get { return uniqueIndex; }
        set
        {
            if (uniqueIndex == -1)
                uniqueIndex = value;
        }
    }

    public bool isPlayer;

    private void Awake()
    {
        itemDB = ItemDB.Instance;
        tradingManager = TradingManager.Instance;
        button = GetComponent<Button>();
        Init();
    }

    private void Init()
    {
        priceText = "Price : ";
        itemCountText = "X ";
        button.onClick.AddListener(OnClickSlot);
        ResetEvent += TurnOffOutLine;
        ResetEvent += TurnOffIcon;
        ResetEvent += TurnOffSlot;
        CallOnResetEvent();
    }

    public void DisPlayItemData(int id, int count)
    {
        if(itemDB.GetItemData(id, out ItemData itemData))
        {
            itemID = id;
            icon.sprite = itemData.Icon;
            itemName.text = itemData.ItemName;
            price.text = priceText + itemData.Price;
            TurnOnSlot();
            TurnOnIcon();
            TurnOffOutLine();
            ItemCountTextUpdate(count);
        }
    }

    public void DisPlayItemData(in ItemData itemData, int count)
    {
        itemID = itemData.ID;
        icon.sprite = itemData.Icon;
        itemName.text = itemData.ItemName;
        price.text = priceText + itemData.Price;
        TurnOnSlot();
        TurnOnIcon();
        TurnOffOutLine();
        ItemCountTextUpdate(count);
    }

    public void DisPlayItemData(in Item item)
    {
        itemID = item.ID;
        icon.sprite = item.Icon;
        itemName.text = item.ItemName;
        price.text = priceText + item.Price;
        TurnOnSlot();
        TurnOnIcon();
        TurnOffOutLine();

        ItemCountTextUpdate(item.Count);
    }

    private void OnClickSlot()
    {
        tradingManager.isPlayerSlotClick = isPlayer;
        tradingManager.SetClickID(itemID);
        tradingManager.SetClickIndex(uniqueIndex);
        tradingManager.CallOnClickSlotButton();
        TurnOnOutLine();
        tradingManager.clickSlotButtonEvent += TurnOffOutLine;
    }


    private void ItemCountTextUpdate(int count)
    {
        if (count > 1)
        {
            itemCount.gameObject.SetActive(true);
            itemCount.text = itemCountText + count;
        }
        else
            itemCount.gameObject.SetActive(false);
    }

    private void TurnOnSlot()
    {
        this.gameObject.SetActive(true);
    }

    private void TurnOffSlot()
    {
        this.gameObject.SetActive(false);
    }

    private void TurnOnIcon()
    {
        icon.enabled = true;
    }

    private void TurnOffIcon()
    {
        icon.enabled = false;
    }

    private void TurnOnOutLine()
    {
        outLine.SetActive(true);
    }

    private void TurnOffOutLine()
    {
        tradingManager.clickSlotButtonEvent -= TurnOffOutLine;
        outLine.SetActive(false);
    }

    public void CallOnResetEvent()
    {
        ResetEvent?.Invoke();
    }
}