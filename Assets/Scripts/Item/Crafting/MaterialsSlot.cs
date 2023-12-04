using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialsSlot : MonoBehaviour
{
    [SerializeField] private GameObject image;
    [SerializeField] private TMP_Text text;
    private Image icon;
    private ItemDB itemDB;
    private int itemCount;
    private int consumption;
    private ItemCraftingManager craftingManager;
    private bool isCraftingItem;
    private readonly string slash = " / ";
    private readonly string multiplication = "X ";

    private void Awake()
    {
        itemDB = ItemDB.Instance;
        icon = image.GetComponent<Image>();
        craftingManager = ItemCraftingManager.Instance;
    }

    private void Start()
    {
        craftingManager.OnCraftingEvent += UpdateItemData;
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

    public void GetItemData(int id, int consumption, int count, bool isCrafting)
    {
        icon.enabled = true;
        itemDB.GetItemData(id, out ItemData newItem);
        icon.sprite = newItem.Icon;
        itemCount = count;
        this.consumption = consumption;
        isCraftingItem = isCrafting;
        UpdateText();
    }

    public void GetItemData(Sprite icon, int consumption, int itemCount, bool isCrafting)
    {
        this.icon.enabled = true;
        this.icon.sprite = icon;
        this.itemCount = itemCount;
        this.consumption = consumption;
        isCraftingItem = isCrafting;
        UpdateText();
    }

    public void UpdateItemData()
    {
        if (itemCount >= consumption)
            itemCount -= consumption;
        UpdateText();
    }

    public void UpdateText()
    {
        if (isCraftingItem)
        {
            text.text = multiplication + itemCount;
        }
        else
        {
            if (consumption == 0)
                text.text = "";
            else
                text.text = itemCount + slash + consumption;

            if (consumption > itemCount)
                text.color = Color.red;
            else
                text.color = Color.black;
        }
    }

    public void Init()
    {
        icon.enabled = false;
        text.text = "";
    }
}
