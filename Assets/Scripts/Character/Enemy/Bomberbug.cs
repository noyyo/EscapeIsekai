using UnityEngine;

public class Bomberbug : Enemy
{
    [Range(0f, 30f)][SerializeField] private float explosionRadius;
    [Tooltip("폭발시 검출할 레이어")]
    [SerializeField] private LayerMask layersToCollide;
    [Tooltip("폭발범위 내의 검색 가능한 최대 객체의 수")]
    [SerializeField] private int MaxCollisionObject = 20;
    [SerializeField] private int damage = 20;
    [SerializeField] private AttackEffectTypes attackEffectTypes;
    [SerializeField] private float attackEffectValue = 1f;

    private ExplosionManager explosionManager;

    private new void Awake()
    {
        base.Awake();
        explosionManager = ExplosionManager.Instance;
        AnimEventReceiver.AnimEventCalled += AnimationEventDecision;
    }

    private void AnimationEventDecision(AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "Explosion")
        {
            Explosion();
        }
    }

    public void Explosion()
    {
        explosionManager.Explosion(damage, attackEffectTypes, attackEffectValue, this.gameObject, this.transform.position, explosionRadius, layersToCollide, MaxCollisionObject);
    }
}
