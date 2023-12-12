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
    private Transform uiBarScriptTransform;
    private Transform mainCameraTransform;

    public bool Test;
    private void Awake()
    {
        uiBarScript = GetComponentInChildren<UIBarScript>();
        uiBarScriptTransform = uiBarScript.transform;
        mainCamera = Camera.main;
        mainCameraTransform = mainCamera.transform;
        thisPos = Vector3.zero;
    }

    private void Update()
    {
        if (Test)
        {
            uiBarScript.transform.rotation = Quaternion.LookRotation(uiBarScriptTransform.position - mainCameraTransform.position);
        }
    }

    public void SetManagedPool(IObjectPool<EnemyHPBar> pool)
    {
        managedPool = pool;
    }

    public void SetEnemyHPBar(Enemy newEnemy, float extraHeight)
    {
        enemy = newEnemy;
        transform.parent = newEnemy.transform;
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
            uiBarScript.transform.rotation = Quaternion.LookRotation(uiBarScript.transform.position - mainCamera.transform.position);
            yield return null;
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
