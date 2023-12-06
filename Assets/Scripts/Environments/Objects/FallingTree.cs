using System.Collections;
using UnityEngine;

public class FallingTree : BaseEnvironmentObject
{
    [SerializeField][ReadOnly] private int maxHP;
    [SerializeField] private float objSpeed = 1f;

    [Range(0f, 8f)][SerializeField] private float playFadeOutTime = 2f;
    [SerializeField] private AttackEffectTypes attackType;
    [Range(0f, 30f)][SerializeField] private float effectValue = 2f;

    [Header("리스폰")]
    [SerializeField] private bool isRespawn = true;
    [SerializeField] private float respawnTime = 5f;

    private int playerDamage;
    private bool isPlayer;
    private bool isBreak;
    private Vector3 defaultPos;
    private Quaternion defaultRot;
    private Vector3 playerPos;
    private Vector2 direction;
    private float deg;
    private float speed;
    private Collider thisCollider;

    private void Awake()
    {
        maxHP = HP;
        if (TryGetComponent<Collider>(out thisCollider))
            thisCollider = GetComponentInChildren<Collider>();
        defaultPos = transform.position;
        defaultRot = transform.rotation;
    }

    private void Start()
    {
        Init();
    }

    public override void TakeDamage(int damage, GameObject attacker)
    { playerDamage = damage; }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {
        isPlayer = attacker.CompareTag(TagsAndLayers.PlayerTag);
        if (isPlayer)
            playerPos = attacker.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBreak)
        {
            if (other.CompareTag(TagsAndLayers.EnemyTag))
            {
                Enemy enemy;
                if (!other.gameObject.TryGetComponent<Enemy>(out enemy))
                    enemy = other.gameObject.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    IDamageable target = null;
                    target = enemy.StateMachine;
                    target?.TakeDamage(1, gameObject);
                    target?.TakeEffect(attackType, effectValue, this.gameObject);
                    isBreak = false;
                }
            }
        }
        else if (isPlayer)
        {
            isPlayer = false;
            ChangeHP();
        }
    }

    private void Init()
    {
        HP = maxHP;
        transform.position = defaultPos;
        transform.rotation = defaultRot;
        thisCollider.enabled = true;
        speed = objSpeed;
        isBreak = false;
        deg = 0;
    }

    private void ChangeHP()
    {
        HP -= playerDamage;
        playerDamage = 0;
        if (HP <= 0)
        {
            HP = 0;
            BreakTree();
        }
        else
            PlayAnimationTakeDamage();
    }

    private void BreakTree()
    {
        direction.Set(defaultPos.x - playerPos.x, defaultPos.z - playerPos.z);
        direction = direction.normalized;
        isBreak = true;
        StartCoroutine(Falling());
    }

    private IEnumerator Falling()
    {
        while (deg < 95)
        {
            deg += speed + Time.deltaTime;
            speed += 0.001f;
            transform.rotation = Quaternion.Euler(deg * direction.y, 0, deg * -direction.x);
            yield return null;
        }
        Invoke("PlayAnimationFadeOut", playFadeOutTime);
        yield break;
    }

    private void PlayAnimationTakeDamage()
    {

    }

    private void PlayAnimationFadeOut()
    {
        //임시
        Deactivate();
        thisCollider.enabled = false;
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

    public override Vector3 GetObjectCenterPosition()
    {
        return thisCollider.bounds.center;
    }
}
