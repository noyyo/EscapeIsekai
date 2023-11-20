using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour, IDamageable
{
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;
    public Rigidbody rigid;
    public GameObject meshObj;
    public GameObject effectObj;

    public AffectedAttackEffectInfo AffectedEffectInfo => throw new NotImplementedException();

    public event Action OnDie;

    public void Init()
    {
        Debug.Log("Init 들어왔다");
        rigid = GetComponent<Rigidbody>();

        if (rigid != null)
        {
            // Vector3.up * (높이값) + transform.forward * 전진 힘 => 포물선
            float throwForce = 5f; // 던지는 힘 설정
            float throwHeight = 10f;

            rigid.AddForce(Vector3.up * throwHeight + transform.forward * throwForce, ForceMode.Impulse);
        }
        StartCoroutine(Explosion());
    }

    public void TakeDamage(int damage)
    {
        throw new NotImplementedException();
    }

    public void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {
        throw new NotImplementedException();
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitObj in rayHits)
        {
            //hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        // statemachine 이 idamageable 인터페이스 => takeeffect, takedamage 
        // attacker는 플레이어로, attackeffecttype은 넉백으로, value로 값 지정가능
        Destroy(gameObject, 3);
    }
}
