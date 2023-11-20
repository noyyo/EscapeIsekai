using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryTailButtonArea;
    [SerializeField] private GameObject itemExplanationPopup;
    [SerializeField] private Button[] inventoryTypeButtons;
    [SerializeField] private Button[] inventoryTailButtons;
    [SerializeField] private GameObject tailUseButton;
    [SerializeField] private TMP_Text tailUseButtonText;
    [SerializeField] private Button backButton;

    private Inventory inventory;
    private GameObject inventoryUI;
    private InventoryManager inventoryManager;
    private UI_Manager ui_manager;
    private GameManager gameManager;
    private TMP_Text[] itemExplanationTexts;
    private string delimiter;
    private string lineBreaking;

    //���� ��µǰ� �ִ� ī�װ�
    private ItemType nowDisplayItemType;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        inventoryManager = InventoryManager.Instance;
        ui_manager = UI_Manager.Instance;

        delimiter = " : ";
        lineBreaking = "\n";
        Init();
    }

    //���Ҽ��ִ� �� �ʱ�ȭ
    private void Init()
    {
        nowDisplayItemType = ItemType.Equipment;

        if (inventoryUI == null)
            inventoryUI = this.gameObject;

        if (inventoryTailButtonArea == null)
            inventoryTailButtonArea = this.transform.GetChild(1).GetChild(1).gameObject;

        if (itemExplanationPopup == null)
            itemExplanationPopup = this.transform.GetChild(3).gameObject;

        if (inventory == null)
            inventory = gameManager.Player.GetComponent<Inventory>();

        //��ư ����
        // ��� : 0, �Һ� : 1, ��� : 2, ��Ÿ : 3
        if(inventoryTypeButtons.Length == 0)
            inventoryTypeButtons = this.transform.GetChild(2).GetComponentsInChildren<Button>();

        //���� : 0, ������ : 1, ��� : 2
        if( inventoryTypeButtons.Length == 0)
            inventoryTailButtons = inventoryTailButtonArea.GetComponentsInChildren<Button>();

        //�ڷΰ���
        if(backButton == null)
            backButton = this.transform.GetChild(1).GetChild(0).GetComponent<Button>();

        if(tailUseButton == null)
            tailUseButton = inventoryTailButtons[2].gameObject;

        if(tailUseButtonText == null)
            tailUseButtonText = tailUseButton.GetComponentInChildren<TMP_Text>();

        itemExplanationTexts = itemExplanationPopup.transform.GetComponentsInChildren<TMP_Text>();

        inventoryTypeButtons[0].onClick.AddListener(() => { OnCategoryButton(ItemType.Equipment); });
        inventoryTypeButtons[1].onClick.AddListener(() => { OnCategoryButton(ItemType.Consumable); });
        inventoryTypeButtons[2].onClick.AddListener(() => { OnCategoryButton(ItemType.Material); });
        inventoryTypeButtons[3].onClick.AddListener(() => { OnCategoryButton(ItemType.ETC); });

        inventoryTailButtons[0].onClick.AddListener(inventory.SortInventory);
        inventoryTailButtons[1].onClick.AddListener(inventory.Drop);
        inventoryTailButtons[2].onClick.AddListener(inventory.UseItem);

        backButton.onClick.AddListener(ui_manager.CallUI_InventoryTurnOff); //���ư���

        ui_manager.UI_InventoryTurnOnEvent += InventroyUITurnOn;
        ui_manager.UI_InventoryTurnOffEvent += InventroyUITurnOff;

        inventoryManager.OnTextChangeEquipEvent += ButtonTextChange_Equip;
        inventoryManager.OnTextChangeUnEquipEvent += ButtonTextChange_Unequip;
        inventoryManager.OnItemExplanationPopUpEvent += ActiveItemExplanationPopUp;
    }

    //�̺�Ʈ�� �ɸ� �޼���( �κ��丮 â ON, OFF)
    private void InventroyUITurnOn()
    {
        inventoryUI.SetActive(true);
    }

    private void InventroyUITurnOff()
    {
        inventoryUI.SetActive(false);
    }

    // ī�װ� ��������ν� ���� ī�װ� ������ UIǥ��
    private void CallItemSlots(ItemType displayType)
    {
        nowDisplayItemType = displayType;
        inventoryManager.CallOnSetDisplayType(displayType);
    }

    // ����â ����
    public void TurnOffItemExplanationPopup()
    {
        itemExplanationPopup.SetActive(false);
    }

    //���� ī�װ��� ���� ���� ��ư�ؽ�Ʈ ���� �� ON, OFF
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
                tailUseButtonText.text = "�Һ�";
                break;
            case ItemType.Material:
                tailUseButton.SetActive(false);
                break;
            default:
                tailUseButton.SetActive(false);
                break;
        }
    }
    
    // ���� ��ư ��� OFF
    public void TurnOffInventoryTailUI()
    {
        inventoryTailButtonArea.SetActive(false);
    }

    //ī�װ� ��ư �������� �ؾ��ش� ���� ���� (�̺�Ʈ�� �ٲ㵵 ���������)
    public void OnCategoryButton(ItemType categoryType)
    {
        if (nowDisplayItemType == categoryType) return;
        InventoryUITurnOff();
        CallItemSlots(categoryType); //Slot�� ǥ�õǴ� Item �ٲٱ�
    }

    public void InventoryUITurnOff()
    {
        TurnOffItemExplanationPopup(); // ����â ����
        TurnOffInventoryTailUI(); // �κ��丮 UI ����
        inventoryManager.CallTurnOffItemClick(); //Ŭ���� �ߴ� UI ����
    }
    
    public void ButtonTextChange_Equip()
    {
        tailUseButtonText.text = "����";
    }

    public void ButtonTextChange_Unequip()
    {
        tailUseButtonText.text = "��� ����";
    }

    public void ActiveItemExplanationPopUp(ItemObject itemObject)
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
}
