using TMPro;
using UnityEngine;

public class UI_Gathering : MonoBehaviour
{
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemExplanation;

    public void Setting()
    {
        _itemName.text = UI_Manager.Instance.itemName;
        _itemExplanation.text = UI_Manager.Instance.itemExplanation;
    }
}
