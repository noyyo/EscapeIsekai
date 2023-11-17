using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryTailButtonArea;
    [SerializeField] private GameObject _itemExplanationPopup;
    [SerializeField] private Button[] _inventoryTypeButtons;
    [SerializeField] private Button[] _inventoryTailButtons;
    [SerializeField] private GameObject _tailUseButton;
    [SerializeField] private TMP_Text _tailUseButtonText;
    [SerializeField] private Button _backButton;

    private Inventory _inventory;
    private GameObject _inventory_GameObject;
    private InventoryManager _inventoryManager;
    private UI_Manager _ui_manager;
    private GameManager _gameManager;

    //���� ��µǰ� �ִ� ī�װ�
    private ItemType _nowDisplayItemType;

    public GameObject ItemExplanationPopup { get { return _itemExplanationPopup; } }

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _inventoryManager = InventoryManager.Instance;
        _ui_manager = UI_Manager.Instance;
        Init();
    }

    //���Ҽ��ִ� �� �ʱ�ȭ
    private void Init()
    {
        _nowDisplayItemType = ItemType.Equipment;

        if (_inventory_GameObject == null)
            _inventory_GameObject = this.gameObject;

        if (_inventoryTailButtonArea == null)
            _inventoryTailButtonArea = this.transform.GetChild(1).GetChild(1).gameObject;

        if (_itemExplanationPopup == null)
            _itemExplanationPopup = this.transform.GetChild(3).gameObject;

        if (_inventory == null)
            _inventory = _gameManager.Player.GetComponent<Inventory>();

        //��ư ����
        // ��� : 0, �Һ� : 1, ��� : 2, ��Ÿ : 3
        if(_inventoryTypeButtons.Length == 0)
            _inventoryTypeButtons = this.transform.GetChild(2).GetComponentsInChildren<Button>();

        //���� : 0, ������ : 1, ��� : 2
        if( _inventoryTypeButtons.Length == 0)
            _inventoryTailButtons = _inventoryTailButtonArea.GetComponentsInChildren<Button>();

        //�ڷΰ���
        if(_backButton == null)
            _backButton = this.transform.GetChild(1).GetChild(0).GetComponent<Button>();

        if(_tailUseButton == null)
            _tailUseButton = _inventoryTailButtons[2].gameObject;

        if(_tailUseButtonText == null)
            _tailUseButtonText = _tailUseButton.GetComponentInChildren<TMP_Text>();

        _inventoryTypeButtons[0].onClick.AddListener(() => { OnCategoryButton(ItemType.Equipment); });
        _inventoryTypeButtons[1].onClick.AddListener(() => { OnCategoryButton(ItemType.Consumable); });
        _inventoryTypeButtons[2].onClick.AddListener(() => { OnCategoryButton(ItemType.Material); });
        _inventoryTypeButtons[3].onClick.AddListener(() => { OnCategoryButton(ItemType.ETC); });

        _inventoryTailButtons[0].onClick.AddListener(_inventory.SortInventory);
        _inventoryTailButtons[1].onClick.AddListener(_inventory.Drop);
        _inventoryTailButtons[2].onClick.AddListener(_inventory.UseItem);

        _backButton.onClick.AddListener(_ui_manager.CallUI_InventoryTurnOff); //���ư���

        _ui_manager.UI_InventoryTurnOnEvent += InventroyUITurnOn;
        _ui_manager.UI_InventoryTurnOffEvent += InventroyUITurnOff;

        _inventoryManager.onTextChangeEquipEvent += ButtonTextChange_Equip;
        _inventoryManager.onTextChangeUnEquipEvent += ButtonTextChange_Unequip;
    }

    //�̺�Ʈ�� �ɸ� �޼���( �κ��丮 â ON, OFF)
    private void InventroyUITurnOn()
    {
        _inventory_GameObject.SetActive(true);
    }

    private void InventroyUITurnOff()
    {
        _inventory_GameObject.SetActive(false);
    }

    // ī�װ� ��������ν� ���� ī�װ� ������ UIǥ��
    private void CallItemSlots(ItemType displayType)
    {
        _nowDisplayItemType = displayType;
        _inventory.SetDisplayType(displayType);
    }

    // ����â ����
    public void TurnOffItemExplanationPopup()
    {
        _itemExplanationPopup.SetActive(false);
    }

    //���� ī�װ��� ���� ���� ��ư�ؽ�Ʈ ���� �� ON, OFF
    public void DisplayInventoryTailUI()
    {
        _inventoryTailButtonArea.SetActive(true);
        switch (_nowDisplayItemType)
        {
            case ItemType.Equipment:
                _tailUseButton.SetActive(true);
                if (_inventoryManager.ClickItem.equip)
                    ButtonTextChange_Unequip();
                else
                    ButtonTextChange_Equip();
                break;
            case ItemType.Consumable:
                _tailUseButton.SetActive(true);
                _tailUseButtonText.text = "�Һ�";
                break;
            case ItemType.Material:
                _tailUseButton.SetActive(false);
                break;
            default:
                _tailUseButton.SetActive(false);
                break;
        }
    }
    
    // ���� ��ư ��� OFF
    public void TurnOffInventoryTailUI()
    {
        _inventoryTailButtonArea.SetActive(false);
    }

    //ī�װ� ��ư �������� �ؾ��ش� ���� ���� (�̺�Ʈ�� �ٲ㵵 ���������)
    public void OnCategoryButton(ItemType categoryType)
    {
        if (_nowDisplayItemType == categoryType) return;
        InventoryUITurnOff();
        CallItemSlots(categoryType); //Slot�� ǥ�õǴ� Item �ٲٱ�
    }

    public void InventoryUITurnOff()
    {
        TurnOffItemExplanationPopup(); // ����â ����
        TurnOffInventoryTailUI(); // �κ��丮 UI ����
        _inventoryManager.CallTurnOffItemClick(); //Ŭ���� �ߴ� UI ����
    }
    
    public void ButtonTextChange_Equip()
    {
        _tailUseButtonText.text = "����";
    }

    public void ButtonTextChange_Unequip()
    {
        _tailUseButtonText.text = "��� ����";
    }
}
