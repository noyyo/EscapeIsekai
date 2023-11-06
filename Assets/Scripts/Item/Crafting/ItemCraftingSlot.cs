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

    //���� ������ ����
    private ItemData_Test _itemData;
    private Image _icon;

    private Button _button;
    private bool isDisplay = false;

    //������ ��ġ �ѹ� ������ �� ���� �ٲ��� ���� ��
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

    //������ư ������ ������ Ȱ��ȭ�ϱ� ���� �޼���
    public void TurnOnOffSlot()
    {
        isDisplay = !isDisplay;
        this.gameObject.SetActive(isDisplay);
    }

    //������ ������ �����ϱ� ���� �޼���
    public void SetSlot(ItemData_Test newItemData, bool isMaterials)
    {
        _itemData = newItemData;
        _icon.sprite = newItemData.Icon;
        _itemName.text = newItemData.ItemName;
        if (isMaterials)
            _itemAvailability.text = "������ �����մϴ�.";
        else
        {
            _itemAvailability.text = "������ �Ұ����մϴ�.";
            _itemAvailability.color = Color.red;
        }
    }

    //��ư�� ������ ����
    public void OnButtonClick()
    {
        //��Ʈ�ѷ��� �ִ� Call@@�� ȣ���� ���۰��� UI�� ON, ������ �Ѱ���
        //��Ʈ�ѷ��� �ִ� �̺�Ʈ�� �����Ͽ� �ٸ� ���� Ŭ���� �ڵ����� �����ǰ� ��������
        _outLine.SetActive(true);
    }

    //�ڵ������� ���� �޼���
    public void TrunOff()
    {
        _outLine.SetActive(false);
    }
}
