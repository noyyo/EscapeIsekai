using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHPBar : MonoBehaviour
{
    private Enemy enemy;
    private UIBarScript uiBarScript;

    private void Awake()
    {
        uiBarScript = GetComponent<UIBarScript>();
    }

    public void SetEnemyHPBar(Enemy newEnemy)
    {
        enemy = newEnemy;
        uiBarScript.UpdateValue(newEnemy.StateMachine.HP, newEnemy.Data.MaxHP);
        Activate();
        //newEnemy.changeHPEvent += uiBarScript.UpdateValue;
        //newEnemy.onReleaseHPBar += OnRelease;
    }

    private void OnRelease()
    {
        //newEnemy.changeHPEvent -= uiBarScript.UpdateValue;
        //newEnemy.onReleaseHPBar -= OnRelease;
        enemy = null;
        Deactivate();
    }

    private void Activate()
    {
        gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
