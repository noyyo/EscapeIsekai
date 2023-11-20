using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private GameObject itemImage;
    [SerializeField] private TMP_Text text_Count;
    [SerializeField] private GameObject outLine;
    private Image backGround;

    //슬롯 데이터 저장
    private Image item2DImage;
    private ItemType itemType;
    private int count;
    private bool isEquip;

    //클릭을 위한 버튼
    private Button button;
    
    //드래그 앤 드롭 구현을 위해 필요한 변수들
    private Transform itemImageTransform;
    
    private Vector3 defaultPos; // 복귀를 위한 위치 저장
    private GameObject inventory_UI; // UI맨앞으로 바꾸기 위해 인벤토리 저장
    private Transform startParent; // 복귀를 위한 transform값 저장
    
    //드래그가 유효한지 저장하기위한 bool값
    private bool isData;

    private InventoryManager inventoryManager;
    private Color defaultColor;

    private float mousePosY;
    private Vector3 defaultContentPos;
    private Transform content;

    //슬롯의 위치 한번 설정한 후 절대 바뀌지 않을 값
    private int uniqueIndex = -1;
    public int UniqueIndex 
    {
        get { return uniqueIndex; }
        set 
        { 
           if(uniqueIndex == -1)
                uniqueIndex = value;
        } 
    }

    private void Awake()
    {
        inventoryManager = InventoryManager.Instance;
        item2DImage = itemImage.GetComponent<Image>();
        itemImageTransform = itemImage.transform;
        inventory_UI = UI_Manager.Instance.Inventory_UI;
        button = GetComponent<Button>();
        backGround = GetComponent<Image>();
        defaultColor = backGround.color;
    }

    private void Start()
    {
        content = this.transform.parent;
        button.onClick.AddListener(SlotClick);
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
        text_Count.text = String.Format("x {0}", count);
    }

    // 해당 슬롯 하나 삭제
    public void ClearSlot()
    {
        item2DImage.sprite = null;
        item2DImage.enabled = false;
        button.enabled = false;
        text_Count.text = "";
        text_Count.gameObject.SetActive(false);
        TurnOffItemClick();
        UnDisplayEquip();
    }

    private void SlotDisplay()
    {
        item2DImage.enabled = true;
        button.enabled = true;
        if (isEquip)
            DisplayEquip();
        else
            UnDisplayEquip();

        if (itemType != ItemType.Equipment)
        {
            text_Count.gameObject.SetActive(true);
            text_Count.text = String.Format("x {0}", count);
        }
    }

    private void SlotClick()
    {
        //클릭한 정보 매니저한태 전달
        inventoryManager.SetClickItem(uniqueIndex);

        //클릭한 모습 표시
        DisplayItemClick();

        //클릭했을때 인벤토리 아래쪽에 있는 버튼 활성화
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
            itemImageTransform.SetParent(inventory_UI.transform, false);
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

    //이후
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

    //먼저
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
