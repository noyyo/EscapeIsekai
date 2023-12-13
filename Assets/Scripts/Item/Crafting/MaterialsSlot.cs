using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialsSlot : MonoBehaviour
{
    [SerializeField] private GameObject image;
    [SerializeField] private TMP_Text text;
    private ItemDB itemDB;
    private ItemCraftingManager craftingManager;
    private Image icon;
    private int itemCount;
    private int consumption;
    private bool isCraftingItem;
    private readonly string slash = " / ";
    private readonly string multiplication = "X ";

    private void Awake()
    {
        itemDB = ItemDB.Instance;
        craftingManager = ItemCraftingManager.Instance;
        icon = image.GetComponent<Image>();
    }

    private void Start()
    {
        craftingManager.OnTextUpdateEvent += UpdateItemData;
    }

    public void GetItemData(ItemData newItem, int consumption, int count, bool isCrafting)
    {
        icon.enabled = true;
        icon.sprite = newItem.Icon;
        itemCount = count;
        this.consumption = consumption;
        isCraftingItem = isCrafting;
        UpdateText();
    }

    public void GetItemData(int id, int newConsumption, int count, bool isCrafting)
    {
        icon.enabled = true;
        itemDB.GetItemData(id, out ItemData newItem);
        icon.sprite = newItem.Icon;
        itemCount = count;
        consumption = newConsumption;
        isCraftingItem = isCrafting;
        UpdateText();
    }

    public void GetItemData(Sprite newicon, int newConsumption, int newitemCount, bool isCrafting)
    {
        icon.enabled = true;
        icon.sprite = newicon;
        itemCount = newitemCount;
        consumption = newConsumption;
        isCraftingItem = isCrafting;
        UpdateText();
    }

    public void UpdateItemData()
    {
        if (itemCount >= consumption)
        {
            itemCount -= consumption;
            UpdateText();
        }
    }

    public void UpdateText()
    {
        if (icon.enabled)
        {
            if (isCraftingItem)
                text.text = multiplication + itemCount;
            else
            {
                text.text = itemCount + slash + consumption;

                if (consumption > itemCount)
                    text.color = Color.red;
                else
                    text.color = Color.black;
            }
        }
    }

    public void Init()
    {
        icon.enabled = false;
        text.text = "";
    }
}
