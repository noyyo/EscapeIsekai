using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    private ItemData_Test itemData;
    private ItemStats itemStats;

    public int ID { get; private set; }
    public string Type { get; private set; }
    public string ItemName { get; private set; }
    public string ItemExplanation { get; private set; }
    public int Price { get; private set; }
    public int MaxCount { get; private set; }
    public bool IsStat { get; private set; }
    public bool IsCrafting { get; private set; }
    public bool IsSale { get; private set; }
    public GameObject DropPrefab { get; private set; }
    public Sprite Icon { get; private set; }
    public ItemData_Test ItemData { get { return itemData; } }
    public ItemStats ItemStats { get { return itemStats; } }

    public int Count { get; private set; }
    public bool IsEquip { get; set; }
    public bool IsMax { get; private set; }

    //기본 스텟
    public int DefaultHP { get; private set; }
    public int DefaultTemperature { get; private set; }
    public float DefaultATK { get; private set; }
    public float DefaultDEF { get; private set; }
    public float DefaultSpeed { get; private set; }
    public float DefaultStamina { get; private set; }
    public int DefaultHunger { get; private set; }

    public Dictionary<string, float> Stats { get; private set; }

    public Item(ItemData_Test data, ItemStats stats, int count)
    {
        itemData = data;
        itemStats = stats;
        ID = data.ID;
        Type = data.Type;
        ItemName = data.ItemName;
        ItemExplanation = data.ItemExplanation;
        Price = data.Price;
        MaxCount = data.MaxCount;
        IsStat = data.IsStat;
        IsCrafting = data.IsCrafting;
        IsSale = data.IsSale;
        DropPrefab = data.DropPrefab;
        Icon = data.Icon;

        if (IsStat)
        {
            DefaultHP = stats.HP;
            DefaultTemperature = stats.Temperature;
            DefaultATK = stats.ATK;
            DefaultDEF = stats.DEF;
            DefaultSpeed = stats.Speed;
            DefaultStamina = stats.Stamina;
            DefaultHunger = stats.Hunger;
        }
        Count = count;

        Stats = new Dictionary<string, float>()
                {
                    {"HP", (float)DefaultHP},
                    {"Temperature", (float)DefaultTemperature},
                    {"ATK", (float)DefaultATK},
                    {"DEF", (float)DefaultDEF},
                    {"Speed", (float)DefaultSpeed},
                    {"Stamina", (float)DefaultStamina},
                    {"Hunger", (int)DefaultHunger}
                };
    }

    public Item(Item item, int count)
    {
        itemData = item.itemData;
        itemStats = item.itemStats;
        ID = item.ID;
        Type = item.Type;
        ItemName = item.ItemName;
        ItemExplanation = item.ItemExplanation;
        Price = item.Price;
        MaxCount = item.MaxCount;
        IsStat = item.IsStat;
        IsCrafting = item.IsCrafting;
        IsSale = item.IsSale;
        DropPrefab = item.DropPrefab;
        Icon = item.Icon;

        if (IsStat)
        {
            DefaultHP = item.DefaultHP;
            DefaultTemperature = item.DefaultTemperature;
            DefaultATK = item.DefaultATK;
            DefaultDEF = item.DefaultDEF;
            DefaultSpeed = item.DefaultSpeed;
            DefaultStamina = item.DefaultStamina;
            DefaultHunger = item.DefaultHunger;
        }
        Count = count;

        Stats = new Dictionary<string, float>()
                {
                    {"HP", (float)DefaultHP},
                    {"Temperature", (float)DefaultTemperature},
                    {"ATK", (float)DefaultATK},
                    {"DEF", (float)DefaultDEF},
                    {"Speed", (float)DefaultSpeed},
                    {"Stamina", (float)DefaultStamina},
                    {"Hunger", (int)DefaultHunger}
                };
    }

    /// <summary>
    /// 값을 수정 후 초과된 값이나 부족분을 돌려주는 메서드 돌려줄 값이 있으며 반환값이 false
    /// </summary>
    /// <param name="count"></param>
    /// <param name="excessCount"></param>
    /// <returns></returns>
    public bool TryAddItem(int count, out int excessCount)
    {
        if (itemData.MaxCount < Count + count || 0 > Count + count)
        {
            if (count > 0)
            {
                excessCount = count + Count - itemData.MaxCount;
                Count = itemData.MaxCount;
                IsMax = true;
            }
            else
            {
                excessCount = count + Count;
                Count = 0;
            }
            return false;
        }
        else
        {
            excessCount = 0;
            Count += count;
            return true;
        }
    }

    /// <summary>
    /// 더해지는 값이 최대값의 초과 되거나 부족할 경우 바로 반환하는 메서드
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool TryAddItem(int count)
    {
        if (itemData.MaxCount < Count + count || 0 > Count + count)
        {
            return false;
        }
        else
        {
            Count += count;
            return true;
        }
    }
}
