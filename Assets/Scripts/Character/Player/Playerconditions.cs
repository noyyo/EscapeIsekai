using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public interface IDamagable
{
    void TakePhysicalDamage(int damage);
}

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

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue+ amount, maxValue);
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

public class Playerconditions : MonoBehaviour, IDamagable
{
    public Condition health;
    public Condition hunger;
    public Condition stamina;
    public Condition skill;
    public Condition powerUp;
    public event Action OnDie;
    public float noHungerHealthDecay;

    public UnityEvent onTakeDamage;


    void Start()
    {
        health.curValue = health.startValue;
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
        skill.curValue = skill.startValue;
        powerUp.curValue = powerUp.startValue;
    }

    void Update()
    {
        hunger.Subtract(hunger.decayRate * Time.deltaTime);
        stamina.Add(stamina.regenRate * Time.deltaTime);
        skill.Add(skill.regenRate * Time.deltaTime);
        powerUp.Add(powerUp.regenRate * Time.deltaTime);

        if (hunger.curValue == 0.0f)
            health.Subtract(noHungerHealthDecay * Time.deltaTime);

        if (health.curValue == 0.0f)
            Die();


        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
        skill.uiBar.fillAmount = skill.GetPercentage();
        powerUp.uiBar.fillAmount = powerUp.GetPercentage();
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
        if (stamina.curValue - amount < 0)
            return false;

        stamina.Subtract(amount);
        return true;
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

    public void Die()
    {
        OnDie?.Invoke();
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        onTakeDamage?.Invoke();
    }
}
