using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TradingController : MonoBehaviour
{
    private ItemDB itemDB;
    private InventoryManager inventoryManager;
    private TradingManager tradingManager;
    private UI_Manager ui_Manager;

    private Transform shopSlotSpawnTransform;
    private Transform playerSlotSpawnTransform;
    private GameObject slotPrefab;
    // 모든 슬롯을 담고있습니다.
    private List<UI_TradingSlot>[] tradingSlotList;
    // 플레이어 <- InventoryManager에서 들고옴
    private Dictionary<int, Item>[] playerInventoryDatas;
    private List<int>[] shopItemDatas;
    // 재구매를 위한 데이터정보 (Manager에 보관중)
    private List<ItemsSoldByUser> repurchaseItem;

    private void Awake()
    {
        ui_Manager = UI_Manager.Instance;
        itemDB = ItemDB.Instance;
        tradingManager = TradingManager.Instance;
        tradingManager.Init(SellItem, BuyItem, Repurchase);
        inventoryManager = InventoryManager.Instance;
        slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Trading/TradingSlot");
        Init();
    }

    private void Init()
    {
        shopSlotSpawnTransform = ui_Manager.Trading_UI.transform.GetChild(3).GetChild(0).GetChild(0);
        playerSlotSpawnTransform = ui_Manager.Trading_UI.transform.GetChild(5).GetChild(0).GetChild(0);
        tradingSlotList = tradingManager.TradingSlotList;
        playerInventoryDatas = inventoryManager.ItemDics;
        shopItemDatas = tradingManager.ShopItemIDList;
        repurchaseItem = tradingManager.RepurchaseItem;
        tradingManager.addShopItem += AddShopItem;
        tradingManager.clickSlotButtonEvent += () => ClickSlot(tradingManager.ClickID);
        tradingManager.clickBuyButtonEvent += ClickActionButton;
        CreatePlayerSlot();
    }

    private void CreatePlayerSlot()
    {
        int count = inventoryManager.InventroySlotCount;
        for (int i = 0; i < count; i++)
        {
            UI_TradingSlot slot = Instantiate(slotPrefab, playerSlotSpawnTransform).GetComponent<UI_TradingSlot>();
            slot.isPlayer = true;
            slot.UniqueIndex = i;
            tradingSlotList[0].Add(slot);
        }
    }

    private void CreateShopSlot()
    {
        UI_TradingSlot slot = Instantiate(slotPrefab, shopSlotSpawnTransform).GetComponent<UI_TradingSlot>();
        slot.UniqueIndex = tradingSlotList[1].Count;
        tradingSlotList[1].Add(slot);
    }

    private void AddShopItem(int id)
    {
        if (itemDB.GetItemData(id, out ItemData itemData))
        {
            int category = (id / 100000) % 10;
            if (category > 2)
                return;
            int index = shopItemDatas[category].IndexOf(id);
            if (tradingSlotList[1].Count <= shopItemDatas[category].Count)
                CreateShopSlot();
            if (index < 0)
                shopItemDatas[category].Add(id);
        }
        else
            Debug.Log("id를 확인해 주세요.");
    }

    private void ClickSlot(int id)
    {
        if (itemDB.GetItemData(id, out ItemData itemData))
        {
            if (itemDB.GetStats(id, out ItemStats itemStats))
            {
                StringBuilder sb = new StringBuilder();

                if (itemStats.HP > 0)
                    sb.Append("HP : " + itemStats.HP + "\n");
                if (itemStats.Temperature > 0)
                    sb.Append("Temperature : " + itemStats.Temperature + "\n");
                if (itemStats.ATK > 0)
                    sb.Append("ATK : " + itemStats.ATK + "\n");
                if (itemStats.DEF > 0)
                    sb.Append("DEF : " + itemStats.DEF + "\n");
                if (itemStats.Speed > 0)
                    sb.Append("Speed : " + itemStats.Speed + "\n");
                if (itemStats.Stamina > 0)
                    sb.Append("Stamina : " + itemStats.Stamina + "\n");

                tradingManager.itemExplanationText(itemData.ItemName, itemData.ItemExplanation, sb.ToString());
            }
            else
            {
                tradingManager.itemExplanationText(itemData.ItemName, itemData.ItemExplanation, "");
            }
        }
    }

    private void ClickActionButton()
    {
        if (tradingManager.isPlayerSlotClick)
        {
            ClickSellButton();
            tradingManager.CallOnDisplayPlayerSlot();
        }
        else
        {
            if (tradingManager.displayShopItemCategory == 3)
                ClickRepurchase();
            else
                ClickBuyButton();
            tradingManager.CallOnDisplayShopSlot();
        }
    }

    private void ClickBuyButton()
    {
        if (!tradingManager.tryByitem(tradingManager.ClickID, 1))
            Debug.Log("돈이 부족합니다.");
        else
            tradingManager.CallOnDisplayPlayerSlot();
    }

    private void ClickSellButton()
    {
        if (!tradingManager.trySellItem(tradingManager.ClickID, 1))
            Debug.Log("아이템이 없습니다.");
        else
            tradingManager.CallOnDisplayShopSlot();
    }

    private void ClickRepurchase()
    {
        if (!tradingManager.tryRepurchase(tradingManager.ClickIndex, 1))
            Debug.Log("돈이 부족합니다.");
        else
            tradingManager.CallOnDisplayPlayerSlot();
    }

    private int SellItem(int itemID, int itemCount)
    {
        int sum = 0;
        if (itemDB.GetItemData(itemID, out ItemData itemData))
        {
            if (inventoryManager.CallTryAddItem(itemID, -itemCount))
            {
                sum = itemCount * itemData.Price;
                if (repurchaseItem.Count >= tradingManager.RepurchaseItemMaxCount)
                    repurchaseItem.RemoveAt(0);

                repurchaseItem.Add(new ItemsSoldByUser(itemID, itemCount));

                if (tradingSlotList[1].Count <= repurchaseItem.Count)
                    CreateShopSlot();
            }
        }
        else
            Debug.LogError("ID를 확인해 주세요.");
        return sum;
    }

    private int BuyItem(int itemID, int itemCount)
    {
        int sum = 0;
        if (itemDB.GetItemData(itemID, out ItemData itemData))
        {
            if (itemCount * itemData.Price <= tradingManager.PlayerMoney)
                if (inventoryManager.CallTryAddItem(itemID, itemCount))
                    sum = itemCount * itemData.Price;
        }
        else
            Debug.LogError("ID를 확인해 주세요.");
        return sum;
    }

    private int Repurchase(int index, int itemCount)
    {
        if (itemCount > repurchaseItem[index].itemCount)
        {
            Debug.Log("Repurchase의 itemCount를 제대로 입력해주세요");
            return 0;
        }

        int sum = 0;
        itemDB.GetItemData(repurchaseItem[index].itemID, out ItemData itemData);
        if (itemCount * itemData.Price <= tradingManager.PlayerMoney)
        {
            if (inventoryManager.CallTryAddItem(itemData.ID, itemCount))
            {
                sum = itemCount * itemData.Price;
                if ((repurchaseItem[index].itemCount -= itemCount) == 0)
                    repurchaseItem.RemoveAt(index);
            }
        }
        return sum;
    }
}
