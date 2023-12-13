using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private GameObject itemImage;
    [SerializeField] private GameObject outLine;
    [SerializeField] private TMP_Text textItemCount;

    private UI_Manager uiManager;
    private InventoryManager inventoryManager;
    private GameObject inventoryUI;
    private Transform itemImageTransform;
    private Transform startParent;
    private Transform content;
    private Button clickButton;
    private Image backGround;
    private Image item2DImage;
    private Vector3 defaultPos;
    private Vector3 defaultContentPos;
    private Color defaultColor;
    private ItemType itemType;
    private float mousePosY;
    private int count;
    private bool isEquip;
    private bool isData;

    //슬롯의 위치 한번 설정한 후 절대 바뀌지 않을 값
    private int uniqueIndex = -1;
    public int UniqueIndex
    {
        get { return uniqueIndex; }
        set
        {
            if (uniqueIndex == -1)
                uniqueIndex = value;
        }
    }

    private void Awake()
    {
        inventoryManager = InventoryManager.Instance;
        uiManager = UI_Manager.Instance;
        item2DImage = itemImage.GetComponent<Image>();
        clickButton = GetComponent<Button>();
        backGround = GetComponent<Image>();
        inventoryUI = uiManager.Inventory_UI;
        itemImageTransform = itemImage.transform;
        defaultColor = backGround.color;
    }

    private void Start()
    {
        content = transform.parent;
        clickButton.onClick.AddListener(SlotClick);
    }

    public void AddItem(Item item)
    {
        CheckItemType(item.ID);
        item2DImage.sprite = item.Icon;
        isEquip = item.IsEquip;
        count = item.Count;
        SlotDisplay();
    }

    private void CheckItemType(int id)
    {
        if (id >= 10300000)
            itemType = ItemType.ETC;
        else if (id >= 10200000)
            itemType = ItemType.Material;
        else if (id >= 10100000)
            itemType = ItemType.Consumable;
        else
            itemType = ItemType.Equipment;
    }

    /// <summary>
    /// 입력된 값(count)으로 변경 더하기나 빼기가 아닌 =입니다.
    /// </summary>
    /// <param name="count"></param>
    public void SetSlotCount(int newCount)
    {
        count = newCount;
        textItemCount.text = String.Format("x {0}", count);
    }

    public void ClearSlot()
    {
        item2DImage.sprite = null;
        item2DImage.enabled = false;
        clickButton.enabled = false;
        textItemCount.text = "";
        textItemCount.gameObject.SetActive(false);
        TurnOffItemClick();
        UnDisplayEquip();
    }

    private void SlotDisplay()
    {
        item2DImage.enabled = true;
        clickButton.enabled = true;
        if (isEquip)
            DisplayEquip();
        else
            UnDisplayEquip();

        if (itemType != ItemType.Equipment)
        {
            textItemCount.gameObject.SetActive(true);
            textItemCount.text = String.Format("x {0}", count);
        }
    }

    private void SlotClick()
    {
        uiManager.PlayClickBtnSound();
        inventoryManager.SetClickItem(uniqueIndex);

        DisplayItemClick();

        inventoryManager.CallDisplayInventoryTailUI(isEquip);
        inventoryManager.CallOnItemExplanationPopUp();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (count > 0)
        {
            isData = true;
            defaultPos = itemImageTransform.position;
            startParent = itemImageTransform.parent;
            itemImageTransform.SetParent(inventoryUI.transform, false);
            uiManager.PlayClickSound();
        }
        else
        {
            defaultContentPos = content.transform.position;
            mousePosY = eventData.position.y;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isData)
            itemImage.transform.position = eventData.position;
        else
        {
            defaultContentPos.y += eventData.position.y - mousePosY;
            mousePosY = eventData.position.y;
            content.position = defaultContentPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isData)
        {
            itemImageTransform.SetParent(startParent, false);
            itemImageTransform.SetAsFirstSibling();
            itemImage.transform.position = defaultPos;

            defaultPos = Vector3.zero;
            startParent = null;
            isData = false;

            inventoryManager.CallChangeSlot(uniqueIndex);
        }
        else
        {
            defaultContentPos = Vector3.zero;
            mousePosY = 0;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        inventoryManager.SaveNewChangedSlot(uniqueIndex);
    }

    public void DisplayItemClick()
    {
        outLine.SetActive(true);
    }

    public void TurnOffItemClick()
    {
        outLine.SetActive(false);
    }

    public void DisplayEquip()
    {
        isEquip = true;
        backGround.color = Color.blue;
    }

    public void UnDisplayEquip()
    {
        isEquip = false;
        backGround.color = defaultColor;
    }
}
