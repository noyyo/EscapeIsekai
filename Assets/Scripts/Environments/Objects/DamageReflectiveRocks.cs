using UnityEngine;

public class DamageReflectiveRocks : BaseEnvironmentObject
{
    [SerializeField] private bool test;

    [Tooltip("대미지 배율 - 기본값 1")][Range(0.5f, 3f)][SerializeField] private float magnification = 1;

    [Tooltip("내가 원하는 값으로 커스텀 - 위의 배율도 적용됨")]
    [Header("Csutom")]
    [SerializeField] private bool isCustom;
    [SerializeField] private int customDamage;
    [SerializeField] private float customValue;
    [Tooltip("밑의 타입과 세트")][SerializeField] private bool lockAttackEffectTypes;
    [SerializeField] private AttackEffectTypes customAttackEffectTypes;
    [SerializeField] private bool isRespawn;
    [SerializeField] private float respawnTime;

    [Header("조건을 충족시 발생하는 모드")]
    [SerializeField] private bool isIF;
    [SerializeField] private string attackerName = "Worm";
    [SerializeField] private AttackEffectTypes attackerAttackEffectTypes;

    [Header("충돌시 낙하물 설정")]
    [SerializeField] private bool isFallingRock;
    [SerializeField] private int fallingDamage;
    [SerializeField] private int count;
    [Tooltip("낙하물의 위치를 랜덤하게 생성하기 위한 반지름")]
    [SerializeField] private float radius;
    [Tooltip("낙하물이 생성될 높이")][SerializeField] private float posY = 10;
    [SerializeField] private float speed = 5;
    [SerializeField] private float fallStartTime = 0;
    [SerializeField] private FallingStalactitesTarget attackTarget;
    [Tooltip("최하 높이 설정")][SerializeField] private float limitPosY = 0;

    private Collider collider;

    private int damage;
    private float value;
    private bool isBoss;
    private AttackEffectTypes attackEffectTypes;
    private FallingStalactitesManager fallingManager;
    private Vector3 basePosition;
    private Vector2 randomCircle;
    private Vector3 initialPosition;

    private void Awake()
    {
        fallingManager = FallingStalactitesManager.Instance;
        Init();
        basePosition = transform.position;
    }

    private void Update()
    {
        if (test)
        {
            FallingObject();
            test = false;
        }
    }

    private void Init()
    {
        collider = GetComponent<Collider>();
    }

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


        if (isIF)
        {
            if (attackEffectTypes == attackerAttackEffectTypes && (attacker.name == attackerName || attacker.CompareTag(TagsAndLayers.EnemyTag)))
                isBoss = true;
        }
        else
        {
            isBoss = attacker.CompareTag(TagsAndLayers.EnemyTag);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBoss)
        {
            IDamageable target = null;
            Enemy enemy;
            if (!other.TryGetComponent<Enemy>(out enemy))
                enemy = other.GetComponentInParent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError("적에게 Enemy컴포넌트가 없습니다.");
                return;
            }
            target = enemy.StateMachine;
            target?.TakeDamage(damage);
            target?.TakeEffect(attackEffectTypes, value, this.gameObject);
            isBoss = false;
            FallingObject();
            Deactivate();
        }
    }

    private void Respawn()
    {
        Init();
        Activate();
    }

    private void Activate()
    {
        this.gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        this.gameObject.SetActive(false);
        if (isRespawn)
            Invoke("Respawn", respawnTime);
    }

    private void FallingObject()
    {
        if (isFallingRock)
        {
            for (int i = 0; i < count; i++)
            {
                randomCircle = Random.insideUnitCircle * radius;
                initialPosition.x = basePosition.x + randomCircle.x;
                initialPosition.y = posY;
                initialPosition.z = basePosition.z + randomCircle.y;
                fallingManager.CallFallingStalactites(transform, initialPosition, fallingDamage, speed, fallStartTime, attackTarget, limitPosY);
            }
        }
    }

    public override Vector3 GetObjectCenterPosition()
    {
        return collider.bounds.center;
    }
}

