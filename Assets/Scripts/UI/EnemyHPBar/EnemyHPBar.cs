using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyHPBar : MonoBehaviour
{
    private IObjectPool<EnemyHPBar> managedPool;
    private Enemy enemy;
    private Camera mainCamera;
    private Vector3 thisPos;
    private UIBarScript uiBarScript;
    private List<ChangeSize> changeSizeRate;

    private void Awake()
    {
        uiBarScript = GetComponentInChildren<UIBarScript>();
        mainCamera = Camera.main;
    }

    public void SetManagedPool(IObjectPool<EnemyHPBar> pool)
    {
        managedPool = pool;
    }

    public void SetEnemyHPBar(Enemy newEnemy, float extraHeight, List<ChangeSize> newchangeSizeRate)
    {
        enemy = newEnemy;
        transform.parent = newEnemy.transform;
        changeSizeRate = newchangeSizeRate;
        uiBarScript.UpdateValue(newEnemy.StateMachine.HP, newEnemy.Data.MaxHP);
        
        //newEnemy.changeHPEvent += uiBarScript.UpdateValue;
        //newEnemy.onReleaseHPBar += OnRelease;
        SetUIPosition(newEnemy.transform, newEnemy.Agent.height, extraHeight);
        StartCoroutine(SetUIRotion());
    }

    private void SetUIPosition(Transform enemyTransform, float enemyHeight, float extraHeight)
    {
        thisPos.Set(0, enemyHeight + extraHeight, 0);
        transform.position = thisPos;
    }

    private IEnumerator SetUIRotion()
    {
        while (true)
        {
            transform.rotation = Quaternion.LookRotation(mainCamera.transform.position);
            UpdateSize();
            yield return null;
        }
    }

    private void UpdateSize()
    {
        foreach (ChangeSize i in changeSizeRate)
        {
            if (i.distance >= enemy.StateMachine.TargetDistance)
            {
                uiBarScript.gameObject.transform.localScale = uiBarScript.gameObject.transform.localScale * i.sizePercent;
            }
        }
    }

    private void OnRelease()
    {
        //newEnemy.changeHPEvent -= uiBarScript.UpdateValue;
        //newEnemy.onReleaseHPBar -= OnRelease;
        enemy = null;
        thisPos = Vector3.zero;
        StopCoroutine(SetUIRotion());
        managedPool.Release(this);
    }
}
