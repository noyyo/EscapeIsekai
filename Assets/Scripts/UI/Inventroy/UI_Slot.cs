using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Slot : MonoBehaviour
{
    private GameObject _itemExplanationPopup;
    private TMP_Text[] _itemText;

    [SerializeField] private GameObject _outLine;


    private void Start()
    {
        _itemExplanationPopup = InventoryManager.Instance.ItemExplanationPopup;
        _itemText = _itemExplanationPopup.transform.GetComponentsInChildren<TMP_Text>();
    }

    public void SetActiveItemExplanationPopup(bool isActive, ItemData_Test itemData)
    {
        _itemExplanationPopup.SetActive(isActive);
        _itemText[0].text = itemData.ItemName;
        _itemText[1].text = "테스트로 직접 입력";
        _itemText[2].text = itemData.ItemExplanation;
    }

    public void DisplayItemClick()
    {
        _outLine.SetActive(true);
    }

    public void TurnOffItemClick()
    {
        _outLine.SetActive(false);
    }

    public void DisPlayEquip()
    {
        //장착 UI표시
    }
}
