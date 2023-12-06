using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bomberbug : Enemy
{
    [Range(0f, 30f)][SerializeField] private float explosionRadius;
    [Tooltip("���߽� ������ ���̾�")]
    [SerializeField] private LayerMask layersToCollide;
    [Tooltip("���߹��� ���� �˻� ������ �ִ� ��ü�� ��")]
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
        if(animationEvent.stringParameter == "Explosion")
        {
            Explosion();
        }
    }

    public void Explosion()
    {
        explosionManager.Explosion(damage, attackEffectTypes, attackEffectValue, this.gameObject, this.transform.position, explosionRadius, layersToCollide, MaxCollisionObject);
    }
}
