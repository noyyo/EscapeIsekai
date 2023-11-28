using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UI_Trading : MonoBehaviour
{
    [SerializeField] private GameObject shopItemCategory;
    [SerializeField] private GameObject myItemCategory;
    [SerializeField] private GameObject itemExplanation;
    [SerializeField] private GameObject tail;
    [SerializeField] private TMP_Text money;

    private UI_Manager ui_Manager;
    private GameManager gameManager;
    private TradingManager tradingManager;
    private InventoryManager inventoryManager;
    private Button[] shopCategoryButtons;
    private Button[] myCategoryButtons;
    private TMP_Text[] itemExplanationTexts;
    private Button[] tailButton;
    private TMP_Text buyButtonText;
    private GameObject BuyButton;
    private event Action ClickCategoryButtonEvnet;

    private void Awake()
    {
        ui_Manager = UI_Manager.Instance;
        gameManager = GameManager.Instance;
        tradingManager = TradingManager.Instance;
        inventoryManager = InventoryManager.Instance;
        Init();
    }

    private void Init()
    {
        shopCategoryButtons = shopItemCategory.GetComponentsInChildren<Button>();
        myCategoryButtons = myItemCategory.GetComponentsInChildren<Button>();
        itemExplanationTexts = itemExplanation.GetComponentsInChildren<TMP_Text>();
        tailButton = tail.GetComponentsInChildren<Button>();
        BuyButton = tail.transform.GetChild(1).gameObject;

        myCategoryButtons[0].onClick.AddListener(() => PlayerItemCategoryButton(0));
        myCategoryButtons[1].onClick.AddListener(() => PlayerItemCategoryButton(1));
        myCategoryButtons[2].onClick.AddListener(() => PlayerItemCategoryButton(2));
        myCategoryButtons[3].onClick.AddListener(() => PlayerItemCategoryButton(3));

        shopCategoryButtons[0].onClick.AddListener(() => ShopItemCategoryButton(0));
        shopCategoryButtons[1].onClick.AddListener(() => ShopItemCategoryButton(1));
        shopCategoryButtons[2].onClick.AddListener(() => ShopItemCategoryButton(2));
        shopCategoryButtons[3].onClick.AddListener(() => ShopItemCategoryButton(3));

        tailButton[0].onClick.AddListener(() => { ui_Manager.CallUI_TradingTurnOff(); });
        tailButton[1].onClick.AddListener(tradingManager.CallOnClickBuyButton);
        tailButton[2].onClick.AddListener(() => { ui_Manager.CallUI_TradingTurnOff(); ui_Manager.CallUI_InventoryTurnOn(); });

        ui_Manager.UI_TradingTurnOnEvent += Activate;
        ui_Manager.UI_TradingTurnOffEvent += Deactivate;
        ui_Manager.UI_TradingTurnOffEvent += ItemExplanationTurnOff;
        ui_Manager.UI_TradingTurnOffEvent += BuyButtonTurnOff;

        tradingManager.clickSlotButtonEvent += ItemExplanationTurnOn;
        tradingManager.clickSlotButtonEvent += BuyButtonTurnOn;
        tradingManager.clickSlotButtonEvent += BuyButtonTextUpdate;
        tradingManager.itemExplanationText = ItemExplanationUpdateText;

        ClickCategoryButtonEvnet += ItemExplanationTurnOff;
        ClickCategoryButtonEvnet += BuyButtonTurnOff;
        buyButtonText = BuyButton.GetComponentInChildren<TMP_Text>();

        tradingManager.displayPlayerSlotEvent += DisplayPlayerSlot;
        tradingManager.displayShopSlotEvent += DisplayShopSlot;

        tradingManager.clickBuyButtonEvent += BuyButtonTurnOff;
        tradingManager.clickBuyButtonEvent += ItemExplanationTurnOff;
        tradingManager.moneyTextUpdateEvent += MoneyTextUpdate;

        BuyButtonTurnOff();
        ItemExplanationTurnOff();
    }

    private void DisplayPlayerSlot(int category)
    {
        int i = 0;
        int count = tradingManager.TradingSlotList[0].Count;
        for (; i < count; i++)
            tradingManager.TradingSlotList[0][i].CallOnResetEvent();

        i = 0;
        if (inventoryManager.ItemDics[category].Count != 0)
        {
            foreach (KeyValuePair<int, Item> item in inventoryManager.ItemDics[category])
            {
                tradingManager.TradingSlotList[0][i].DisPlayItemData(item.Value);
                i++;
            }
        }
    }

    private void DisplayShopSlot(int category)
    {
        int i = 0;

        int count = tradingManager.TradingSlotList[1].Count;
        if( count > 0)
        {
            for (; i < count; i++)
                tradingManager.TradingSlotList[1][i].CallOnResetEvent();
            i = 0;
            if (category == 3 && tradingManager.RepurchaseItem.Count != 0)
            {
                foreach (ItemsSoldByUser j in tradingManager.RepurchaseItem)
                {
                    tradingManager.TradingSlotList[1][i].DisPlayItemData(j.itemID, j.itemCount);
                    i++;
                }
            }
            else if (category != 3 && tradingManager.ShopItemIDList[category].Count != 0)
            {
                foreach (int id in tradingManager.ShopItemIDList[category])
                {
                    tradingManager.TradingSlotList[1][i].DisPlayItemData(id, 1);
                    i++;
                }
            }
        }
    }

    private void PlayerItemCategoryButton(int category)
    {
        if(tradingManager.displayPlayerItemCategory != category)
        {
            DisplayPlayerSlot(category);
            tradingManager.displayPlayerItemCategory = category;
            ClickCategoryButtonEvnet?.Invoke();

        }        
    }

    private void ShopItemCategoryButton(int category)
    {
        if(tradingManager.displayShopItemCategory != category)
        {
            DisplayShopSlot(category);
            tradingManager.displayShopItemCategory = category;
            ClickCategoryButtonEvnet?.Invoke();
        }
    }

    private void BuyButtonTurnOn()
    {
        BuyButton.SetActive(true);
    }

    private void BuyButtonTurnOff()
    {
        BuyButton.SetActive(false);
    }
    
    private void MoneyTextUpdate()
    {
        money.text = tradingManager.PlayerMoney.ToString();
    }

    private void BuyButtonTextUpdate()
    {
        if (tradingManager.isPlayerSlotClick)
            buyButtonText.text = "판매";
        else
            buyButtonText.text = "구매";
    }

    private void ItemExplanationTurnOn()
    {
        itemExplanation.SetActive(true);
    }

    private void ItemExplanationTurnOff()
    {
        itemExplanation.SetActive(false);
    }

    private void ItemExplanationUpdateText(string itemName, string itemExplanation, string itemStatus)
    {
        itemExplanationTexts[0].text = itemName;
        itemExplanationTexts[1].text = itemExplanation;
        itemExplanationTexts[2].text = itemStatus;
    }

    private void Activate()
    {
        this.gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
