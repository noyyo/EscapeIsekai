using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HPBarMain : MonoBehaviour
{
    [SerializeField] private int poolMaxCount = 30;
    [Range(0f, 30f)][SerializeField] private float extraHeight;
    private UI_Manager uiManager;
    private GameObject hpBarUIPrefab;
    private IObjectPool<EnemyHPBar> objectPool_EnemyHPBar;
    private BossHPBar bossHPBar;

    private void Awake()
    {
        uiManager = UI_Manager.Instance;
        objectPool_EnemyHPBar = new ObjectPool<EnemyHPBar>(CreateEnemyHPBar, OnGetEnemyHPBar, OnReleaseEnemyHPBar, OnDestroyEnemyHPBar, maxSize: poolMaxCount);
        hpBarUIPrefab = Resources.Load<GameObject>("Prefabs/UI/EnemyHPBar/UI_EnemyHPBar");
    }

    private void Start()
    {
        uiManager.enemyHPBarUITurnOnEvent += SetEnemyHpBarByType;
        bossHPBar = uiManager.BossHPBarUI.GetComponent<BossHPBar>();
    }

    private void SetEnemyHpBarByType(Enemy enemy)
    {
        if (enemy.Data.ID > 100)
        {
            EnemyHPBar hpBar = objectPool_EnemyHPBar.Get();
            hpBar.SetEnemyHPBar(enemy, extraHeight);
        }
        else
        {
            bossHPBar.SetEnemyHPBar(enemy);
        }
    }

    //Objectpool
    private EnemyHPBar CreateEnemyHPBar()
    {
        EnemyHPBar hPBar = Instantiate(hpBarUIPrefab, transform).GetComponent<EnemyHPBar>();
        hPBar.SetManagedPool(objectPool_EnemyHPBar);
        return hPBar;
    }

    private void OnGetEnemyHPBar(EnemyHPBar hPBar)
    {
        hPBar.gameObject.SetActive(true);
    }
    private void OnReleaseEnemyHPBar(EnemyHPBar hPBar)
    {
        hPBar.gameObject.SetActive(false);
    }

    private void OnDestroyEnemyHPBar(EnemyHPBar hPBar)
    {
        Destroy(hPBar.gameObject);
    }
    //---------
}
