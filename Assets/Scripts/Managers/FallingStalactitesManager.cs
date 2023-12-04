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
    /// ���� ����߸��ϴ�.
    /// </summary>
    /// <param name="transform">�ش� transform�� �ؿ� �����˴ϴ�.</param>
    /// <param name="vector3">���� ��ġ(���� ����)</param>
    /// <param name="speed">���ϼӵ�</param>
    /// <param name="fallStartTime">�غ�ð�</param>
    /// <param name="attackTarget">�´°�ü ���� : All, Player, Enemy</param>
    /// <param name="limitPosY">���� Y�� ���� ���� �Է¾��ص� 0���� �ԷµǸ� ���� ���� ���̰� 0���� ������ OnTrigger�� ����� �ڵ����� ����</param>
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
