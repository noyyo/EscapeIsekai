using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemCrafting : MonoBehaviour
{
    [SerializeField] private Button craftingButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private TMP_Text craftPriceText;
    [SerializeField] private TMP_Text[] itemText;
    private ItemDB itemDB;
    private UI_Manager ui_Manager;
    private ItemCraftingManager craftingManager;
    private InventoryManager inventoryManager;
    
    private void Awake()
    {
        itemDB = ItemDB.Instance;
        craftingManager = ItemCraftingManager.Instance;
        ui_Manager = UI_Manager.Instance;
        inventoryManager = InventoryManager.Instance;
    }

    public void Start()
    {
        Init();
        craftingButton.onClick.AddListener(craftingManager.CallOnCrafting);
        backButton.onClick.AddListener(ui_Manager.CallUI_ItemCraftingTurnOff);
        inventoryButton.onClick.AddListener(() => { ui_Manager.CallUI_ItemCraftingTurnOff(); ui_Manager.CallUI_InventoryTurnOn(); });
        craftingManager.OnUpdateUIEvent += UpdatePriceText;
        craftingManager.OnUpdateUIEvent += UpdateItemExplanationText;
        craftingManager.OnUpdateUIEvent += AddMaterialsSlot;
    }

    private void Init()
    {
        if (craftingButton == null)
            craftingButton = this.transform.GetChild(2).GetChild(2).GetComponent<Button>();
        if (craftPriceText == null)
            craftPriceText = this.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>();
        if (itemText.Length == 0)
            itemText = this.transform.GetChild(3).GetComponentsInChildren<TMP_Text>();
        if (backButton == null)
            backButton = this.transform.GetChild(4).GetChild(0).GetChild(2).GetComponent<Button>();
        if (inventoryButton == null)
            inventoryButton = this.transform.GetChild(4).GetChild(0).GetChild(1).GetComponent<Button>();
    }

    public void UpdatePriceText(ItemRecipe clickSlot)
    {
        craftPriceText.text = "Crafting Price : " + clickSlot.CraftingPrice;
    }

    public void UpdateItemExplanationText(ItemRecipe clickSlot)
    {
        itemDB.GetItemData(clickSlot.CraftingID, out ItemData_Test craftingItem);
        itemText[0].text = craftingItem.ItemName;
        itemText[1].text = craftingItem.ItemExplanation;

        itemDB.GetStats(craftingItem.ID, out ItemStats itemStats);
        StringBuilder sb = new StringBuilder();

        if(itemStats.HP > 0)
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

        itemText[2].text = sb.ToString();
    }

    //������ ������ ��������
    public void AddMaterialsSlot(ItemRecipe clickSlot)
    {
        Sprite[] sprites = inventoryManager.CallIsCheckItems(clickSlot, out int[] sum);
        craftingManager.MaterialsSlots[0].GetItemData(sprites[0], 0, 0);
        int materialsLength = craftingManager.ClickSlot.Materials.Length;
        for (int i = 1; i <= materialsLength; i++)
        {
            craftingManager.MaterialsSlots[i].GetItemData(sprites[i], craftingManager.ClickSlot.MaterialsCount[i - 1], sum[i - 1]);
        }
        if (materialsLength != 7)
        {
            for (int i = materialsLength + 1; i <= 7; i++)
            {
                craftingManager.MaterialsSlots[i].Init();
            }
        }
    }
}
