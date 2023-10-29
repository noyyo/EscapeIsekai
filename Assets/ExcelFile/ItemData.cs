using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum ItemType
{
    Equipment,
    Consumable,
    Material,
    ETC
}

[Serializable]
public class ItemData
{
    [SerializeField] private int _id;
    [SerializeField] private string _itemName;
    [SerializeField] private string _itemExplanation;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private int _price;
    [SerializeField] private int _maxCount;
    [SerializeField] private bool _isQuestItem;
    [SerializeField] private string _dropPrefabPath;
    [SerializeField] private string _iconPath;
    
    private GameObject _dropPrefab;
    private Sprite _icon;

    public int ID { get { return _id; } }
    public string ItemName { get { return _itemName; } }
    public string ItemExplanation { get { return _itemExplanation; } }
    public ItemType ItemType { get { return _itemType; } }
    public int Price { get { return _price; } }
    public int MaxCount { get { return _maxCount; } }
    public bool IsQuestItem { get { return _isQuestItem; } }
    public GameObject DropPrefab
    {
        get
        {
            if(_dropPrefab == null)
            {
                _dropPrefab = Resources.Load(_dropPrefabPath) as GameObject;
            }

            return _dropPrefab;
        }
    }

    public Sprite Icon
    {
        get
        {
            if (_dropPrefab == null)
            {
                _icon = Resources.Load(_iconPath) as Sprite;
            }

            return _icon;
        }
    }

    public ItemData(int id, string itemName, string itemExplanation, ItemType itemType, int price, int maxCount, string dropPrefabPath, string iconPath)
    {
        _id = id;
        _itemName = itemName;
        _itemType = itemType;
        _price = price;
        _maxCount = maxCount;
        _dropPrefabPath = dropPrefabPath;
        _iconPath = iconPath;
    }
}