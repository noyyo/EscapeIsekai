using UnityEngine;

public class DamageReflectiveRocks : BaseEnvironmentObject
{

    [Tooltip("대미지 배율 - 기본값 1")][SerializeField] private float magnification = 1;

    [Tooltip("내가 원하는 값으로 커스텀 - 위의 배율도 적용됨")][Header("Csutom")]
    [SerializeField] bool isCustom;
    [SerializeField] private int customDamage;
    [SerializeField] private float customValue;
    [SerializeField] private bool lockAttackEffectTypes;
    [SerializeField] private AttackEffectTypes customAttackEffectTypes;
    
    private int damage;
    private float value;
    private AttackEffectTypes attackEffectTypes;

    private void Start()
    {
        damage = (int)(customDamage * magnification);
        value = customValue;
        attackEffectTypes = customAttackEffectTypes;
    }

    public override void TakeDamage(int damage)
    {
        if (!isCustom)
            this.damage = (int)(damage * magnification);
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {
        if (!isCustom)
            this.value = value;
        if (!lockAttackEffectTypes)
            this.attackEffectTypes = attackEffectTypes;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = null;
        if (other.tag == TagsAndLayers.EnemyTag)
        {
            Enemy enemy;
            enemy = other.GetComponentInParent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError("적에게 Enemy컴포넌트가 없습니다.");
                return;
            }
            target = enemy.StateMachine;
            target?.TakeDamage(damage);
            target?.TakeEffect(attackEffectTypes, value, this.gameObject);
        }
        
    }
}

