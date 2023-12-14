using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Condition
{
    public event Action<Condition> ConditionUpdated;

    [field: SerializeField] public float curValue { get; private set; }
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    [ReadOnly] public Image uiBar;

    public Condition()
    {
        Init();
        ConditionUpdated?.Invoke(this);
    }
    public void Init()
    {
        curValue = startValue;
    }
    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
        ConditionUpdated?.Invoke(this);
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f);
        ConditionUpdated?.Invoke(this);
    }


    public float GetPercentage()
    {
        return curValue / maxValue;
    }

}


public class Playerconditions : MonoBehaviour
{
    [field: SerializeField] public int Power { get; set; } = 5;
    [field: SerializeField] public int Guard { get; private set; } = 0;
    public Condition health;
    public Condition hunger;
    public Condition rollCoolTime;
    public Condition stamina;
    public Condition skill;
    public Condition powerUp;
    public Condition superJump;
    public Condition throwskill;
    public Condition noStamina;
    public Condition shield;
    private InventoryManager inventoryManager;
    private readonly Player player;
    private bool nostaminaActive = false;
    [ReadOnly] public float noHungerHealthDecay;

    private void Awake()
    {
        Power = 5;
        Guard = 0;
    }
    private void Start()
    {
        Initialize();
    }
    private void Equip(Item item)
    {
        if (item.IsEquip)
        {
            Power += (int)item.DefaultATK;
            Guard += (int)item.DefaultDEF;
        }
    }

    private void UnEquip(Item item)
    {
        if (!item.IsEquip)
        {
            Power -= (int)item.DefaultATK;
            Guard -= (int)item.DefaultDEF;
        }
    }
    public void Initialize()
    {
        inventoryManager = InventoryManager.Instance;
        inventoryManager.OnEquipItemEvent += Equip;
        inventoryManager.UnEquipItemEvent += UnEquip;

        health.Init();
        hunger.Init();
        rollCoolTime.Init();
        stamina.Init();
        skill.Init();
        powerUp.Init();
        superJump.Init();
        throwskill.Init();
        noStamina.Init();
        shield.Init();
    }

    void Update()
    {
        hunger.Subtract(hunger.decayRate * Time.deltaTime);
        stamina.Add(stamina.regenRate * Time.deltaTime);
        skill.Add(skill.regenRate * Time.deltaTime);
        powerUp.Add(powerUp.regenRate * Time.deltaTime);
        superJump.Add(superJump.regenRate * Time.deltaTime);
        throwskill.Add(throwskill.regenRate * Time.deltaTime);
        noStamina.Add(noStamina.regenRate * Time.deltaTime);
        shield.Add(shield.regenRate * Time.deltaTime);
        rollCoolTime.Add(rollCoolTime.regenRate * Time.deltaTime);

        if (hunger.curValue == 0.0f)
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        if (!nostaminaActive && stamina.curValue < amount)
            return false;

        if (!nostaminaActive)
            stamina.Subtract(amount);

        return true;
    }

    public void ActiveNoStaminaBuff()
    {
        nostaminaActive = true;
    }

    public void DeActivateNoStaminaBuff()
    {
        nostaminaActive = false;
    }

    public bool RollCoolTime(float amount)
    {
        if (rollCoolTime.curValue < amount)
            return false;

        rollCoolTime.Subtract(rollCoolTime.maxValue);
        return true;
    }

    public bool UseSkill(float amount)
    {
        if (skill.curValue < amount)
            return false;

        skill.Subtract(skill.maxValue);
        return true;
    }

    public bool ActivePowerUp(float amount)
    {
        if (powerUp.curValue - amount < 0)
            return false;

        powerUp.Subtract(powerUp.maxValue);
        return true;
    }

    public bool UseSuperJump(float amount)
    {
        if (superJump.curValue - amount < 0)
            return false;

        superJump.Subtract(superJump.maxValue);
        return true;
    }
    public bool UseThrow(float amount)
    {
        if (throwskill.curValue - amount < 0)
            return false;

        throwskill.Subtract(throwskill.maxValue);
        return true;
    }
    public bool UseNoStamina(float amount)
    {
        if (noStamina.curValue - amount < 0)
            return false;

        noStamina.Subtract(noStamina.maxValue);
        return true;
    }
    public bool UseShield(float amount)
    {
        if (shield.curValue - amount < 0)
            return false;

        shield.Subtract(shield.maxValue);
        return true;
    }
}
