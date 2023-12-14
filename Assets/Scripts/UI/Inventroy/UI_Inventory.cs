using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryTailButtonArea;
    [SerializeField] private GameObject itemExplanationPopup;
    [SerializeField] private GameObject tailUseButton;
    [SerializeField] private Button[] inventoryTypeButtons;
    [SerializeField] private Button[] inventoryTailButtons;
    [SerializeField] private Button backButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private TMP_Text tailUseButtonText;
    [SerializeField] private TMP_Text money;

    private GameManager gameManager;
    private UI_Manager ui_manager;
    private InventoryManager inventoryManager;
    private TradingManager tradingManager;
    private Inventory inventory;
    private GameObject inventoryUI;
    private TMP_Text[] itemExplanationTexts;
    private readonly string delimiter = " : ";
    private readonly string lineBreaking = "\n";
    private ItemType nowDisplayItemType;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        inventoryManager = InventoryManager.Instance;
        ui_manager = UI_Manager.Instance;
        tradingManager = TradingManager.Instance;
        Init();
    }

    //변할수있는 값 초기화
    private void Init()
    {
        nowDisplayItemType = ItemType.Equipment;

        if (inventoryUI == null)
            inventoryUI = gameObject;

        if (inventoryTailButtonArea == null)
            inventoryTailButtonArea = transform.GetChild(1).GetChild(1).gameObject;

        if (itemExplanationPopup == null)
            itemExplanationPopup = transform.GetChild(3).gameObject;

        if (inventory == null)
            inventory = gameManager.Player.GetComponent<Inventory>();

        if (inventoryTypeButtons.Length == 0)
            inventoryTypeButtons = transform.GetChild(2).GetComponentsInChildren<Button>();

        if (inventoryTypeButtons.Length == 0)
            inventoryTailButtons = inventoryTailButtonArea.GetComponentsInChildren<Button>();

        if (backButton == null)
            backButton = transform.GetChild(1).GetChild(0).GetComponent<Button>();

        if (tailUseButton == null)
            tailUseButton = inventoryTailButtons[2].gameObject;

        if (tailUseButtonText == null)
            tailUseButtonText = tailUseButton.GetComponentInChildren<TMP_Text>();

        itemExplanationTexts = itemExplanationPopup.transform.GetComponentsInChildren<TMP_Text>();

        inventoryTypeButtons[0].onClick.AddListener(() => { OnCategoryButton(ItemType.Equipment); });
        inventoryTypeButtons[1].onClick.AddListener(() => { OnCategoryButton(ItemType.Consumable); });
        inventoryTypeButtons[2].onClick.AddListener(() => { OnCategoryButton(ItemType.Material); });
        inventoryTypeButtons[3].onClick.AddListener(() => { OnCategoryButton(ItemType.ETC); });

        inventoryTailButtons[0].onClick.AddListener(() => { ui_manager.PlayClickBtnSound(); inventory.SortInventory(); });
        inventoryTailButtons[1].onClick.AddListener(() => { ui_manager.PlayClickBtnSound(); inventory.Drop(); });
        inventoryTailButtons[2].onClick.AddListener(() => { ui_manager.PlayClickBtnSound(); inventory.UseItem(); });

        backButton.onClick.AddListener(() => { ui_manager.PlayClickBtnSound(); ui_manager.CallUI_InventoryTurnOff(); }); //돌아가기
        optionButton.onClick.AddListener(() => { ui_manager.PlayClickBtnSound(); ui_manager.CallUI_InventoryTurnOff(); ui_manager.CallUI_OptionTurnOn(); });

        ui_manager.UI_InventoryTurnOnEvent += InventroyUITurnOn;
        ui_manager.UI_InventoryTurnOffEvent += InventroyUITurnOff;

        inventoryManager.OnTextChangeEquipEvent += ButtonTextChange_Equip;
        inventoryManager.OnTextChangeUnEquipEvent += ButtonTextChange_Unequip;
        inventoryManager.OnItemExplanationPopUpEvent += ActiveItemExplanationPopUp;
        inventoryManager.UseButtonTurnOffEvent += UseButtonTurnOff;

        tradingManager.moneyTextUpdateEvent += MoneyTextUpdate;
    }

    //이벤트에 걸린 메서드( 인벤토리 창 ON, OFF)
    private void InventroyUITurnOn()
    {
        inventoryUI.SetActive(true);
    }

    private void InventroyUITurnOff()
    {
        inventoryUI.SetActive(false);
    }

    // 카테고리 변경됨으로써 현재 카테고리 저장후 UI표시
    private void CallItemSlots(ItemType displayType)
    {
        nowDisplayItemType = displayType;
        inventoryManager.CallOnSetDisplayType(displayType);
    }

    // 설명창 끄기
    public void TurnOffItemExplanationPopup()
    {
        itemExplanationPopup.SetActive(false);
    }

    //현재 카테고리에 따른 밑쪽 버튼텍스트 번경 및 ON, OFF
    public void DisplayInventoryTailUI(bool isEquip)
    {
        inventoryTailButtonArea.SetActive(true);
        switch (nowDisplayItemType)
        {
            case ItemType.Equipment:
                tailUseButton.SetActive(true);
                if (isEquip)
                    ButtonTextChange_Unequip();
                else
                    ButtonTextChange_Equip();
                break;
            case ItemType.Consumable:
                tailUseButton.SetActive(true);
                tailUseButtonText.text = "소비";
                break;
            case ItemType.Material:
                UseButtonTurnOff();
                break;
            default:
                UseButtonTurnOff();
                break;
        }
    }

    // 밑쪽 버튼 모두 OFF
    public void TurnOffInventoryTailUI()
    {
        inventoryTailButtonArea.SetActive(false);
    }

    //카테고리 버튼 눌렀을시 해야해는 동작 모음 (이벤트로 바꿔도 상관없을듯)
    public void OnCategoryButton(ItemType categoryType)
    {
        ui_manager.PlayClickBtnSound();
        if (nowDisplayItemType == categoryType) return;
        InventoryUITurnOff();
        CallItemSlots(categoryType); //Slot에 표시되는 Item 바꾸기
    }

    public void InventoryUITurnOff()
    {
        TurnOffItemExplanationPopup(); // 설명창 오프
        TurnOffInventoryTailUI(); // 인벤토리 UI 오프
        inventoryManager.CallTurnOffItemClick(); //클릭시 뜨는 UI 오프
    }

    public void ButtonTextChange_Equip()
    {
        tailUseButtonText.text = "장착";
    }

    public void ButtonTextChange_Unequip()
    {
        tailUseButtonText.text = "장비 해제";
    }

    public void ActiveItemExplanationPopUp(Item itemObject)
    {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, float> i in itemObject.Stats)
        {
            if (i.Value > 0)
                sb.Append(i.Key + delimiter + (int)i.Value + lineBreaking);
        }
        itemExplanationTexts[0].text = itemObject.ItemName;
        itemExplanationTexts[1].text = sb.ToString();
        itemExplanationTexts[2].text = itemObject.ItemExplanation;
        itemExplanationPopup.SetActive(true);
    }

    private void MoneyTextUpdate()
    {
        money.text = tradingManager.PlayerMoney.ToString();
    }
    
    private void UseButtonTurnOff()
    {
        tailUseButton.SetActive(false);
    }
}
