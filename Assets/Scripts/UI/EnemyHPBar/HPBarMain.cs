using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[Serializable]
public struct ChangeSize
{
    public float distance;
    public float sizePercent;
}

public class HPBarMain : MonoBehaviour
{
    [SerializeField] private int poolMaxCount = 30;
    [Range(0f, 30f)][SerializeField] private float extraHeight;
    [SerializeField] private List<ChangeSize> changeSizeRate; 
    private UI_Manager uiManager;
    private GameObject hpBarUIPrefab;
    private IObjectPool<EnemyHPBar> objectPool_EnemyHPBar;
    private BossHPBar bossHPBar;

    private void Awake()
    {
        uiManager = UI_Manager.Instance;
        objectPool_EnemyHPBar = new ObjectPool<EnemyHPBar>(CreateEnemyHPBar, OnGetEnemyHPBar, OnReleaseEnemyHPBar, OnDestroyEnemyHPBar, maxSize: poolMaxCount);
        hpBarUIPrefab = Resources.Load<GameObject>("");
    }

    private void Start()
    {
        uiManager.enemyHPBarUITurnOnEvent += PlaySFXReturnSource;
        bossHPBar = uiManager.BossHPBarUI.GetComponent<BossHPBar>();
    }

    private void PlaySFXReturnSource(Enemy enemy)
    {
        if (enemy.CompareTag(TagsAndLayers.EnemyTag))
        {
            EnemyHPBar hpBar = objectPool_EnemyHPBar.Get();
            hpBar.SetEnemyHPBar(enemy, extraHeight, changeSizeRate);
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
