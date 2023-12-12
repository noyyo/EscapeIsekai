using System.Collections;
using UnityEngine;
public enum ScaffoldingType
{
    All,
    LimitTime,
    TakeDamage
}

public class Scaffolding : BaseEnvironmentObject
{
    [SerializeField] private ScaffoldingType type;
    [SerializeField] private float respawnTime = 10f;

    [Header("LimitTime ������")]
    [SerializeField] private float limitTime = 3f;

    [Header("TakeDamage ������")]
    [SerializeField][ReadOnly] private int maxHP;
    private bool isInvoke;
    // -1�� ����, 0�� �ʱ�ȭ, 1�� �÷��̾�
    private int attackerNumber;
    private WaitForSeconds breakWaitForSeconds;
    private WaitForSeconds respawnWaitForSeconds;
    private Collider thisCollider;

    private void Awake()
    {
        maxHP = HP;
        TryGetComponent(out thisCollider);
        if (thisCollider == null)
            thisCollider = GetComponentInChildren<Collider>();
        breakWaitForSeconds = new WaitForSeconds(limitTime);
        respawnWaitForSeconds = new WaitForSeconds(respawnTime);
        Init();
    }

    public override void TakeDamage(int damage, GameObject attacker)
    {
        if (!CanTakeDamageAndEffect(attacker))
            return;
        Damage(damage);
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    { }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(TagsAndLayers.PlayerTag))
            return;
        if (!isInvoke && type != ScaffoldingType.TakeDamage)
        {
            StartCoroutine(IBreakScaffolding());
            isInvoke = true;
        }
    }

    private void Damage(int damage)
    {
        switch (type)
        {
            default:
                HP = HP - damage;
                if (HP <= 0)
                {
                    HP = 0;
                    StopCoroutine(IBreakScaffolding());
                    BreakScaffolding();
                }
                else
                    PlayAnimationTakeDamage();
                break;
        }
    }

    private IEnumerator IBreakScaffolding()
    {
        yield return breakWaitForSeconds;
        BreakScaffolding();
        StartCoroutine(RespawnScaffolding());
    }

    private IEnumerator RespawnScaffolding()
    {
        yield return respawnWaitForSeconds;
        Init();
        Activate();
    }

    private void BreakScaffolding()
    {
        thisCollider.enabled = false;
        PlayAnimationBreakScaffolding();
    }

    private void Init()
    {
        thisCollider.enabled = true;
        HP = maxHP;
        isInvoke = false;
    }

    private void PlayAnimationBreakScaffolding()
    {


        //�ӽ�
        Deactivate();
    }

    private void PlayAnimationTakeDamage()
    {

    }

    private void Activate()
    {
        gameObject.SetActive(true);
    }

    //�ִϸ��̼� ������ ȣ���Ұ�
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public override Vector3 GetObjectCenterPosition()
    {
        return thisCollider.bounds.center;
    }
}
