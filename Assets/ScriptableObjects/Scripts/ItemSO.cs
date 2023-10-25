using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Equipment,
    Consumable,
    ETC,
    QuestItem
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObject/ItemSO", order = int.MaxValue)]
public class ItemSO : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private GameObject _dropprefab;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _itemName;
    [SerializeField] private string _itemExplanation;
    [SerializeField] private ItemType _itemType;

    public int ID { get { return _id; } }
    public GameObject DropPrefab { get { return _dropprefab; } }
    public Sprite Icon { get { return _icon; } }
    public string ItemName { get { return _itemName; } }
    public string ItemExplanation { get { return _itemExplanation; } }
    public ItemType ItemType { get { return _itemType; } }
}
