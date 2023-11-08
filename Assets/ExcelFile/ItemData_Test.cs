using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ItemEquipmentType
{
    //���� 0 ~ 9, ���� 10 ~ 19, ��ű� 20 ~ 29
    Head,
    Top,
    Bottoms,
    Shoes,
    Gloves,
    OneHandedWeapon = 10,
    TwoHandedWeapon = 11,
    Accessories = 20
}

public enum ItemConsumableType
{
    HpHeal,
    Recipe = 10
}

public enum ItemType
{
    Equipment,
    Consumable,
    Material,
    ETC,
    Quest
}

[Serializable]
public class ItemData_Test
{
    [SerializeField] private int _id;
    [SerializeField] private string _itemName;
    [SerializeField] private string _itemExplanation;
    [SerializeField] private int _price;
    [SerializeField] private int _maxCount;
    [SerializeField] private string _dropPrefabPath;
    [SerializeField] private string _iconPath;
    [SerializeField] private bool _isStat;
    [SerializeField] private bool _isCrafting;

    private GameObject _dropPrefab;
    private Sprite _icon;

    public int ID { get { return _id; } }
    public string ItemName { get { return _itemName; } }
    public string ItemExplanation { get { return _itemExplanation; } }
    public int Price { get { return _price; } }
    public int MaxCount { get { return _maxCount; } }
    public bool IsStat { get { return _isStat; } }
    public bool IsCrafting { get { return _isCrafting; } }

    public GameObject DropPrefab
    {
        get
        {
            if(_dropPrefab == null)
            {
                _dropPrefab = Resources.Load<GameObject>(_dropPrefabPath);
            }

            return _dropPrefab;
        }
    }

    public Sprite Icon
    {
        get
        {
            if (_icon == null)
            {
                _icon = Resources.Load<Sprite>(_iconPath);
            }

            return _icon;
        }
    }
}