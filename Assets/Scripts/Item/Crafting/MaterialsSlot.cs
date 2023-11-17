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
    private ItemDB _itemDB;
    private int _itemCount;
    private int _consumption;
    private ItemCraftingManager _craftingManager;

    private void Awake()
    {
        _itemDB = ItemDB.Instance;
        _icon = _image.GetComponent<Image>();
    }

    private void Start()
    {
        _craftingManager = ItemCraftingManager.Instance;
        _craftingManager.onCraftingEvent += UpdateItemData;
    }

    public void GetItemData(ItemData_Test newItem, int consumption, int count)
    {
        _icon.enabled = true;
        _icon.sprite = newItem.Icon;
        _itemCount = count;
        _consumption = consumption;
        UpdateText();
    }

    public void GetItemData(int id, int consumption, int count)
    {
        _icon.enabled = true;
        _itemDB.GetItemData(id, out ItemData_Test newItem);
        _icon.sprite = newItem.Icon;
        _itemCount = count;
        _consumption = consumption;
        UpdateText();
    }

    public void GetItemData(Sprite icon, int consumption, int count)
    {
        _icon.enabled = true;
        _icon.sprite = icon;
        _itemCount = count;
        _consumption = consumption;
        UpdateText();
    }

    public void UpdateItemData()
    {
        if(_itemCount >= _consumption)
            _itemCount -= _consumption;
        UpdateText();
    }

    public void UpdateText()
    {
        _text.text = _itemCount + " / " + _consumption;

        if (_consumption == 0)
            _text.text = "";

        if (_consumption > _itemCount)
            _text.color = Color.red;
        else
            _text.color = Color.black;
    }

    public void Init()
    {
        _icon.enabled = false;
        _text.text = "";
    }
}
