using UnityEngine;

public class BreakableWall : BaseEnvironmentObject
{
    [SerializeField] private int hp = 100;
    [SerializeField] Collider thisCollider;

    private int defaultHP;
    private int damage;
    private bool isPlayer;

    private void Start()
    {
        defaultHP = hp;
        if (thisCollider == null)
            thisCollider = GetComponent<Collider>();
        if (thisCollider == null)
            thisCollider = GetComponentInChildren<Collider>();
        Init();
    }

    public override void TakeDamage(int damage)
    { this.damage = damage; }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    { isPlayer = attacker.CompareTag(TagsAndLayers.PlayerTag); }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            if (hp > 0)
                HPControl();
            isPlayer = false;
        }
    }

    private void HPControl()
    {
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            OnBreak();
            return;
        }
        PlayTakeDamageAnimation();
    }

    private void OnBreak()
    {
        thisCollider.enabled = false;
        PlayerBreakAnimation();

        //애니메이션이 끝나면 비활성화
        gameObject.SetActive(false);
    }

    private void PlayTakeDamageAnimation()
    {

    }

    private void PlayerBreakAnimation()
    {

    }

    public void Init()
    {
        hp = defaultHP;
        gameObject.SetActive(true);
        thisCollider.enabled = true;
    }

    public override Vector3 GetObjectCenterPosition()
    {
        return thisCollider.bounds.center;
    }
}
