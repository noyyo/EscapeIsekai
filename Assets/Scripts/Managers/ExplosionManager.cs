using UnityEngine;
using UnityEngine.Pool;

public class ExplosionManager : CustomSingleton<ExplosionManager>
{
    protected ExplosionManager() { }
    [SerializeField] private int poolMaxCount = 5;

    private IObjectPool<ExplosionController> objectPool_ExplosionController;
    private GameObject prefab;

    private void Awake()
    {
        prefab = Resources.Load<GameObject>("Prefabs/Effect/Explosion");
        objectPool_ExplosionController = new ObjectPool<ExplosionController>(CreateExplosionController, OnGetExplosion, OnReleasExplosion, OnDestroyExplosion, maxSize: poolMaxCount);
    }

    public void Explosion(int damage, AttackEffectTypes attackEffectTypes, float attackEffectValue, GameObject attacker, Vector3 pos, float explosionRadius, LayerMask layer, int MaxCollisionObject = 20, float explosiondelayTime = 0)
    {
        ExplosionController controller = objectPool_ExplosionController.Get();
        controller.ExplosionInit(this.transform, pos, explosionRadius, layer, MaxCollisionObject, explosiondelayTime);
        controller.ExplosionInitEnemyData(damage, attackEffectTypes, attackEffectValue, attacker);
        controller.Explosion();
    }

    private ExplosionController CreateExplosionController()
    {
        ExplosionController explosion = Instantiate(prefab).GetComponent<ExplosionController>();
        explosion.SetManagedPool(objectPool_ExplosionController);
        return explosion;
    }

    private void OnGetExplosion(ExplosionController explosionController)
    {
        explosionController.gameObject.SetActive(true);
    }
    private void OnReleasExplosion(ExplosionController explosionController)
    {
        explosionController.gameObject.SetActive(false);
    }

    private void OnDestroyExplosion(ExplosionController explosionController)
    {
        Destroy(explosionController.gameObject);
    }
}
