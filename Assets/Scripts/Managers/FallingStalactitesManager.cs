using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

public enum FallingStalactitesTarget
{
    All,
    Player,
    Enemy
}


public class FallingStalactitesManager : CustomSingleton<FallingStalactitesManager>
{ 
    protected FallingStalactitesManager() { }

    [SerializeField] private int poolMaxCount = 10;
    private IObjectPool<FallingStalactites> objectPool_FallingStalactites;
    private GameObject stalactitesPrefab;
    private AOEIndicatorPool aoeIndicatorPool;
    private ObjectPool<AOEIndicator> objectPool_AOEIndicator;

    private void Awake()
    {
        aoeIndicatorPool = AOEIndicatorPool.Instance;
        stalactitesPrefab = Resources.Load<GameObject>("Prefabs/Entities/Environments/FallingStalactites");
        objectPool_FallingStalactites = new ObjectPool<FallingStalactites>(CreateFallingStalactites, OnGetFallingStalactites, OnReleasFallingStalactites, OnDestroyFallingStalactites, maxSize: poolMaxCount);
        objectPool_AOEIndicator = aoeIndicatorPool.GetIndicatorPool(AOETypes.Circle);
    }

    /// <summary>
    /// 돌을 떨어뜨립니다.
    /// </summary>
    /// <param name="transform">해당 transform의 밑에 생성됩니다.</param>
    /// <param name="vector3">돌의 위치(높이 포함)</param>
    /// <param name="speed">낙하속도</param>
    /// <param name="fallStartTime">준비시간</param>
    /// <param name="attackTarget">맞는객체 종류 : All, Player, Enemy</param>
    /// <param name="limitPosY">최하 Y의 높이 값을 입력안해도 0으로 입력되며 만약 땅의 높이가 0보다 높을시 OnTrigger을 사용해 자동으로 제거</param>
    public void CallFallingStalactites(Transform transform, Vector3 vector3, int newDamage, float speed, float fallStartTime, FallingStalactitesTarget attackTarget, float limitPosY = 0, float radius = 0, float depth = 0)
    {
        FallingStalactites stalactites = objectPool_FallingStalactites.Get();
        stalactites.SetAOEIndicator(objectPool_AOEIndicator.Get());
        stalactites.Falling(transform, vector3, newDamage, speed, fallStartTime, attackTarget, limitPosY);
    }

    private FallingStalactites CreateFallingStalactites()
    {
        FallingStalactites stalactites = Instantiate(stalactitesPrefab).GetComponent<FallingStalactites>();
        stalactites.SetManagedPool(objectPool_FallingStalactites);
        return stalactites;
    }

    private void OnGetFallingStalactites(FallingStalactites stalactites)
    {
        stalactites.gameObject.SetActive(true);
    }
    private void OnReleasFallingStalactites(FallingStalactites stalactites)
    {
        stalactites.gameObject.SetActive(false);
    }

    private void OnDestroyFallingStalactites(FallingStalactites stalactites)
    {
        Destroy(stalactites.gameObject);
    }

    public void OnRelease(AOEIndicator newAOEIndicator)
    {
        objectPool_AOEIndicator.Release(newAOEIndicator);
    }
}
