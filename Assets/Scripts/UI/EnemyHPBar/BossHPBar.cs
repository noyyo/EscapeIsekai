using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHPBar : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] private UIBarScript uiBarScript;

    public void SetEnemyHPBar(Enemy newEnemy)
    {
        enemy = newEnemy;
        uiBarScript.UpdateValue(newEnemy.StateMachine.HP, newEnemy.Data.MaxHP);
        Activate();
        enemy.StateMachine.HpUpdated += uiBarScript.UpdateValue;
        enemy.StateMachine.ReleaseMonsterUI += OnRelease;
    }

    private void OnRelease()
    {
        enemy.StateMachine.HpUpdated -= uiBarScript.UpdateValue;
        enemy.StateMachine.ReleaseMonsterUI -= OnRelease;
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
