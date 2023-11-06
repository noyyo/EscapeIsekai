using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftingSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemAvailability;
    [SerializeField] private GameObject _outLine;

    //슬롯 데이터 저장
    private ItemData_Test _itemData;
    private Image _icon;

    private Button _button;
    private bool isDisplay = false;

    //슬롯의 위치 한번 설정한 후 절대 바뀌지 않을 값
    private int _uniqueIndex = -1;
    public int UniqueIndex
    {
        get { return _uniqueIndex; }
        set
        {
            if (_uniqueIndex == -1)
                _uniqueIndex = value;
        }
    }

    private void Awake()
    {
        _icon = GetComponent<Image>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);
    }

    //목차버튼 누르면 슬롯을 활성화하기 위한 메서드
    public void TurnOnOffSlot()
    {
        isDisplay = !isDisplay;
        this.gameObject.SetActive(isDisplay);
    }

    //슬롯의 정보를 저장하기 위한 메서드
    public void SetSlot(ItemData_Test newItemData, bool isMaterials)
    {
        _itemData = newItemData;
        _icon.sprite = newItemData.Icon;
        _itemName.text = newItemData.ItemName;
        if (isMaterials)
            _itemAvailability.text = "제작이 가능합니다.";
        else
        {
            _itemAvailability.text = "제작이 불가능합니다.";
            _itemAvailability.color = Color.red;
        }
    }

    //버튼을 누르면 실행
    public void OnButtonClick()
    {
        //컨트롤러에 있는 Call@@을 호출해 제작관련 UI를 ON, 정보를 넘겨줌
        //컨트롤러에 있는 이벤트에 연결하여 다른 슬롯 클릭시 자동으로 해제되게 설정해줌
        _outLine.SetActive(true);
    }

    //자동해제를 위한 메서드
    public void OutLineTrunOff()
    {
        _outLine.SetActive(false);
    }
}
