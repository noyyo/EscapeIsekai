using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BreakableWall : BaseEnvironmentObject
{
    [SerializeField] private int _hp = 100;
    [SerializeField] Collider _collider;
    [SerializeField] string _colliderName = "Weapon";

    private int defaultHP;
    private int _damage;
    private bool _isTakeDamage;

    private void Start()
    {
        defaultHP = _hp;
        if (_collider == null)
            _collider = GetComponent<Collider>();
        if( _collider == null )
            _collider = GetComponentInChildren<Collider>();
        Init();
    }

    public override void TakeDamage(int damage)
    {
        _damage = damage;
        _isTakeDamage = true;
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == _colliderName && _isTakeDamage == true)
        {
            if (_hp > 0)
                HPControl();
            _isTakeDamage = false;
        }
    }

    private void HPControl()
    {
        _hp -= _damage;
        if( _hp <= 0 )
        {
            _hp = 0;
            OnBreak();
            return;
        }
        PlayTakeDamageAnimation();
    }

    private void OnBreak()
    {
        _collider.enabled = false;
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
        _hp = defaultHP;
        gameObject.SetActive(true);
        _collider.enabled = true;
    }
}
