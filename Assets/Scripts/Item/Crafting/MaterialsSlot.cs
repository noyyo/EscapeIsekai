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

    public void GetItemData(ItemData_Test newItem, int consumption, int count)
    {
        icon.enabled = true;
        icon.sprite = newItem.Icon;
        itemCount = count;
        this.consumption = consumption;
        UpdateText();
    }

    public void GetItemData(int id, int consumption, int count)
    {
        icon.enabled = true;
        itemDB.GetItemData(id, out ItemData_Test newItem);
        icon.sprite = newItem.Icon;
        itemCount = count;
        this.consumption = consumption;
        UpdateText();
    }

    public void GetItemData(Sprite icon, int consumption, int count)
    {
        this.icon.enabled = true;
        this.icon.sprite = icon;
        itemCount = count;
        this.consumption = consumption;
        UpdateText();
    }

    public void UpdateItemData()
    {
        if(itemCount >= consumption)
            itemCount -= consumption;
        UpdateText();
    }

    public void UpdateText()
    {
        text.text = itemCount + " / " + consumption;

        if (consumption == 0)
            text.text = "";

        if (consumption > itemCount)
            text.color = Color.red;
        else
            text.color = Color.black;
    }

    public void Init()
    {
        icon.enabled = false;
        text.text = "";
    }
}
