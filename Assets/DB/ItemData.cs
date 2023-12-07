using System;
using UnityEngine;

public enum ItemType
{
    Equipment,
    Consumable,
    Material,
    ETC,
    Quest
}

[Serializable]
public class ItemData
{
    [SerializeField] private int id;
    [SerializeField] private string type;
    [SerializeField] private string itemName;
    [SerializeField] private string itemExplanation;
    [SerializeField] private int price;
    [SerializeField] private int maxCount;
    [SerializeField] private string dropPrefabPath;
    [SerializeField] private string iconPath;
    [SerializeField] private bool isStat;
    [SerializeField] private bool isCrafting;
    [SerializeField] private bool isSale;

    private GameObject dropPrefab;
    private Sprite icon;

    public int ID { get { return id; } }
    public string Type { get { return type; } }
    public string ItemName { get { return itemName; } }
    public string ItemExplanation { get { return itemExplanation; } }
    public int Price { get { return price; } }
    public int MaxCount { get { return maxCount; } }
    public bool IsStat { get { return isStat; } }
    public bool IsCrafting { get { return isCrafting; } }
    public bool IsSale { get { return isSale; } }
    public GameObject DropPrefab
    {
        get
        {
            if (dropPrefab == null)
                dropPrefab = Resources.Load<GameObject>(dropPrefabPath);
            return dropPrefab;
        }
    }

    public Sprite Icon
    {
        get
        {
            if (icon == null)
                icon = Resources.Load<Sprite>(iconPath);
            return icon;
        }
    }
}