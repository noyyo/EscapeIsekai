using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private ItemData_Test itemData;
    private ItemStats itemStats;

    public int ID { get; private set; }
    public string ItemName { get; private set; }
    public string ItemExplanation { get; private set; }
    public int Price { get; private set; }
    public int MaxCount { get; private set; }
    public bool IsStat { get; private set; }
    public bool IsCrafting { get; private set; }
    public GameObject DropPrefab { get; private set; }
    public Sprite Icon { get; private set; }
    public ItemData_Test ItemData { get { return itemData; } }
    public ItemStats ItemStats { get { return itemStats; } }

    public int Count { get; private set; }
    public bool IsEquip { get; set; }
    public bool IsMax { get; private set; }

    //�⺻ ����
    public int DefaultHP { get; private set; }
    public int DefaultTemperature { get; private set; }
    public float DefaultATK { get; private set; }
    public float DefaultDEF { get; private set; }
    public float DefaultSpeed { get; private set; }
    public float DefaultStamina { get; private set; }
    
    public Dictionary<string, float> Stats { get; private set; }

    public ItemObject(ItemData_Test data, ItemStats stats, int count)
    {
        itemData = data;
        itemStats = stats;
        ID = data.ID;
        ItemName = data.ItemName;
        ItemExplanation = data.ItemExplanation;
        Price = data.Price;
        MaxCount = data.MaxCount;
        IsStat = data.IsStat;
        IsCrafting = data.IsCrafting;
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
        }
        Count = count;

        Stats = new Dictionary<string, float>()
                {
                    {"HP", (float)DefaultHP},
                    {"Temperature", (float)DefaultTemperature},
                    {"ATK", (float)DefaultATK},
                    {"DEF", (float)DefaultDEF},
                    {"Speed", (float)DefaultSpeed},
                    {"Stamina", (float)DefaultStamina}
                };
    }

    /// <summary>
    /// ���� ���� �� �ʰ��� ���̳� �������� �����ִ� �޼��� ������ ���� ������ ��ȯ���� false
    /// </summary>
    /// <param name="count"></param>
    /// <param name="excessCount"></param>
    /// <returns></returns>
    public bool TryAddItem(int count, out int excessCount)
    {
        if (itemData.MaxCount <= Count + count || 0 > Count + count)
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
    /// �������� ���� �ִ밪�� �ʰ� �ǰų� ������ ��� �ٷ� ��ȯ�ϴ� �޼���
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool TryAddItem(int count)
    {
        if (itemData.MaxCount <= Count + count || 0 > Count + count)
        {
            return false;
        }
        else
        {
            Count += count;
            return true;
        }
    }

    /// <summary>
    /// �Ŵ������� Drop�����θ� ȣ��
    /// </summary>
    /// <param name="itemObject"></param>
    /// <param name="count"></param>
    public void GetData(ItemObject itemObject, int count)
    {
        itemData = itemObject.itemData;
        itemStats = itemObject.itemStats;
        ID = itemObject.ID;
        ItemName = itemObject.ItemName;
        ItemExplanation = itemObject.ItemExplanation;
        Price = itemObject.Price;
        MaxCount = itemObject.MaxCount;
        IsStat = itemObject.IsStat;
        IsCrafting = itemObject.IsCrafting;
        DropPrefab = itemObject.DropPrefab;
        Icon = itemObject.Icon;

        if (IsStat)
        {
            DefaultHP = itemObject.DefaultHP;
            DefaultTemperature = itemObject.DefaultTemperature;
            DefaultATK = itemObject.DefaultATK;
            DefaultDEF = itemObject.DefaultDEF;
            DefaultSpeed = itemObject.DefaultSpeed;
            DefaultStamina = itemObject.DefaultStamina;
        }
        Count = count;

        Stats = new Dictionary<string, float>()
                {
                    {"HP", (float)DefaultHP},
                    {"Temperature", (float)DefaultTemperature},
                    {"ATK", (float)DefaultATK},
                    {"DEF", (float)DefaultDEF},
                    {"Speed", (float)DefaultSpeed},
                    {"Stamina", (float)DefaultStamina}
                };
    }
}
