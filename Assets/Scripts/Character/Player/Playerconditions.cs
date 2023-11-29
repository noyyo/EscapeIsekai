using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class Condition
{
    [HideInInspector]
    public float curValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    public Image uiBar;

    public bool isShieldActive;
    public float shieldValue;

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

    public void ActivateShield(float shieldAmount)
    {
        shieldValue = shieldAmount;
        isShieldActive = true;
    }

    public void DeActivateShield()
    {
        isShieldActive = false;
    }
}

public class PlayerStat
{
    public int Power { get; private set; } = 10;
    public int Guard { get; private set; } = 10;
}

public class Playerconditions : MonoBehaviour
{
    public Condition health;
    public Condition hunger;
    public Condition stamina;
    public Condition skill;
    public Condition powerUp;
    public Condition superJump;
    public Condition throwskill;
    public Condition noStamina;
    public Condition shield;
    public float noHungerHealthDecay;

    private bool nostaminaActive = false;

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

        if (hunger.curValue == 0.0f)
            health.Subtract(noHungerHealthDecay * Time.deltaTime);

        if (health.isShieldActive)
        {
            health.Subtract(health.decayRate * Time.deltaTime);
            if(health.curValue == 0.0f)
                health.DeActivateShield();
        }

        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
        skill.uiBar.fillAmount = skill.GetPercentage();
        powerUp.uiBar.fillAmount = powerUp.GetPercentage();
        superJump.uiBar.fillAmount = superJump.GetPercentage();
        throwskill.uiBar.fillAmount = throwskill.GetPercentage();
        noStamina.uiBar.fillAmount = noStamina.GetPercentage();
        shield.uiBar.fillAmount = shield.GetPercentage();
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
        if(!nostaminaActive && stamina.curValue - amount < 0)
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

    public void ActivateShield(float shieldAmount)
    {
        health.ActivateShield(shieldAmount);
    }

    public void DeActivateShield()
    {
        health.DeActivateShield();
    }

    public bool UseSkill(float amount)
    {
        if(skill.curValue - amount < 0)
            return false;

        skill.Subtract(amount);
        return true;
    }

    public bool ActivePowerUp(float amount)
    {
        if(powerUp.curValue - amount < 0)
            return false;

        powerUp.Subtract(powerUp.maxValue);
        return true;
    }

    public bool UseSuperJump(float amount)
    {
        if(superJump.curValue - amount <0)
            return false;

        superJump.Subtract(amount);
        return true;
    }
    public bool UseThrow(float amount)
    {
        if (throwskill.curValue - amount < 0)
            return false;

        throwskill.Subtract(amount);
        return true;
    }
    public bool UseNoStamina(float amount)
    {
        if (noStamina.curValue - amount < 0)
            return false;

        noStamina.Subtract(amount);
        return true;
    }
    public bool UseShield(float amount)
    {
        if (shield.curValue - amount < 0)
            return false;

        shield.Subtract(amount);
        return true;
    }
}
