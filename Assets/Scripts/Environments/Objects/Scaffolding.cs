using System.Collections;
using System.Collections.Generic;
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

    [Header("LimitTime 설정용")]
    [SerializeField] private float limitTime = 3f;

    [Header("TakeDamage 설정용")]
    [SerializeField] private int maxHP = 100;

    private int hp;
    private int takeDamage;
    private bool isInvoke;
    // -1은 몬스터, 0은 초기화, 1은 플레이어
    private int attackerNumber;
    private WaitForSeconds breakWaitForSeconds;
    private WaitForSeconds respawnWaitForSeconds;
    private Collider thisCollider;

    private void Awake()
    {
        TryGetComponent<Collider>(out thisCollider);
        if(thisCollider == null)
            thisCollider = GetComponentInChildren<Collider>();
        breakWaitForSeconds = new WaitForSeconds(limitTime);
        respawnWaitForSeconds = new WaitForSeconds(respawnTime);
        Init();
    }

    public override void TakeDamage(int damage)
    {
        takeDamage = damage;
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {
        if (attacker.CompareTag(TagsAndLayers.PlayerTag))
            attackerNumber = 1;
        else if (attacker.CompareTag(TagsAndLayers.EnemyTag))
            attackerNumber = -1;
        else
            attackerNumber = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackerNumber == 1)
        {
            switch(type)
            {
                case ScaffoldingType.TakeDamage:
                    break;
                default:
                    if (!isInvoke)
                    {
                        StartCoroutine(IBreakScaffolding());
                        isInvoke = true;
                    }
                    break;
            }
        }
        else if(attackerNumber == -1)
        {
            switch (type)
            {
                case ScaffoldingType.LimitTime:
                    break;
                default:
                    Damage();
                    break;
            }
        }
        attackerNumber = 0;
    }

    private void Damage()
    {
        switch (type)
        {
            default:
                hp = hp - takeDamage;
                if (hp <= 0)
                {
                    hp = 0;
                    StopCoroutine(IBreakScaffolding());
                    BreakScaffolding();
                }
                else
                    PlayAnimationTakeDamage();
                takeDamage = 0;
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
        hp = maxHP;
        isInvoke = false;
    }

    private void PlayAnimationBreakScaffolding()
    {


        //임시
        Deactivate();
    }

    private void PlayAnimationTakeDamage()
    {

    }

    private void Activate()
    {
        this.gameObject.SetActive(true);
    }

    //애니메이션 끝나면 호출할것
    private void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
