using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemAvailability;
    [SerializeField] private GameObject outLine;
    [SerializeField] private Image icon;

    //슬롯 데이터 저장
    private GameManager gameManager;
    private ItemDB itemDB;
    private ItemCraftingManager craftingManager;
    private ItemData itemData;
    private Inventory inventory;
    private ItemRecipe itemRecipe;
    private Button button;
    private int[] materialsCount;
    private bool[] boolArray;
    private bool isDisplay = true;
    private bool isMake;

    private void Awake()
    {
        Init();
        TurnOnOffSlot();
    }

    private void Init()
    {
        gameManager = GameManager.Instance;
        craftingManager = ItemCraftingManager.Instance;
        itemDB = ItemDB.Instance;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        inventory = gameManager.Player.GetComponent<Inventory>();
        craftingManager.OnTextUpdateEvent += MakeCheck;
    }

    //목차버튼 누르면 슬롯을 활성화하기 위한 메서드
    public void TurnOnOffSlot()
    {
        isDisplay = !isDisplay;
        gameObject.SetActive(isDisplay);
    }

    //슬롯의 정보를 저장하기 위한 메서드
    public void SetSlot(in ItemRecipe newRecipe)
    {
        if (itemDB == null)
            Init();
        itemDB.GetItemData(newRecipe.CraftingID, out ItemData newItemData);
        itemData = newItemData;
        itemRecipe = newRecipe;
        icon.sprite = newItemData.Icon;
        itemName.text = newItemData.ItemName;
        MakeCheck();
    }

    private void OnButtonClick()
    {
        craftingManager.CallOnClickCraftingSlotEvent(itemRecipe, isMake);
        craftingManager.ChangeCurrentIsMake = ReturnCurrentIsMake;
        outLine.SetActive(true);
        craftingManager.OffOutLineEvent += OutLineTrunOff;
    }

    private void OutLineTrunOff()
    {
        outLine.SetActive(false);
    }

    private void MakeCheck()
    {
        boolArray = inventory.IsCheckItems(itemRecipe.Materials, itemRecipe.MaterialsCount);

        isMake = true;
        foreach (bool i in boolArray)
        {
            if (i == false)
                isMake = false;
        }
        UpdateIsMakeTextUI();
    }

    private void UpdateIsMakeTextUI()
    {
        if (isMake)
        {
            itemAvailability.text = "제작이 가능합니다.";
            itemAvailability.color = Color.white;
        }
        else
        {
            itemAvailability.text = "재료가 부족합니다.";
            itemAvailability.color = Color.red;
        }
    }

    private bool ReturnCurrentIsMake()
    {
        return isMake;
    }
}
