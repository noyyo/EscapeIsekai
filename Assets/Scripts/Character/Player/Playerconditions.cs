using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Condition
{
    public float curValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    [ReadOnly] public Image uiBar;

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }


    public float GetPercentage()
    {
        return curValue / maxValue;
    }

}


public class Playerconditions : MonoBehaviour
{
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
    [ReadOnly] public float noHungerHealthDecay;

    private InventoryManager inventoryManager;

    [field:SerializeField] public int Power { get; set; } = 5;
    [field: SerializeField] public int Guard { get; private set; } = 0;


    private bool nostaminaActive = false;

    private void Awake()
    {
        Power = 5;
        Guard = 0;
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



    public void Initialize(PlayerUI playerUI)
    {
        health.curValue = health.startValue;
        health.uiBar = playerUI.hpBar_Image;

        hunger.curValue = hunger.startValue;
        hunger.uiBar = playerUI.hunger_Image;

        stamina.curValue = stamina.startValue;
        stamina.uiBar = playerUI.stamina_Image;

        skill.curValue = skill.startValue;
        skill.uiBar = playerUI.Skill_Image;

        powerUp.curValue = powerUp.startValue;
        powerUp.uiBar = playerUI.powerUp_Image;

        superJump.curValue = superJump.startValue;
        superJump.uiBar = playerUI.SuperJump_Image;

        throwskill.curValue = throwskill.startValue;
        throwskill.uiBar = playerUI.Throw_Image;

        noStamina.curValue = noStamina.startValue;
        noStamina.uiBar = playerUI.NoStamina_Image;

        shield.curValue = shield.startValue;
        shield.uiBar = playerUI.Shield_Image;

        rollCoolTime.curValue = rollCoolTime.startValue;
        rollCoolTime.uiBar = playerUI.Roll_Image;

        inventoryManager = InventoryManager.Instance;
        inventoryManager.OnEquipItemEvent += Equip;
        inventoryManager.UnEquipItemEvent += UnEquip;
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


        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
        skill.uiBar.fillAmount = skill.GetPercentage();
        powerUp.uiBar.fillAmount = powerUp.GetPercentage();
        superJump.uiBar.fillAmount = superJump.GetPercentage();
        throwskill.uiBar.fillAmount = throwskill.GetPercentage();
        noStamina.uiBar.fillAmount = noStamina.GetPercentage();
        shield.uiBar.fillAmount = shield.GetPercentage();
        rollCoolTime.uiBar.fillAmount = rollCoolTime.GetPercentage();
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
        if (!nostaminaActive && stamina.curValue - amount < 0)
            return false;

        if (!nostaminaActive)
            stamina.curValue -= amount;

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
        if (rollCoolTime.curValue - amount < 0)
            return false;

        rollCoolTime.Subtract(rollCoolTime.maxValue);
        return true;
    }

    public bool UseSkill(float amount)
    {
        if (skill.curValue - amount < 0)
            return false;

        skill.Subtract(amount);
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
