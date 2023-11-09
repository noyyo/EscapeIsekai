using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class MaterialsSlot : MonoBehaviour
{
    [SerializeField] private GameObject _image;
    [SerializeField] private TMP_Text _text;
    private Image _icon;
    private int _consumption;
    private ItemDB _itemDB;

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        _icon = _image.GetComponent<Image>();
    }

    public void GetItemData(ItemData_Test newItem, int consumption, int count)
    {
        _icon.enabled = true;
        _icon.sprite = newItem.Icon;
        if(consumption != 0)
        {
            _consumption = consumption;
            _text.text = count + " / " + _consumption;
        }
        _text.text = "";
    }

    public void GetItemData(int id, int consumption, int count)
    {
        _icon.enabled = true;
        _itemDB.GetItemData(id, out ItemData_Test newItem);
        _icon.sprite = newItem.Icon;
        _text.text = "";
        if (consumption != 0)
        {
            _consumption = consumption;
            _text.text = count + " / " + _consumption;
        }
    }

    public void GetItemData(int id)
    {
        _icon.enabled = true;
        _itemDB.GetItemData(id, out ItemData_Test newItem);
        _icon.sprite = newItem.Icon;
        _text.text = "";
    }

    public void GetItemData(Sprite icon, int consumption, int count)
    {
        _icon.enabled = true;
        _icon.sprite = icon;
        _text.text = "";
        if (consumption != 0)
        {
            _consumption = consumption;
            _text.text = count + " / " + _consumption;
        }
        
    }

    public void UpdateText(int count)
    {
        _text.text = "";
        if (_consumption != 0)
        {
            _text.text = count + " / " + _consumption;
        }
    }

    public void Init()
    {
        _icon.enabled = false;
        _text.text = "";
    }
}
