using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Gathering : MonoBehaviour
{
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemExplanation;
    
    private void Start()
    {
        _itemName.text = UI_Manager.Instance.itemName;
        _itemExplanation.text = UI_Manager.Instance.itemExplanation;
    }
}
