using UnityEngine;

public class BreakableWall : BaseEnvironmentObject
{
    private Collider thisCollider;
    private int defaultHP;
    private void Awake()
    {
        defaultHP = HP;
        thisCollider = GetComponent<Collider>();
        if (thisCollider == null)
            thisCollider = GetComponentInChildren<Collider>();
        OnDie += OnBreak;
        Init();
    }

    public override void TakeDamage(int damage, GameObject attacker)
    {
        if (!CanTakeDamageAndEffect(attacker))
            return;
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            return;
        }
        PlayTakeDamageAnimation();
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {

    }

    private void OnBreak()
    {
        thisCollider.enabled = false;
        PlayBreakAnimation();

        //애니메이션이 끝나면 비활성화
        gameObject.SetActive(false);
    }

    private void PlayTakeDamageAnimation()
    {

    }

    private void PlayBreakAnimation()
    {

    }

    public void Init()
    {
        HP = defaultHP;
        gameObject.SetActive(true);
        thisCollider.enabled = true;
    }

    public override Vector3 GetObjectCenterPosition()
    {
        return thisCollider.bounds.center;
    }
}
