using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;


public class BreakableWall : BaseEnvironmentObject
{
    [SerializeField] private int _hp = 100;
    [SerializeField] Collider _collider;
    [SerializeField] private TagField _tagField;

    private int defaultHP;
    private int _damage;

    private void Start()
    {
        defaultHP = _hp;
        Init();
        if(_collider == null)
            _collider = GetComponent<Collider>();
    }

    public override void TakeDamage(int damage)
    {
        _damage = damage;
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Wapon")
        {
            Debug.Log("체력감소");
            Debug.Log(_hp);
            if (_hp > 0)
                HPControl();
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
