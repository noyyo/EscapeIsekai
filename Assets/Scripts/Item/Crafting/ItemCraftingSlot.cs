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
    private ItemData itemData;
    private Inventory inventory;
    private int[] materialsCount;
    private bool[] boolArray;
    private ItemCraftingManager craftingManager;

    private Button button;
    private bool isDisplay = false;
    private bool isMake = true;
    private ItemDB itemDB;
    private GameManager gameManager;
    private ItemRecipe itemRecipe;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        gameManager = GameManager.Instance;
        craftingManager = ItemCraftingManager.Instance;
        itemDB = ItemDB.Instance;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        inventory = gameManager.Player.GetComponent<Inventory>();
    }

    private void Start()
    {
        craftingManager.OnCraftingEvent += MakeCheck;
    }

    //목차버튼 누르면 슬롯을 활성화하기 위한 메서드
    public void TurnOnOffSlot()
    {
        isDisplay = !isDisplay;
        this.gameObject.SetActive(isDisplay);
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
        MakeCheck(newRecipe);
    }

    //버튼을 누르면 실행
    public void OnButtonClick()
    {
        MakeCheck(itemRecipe);
        craftingManager.CallOnClickCraftingSlotEvent(itemRecipe, isMake);
        outLine.SetActive(true);
        craftingManager.OffOutLineEvent += OutLineTrunOff;
    }

    //자동해제를 위한 메서드
    public void OutLineTrunOff()
    {
        outLine.SetActive(false);
    }

    public void MakeCheck(ItemRecipe newRecipe)
    {
        boolArray = inventory.IsCheckItems(newRecipe.Materials, newRecipe.MaterialsCount, out materialsCount);

        isMake = true;
        foreach (bool i in boolArray)
        {
            if (i == false)
                isMake = false;
        }
        UpdateIsMakeTextUI();
    }

    public void MakeCheck()
    {
        //아이템 재료가 있는지 확인
        boolArray = inventory.IsCheckItems(itemRecipe.Materials, itemRecipe.MaterialsCount, out materialsCount);

        isMake = true;
        foreach (bool i in boolArray)
        {
            if (i == false)
                isMake = false;
        }
        //아이템 추가 후 다음에 제작가능한지 확인
        if (craftingManager.CraftingItem(isMake))
        {
            boolArray = inventory.IsCheckItems(itemRecipe.Materials, itemRecipe.MaterialsCount, out materialsCount);

            isMake = true;
            foreach (bool i in boolArray)
            {
                if (i == false)
                    isMake = false;
            }
        }
        UpdateIsMakeTextUI();
    }

    public void UpdateIsMakeTextUI()
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
}
