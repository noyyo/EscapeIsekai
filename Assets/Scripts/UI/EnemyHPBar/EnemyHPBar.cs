using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyHPBar : MonoBehaviour
{
    private Enemy enemy;
    private Camera mainCamera;
    private UIBarScript uiBarScript;
    private Transform uiBarScriptTransform;
    private Transform mainCameraTransform;
    private Vector3 thisPos;
    private IObjectPool<EnemyHPBar> managedPool;
    private bool isSetted;
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
        if (isSetted && gameObject.activeInHierarchy)
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }
    public void SetManagedPool(IObjectPool<EnemyHPBar> pool)
    {
        managedPool = pool;
    }

    public void SetEnemyHPBar(Enemy newEnemy, float extraHeight)
    {
        enemy = newEnemy;
        transform.SetParent(newEnemy.transform);
        uiBarScript.UpdateValue(newEnemy.StateMachine.HP, newEnemy.Data.MaxHP);
        
        newEnemy.StateMachine.HpUpdated += uiBarScript.UpdateValue;
        newEnemy.StateMachine.ReleaseMonsterUI += OnRelease;
        SetUIPosition(newEnemy.transform, newEnemy.Agent.height, extraHeight);
        isSetted = true;
    }

    private void SetUIPosition(Transform enemyTransform, float enemyHeight, float extraHeight)
    {
        thisPos.Set(0, enemyHeight + extraHeight, 0);
        transform.localPosition = thisPos;
    }


    private void OnRelease()
    {
        enemy.StateMachine.HpUpdated -= uiBarScript.UpdateValue;
        enemy.StateMachine.ReleaseMonsterUI -= OnRelease;
        enemy = null;
        isSetted = false;
        thisPos = Vector3.zero;
        managedPool.Release(this);
    }
}
