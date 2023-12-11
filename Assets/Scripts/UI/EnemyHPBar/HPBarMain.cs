using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HPBarMain : MonoBehaviour
{
    [SerializeField] private int poolMaxCount = 30;
    [Range(1f, 100f)][SerializeField] private int displayRange;
    private GameObject hpBarUIPrefab;
    private IObjectPool<EnemyHPBar> objectPool_EnemyHPBar;

    private void Awake()
    {
        objectPool_EnemyHPBar = new ObjectPool<EnemyHPBar>(CreateEnemyHPBar, OnGetEnemyHPBar, OnReleasEnemyHPBar, OnDestroyEnemyHPBar, maxSize: poolMaxCount);
        hpBarUIPrefab = Resources.Load<GameObject>("");
    }

    //Objectpool
    private EnemyHPBar CreateEnemyHPBar()
    {
        EnemyHPBar hPBar = Instantiate(hpBarUIPrefab, transform).GetComponent<EnemyHPBar>();
        hPBar.SetManagedPool(objectPool_EnemyHPBar);
        return hPBar;
    }

    private void OnGetEnemyHPBar(EnemyHPBar sfx)
    {
        sfx.gameObject.SetActive(true);
    }
    private void OnReleasEnemyHPBar(EnemyHPBar sfx)
    {
        sfx.gameObject.SetActive(false);
    }

    private void OnDestroyEnemyHPBar(EnemyHPBar sfx)
    {
        Destroy(sfx.gameObject);
    }
    //---------
}
