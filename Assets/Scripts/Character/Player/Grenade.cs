using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;
    public Rigidbody rigid;
    public GameObject meshObj;
    public GameObject effectObj;
    public float explosionRadius = 5f; // 폭발 반경

    public void Init()
    {
        rigid = GetComponent<Rigidbody>();

        if (rigid != null)
        {
            // Vector3.up * (높이값) + transform.forward * 전진 힘 => 포물선
            float throwForce = 5f; // 던지는 힘 설정
            float throwHeight = 10f;
            rigid.AddForce(Vector3.up * throwHeight + transform.forward * throwForce * rigid.mass, ForceMode.Impulse);
        }
        StartCoroutine(Explosion());
    }


    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(2.5f);    // 나중에 데이터로 빼서 설정했으면 좋겠음 => 불가능. 공중체공시간이 존재해야함
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        // Physics.OverlapSphere
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider col in colliders)
        {
            //col.gameObject.CompareTag("Enemy");
            if(col.TryGetComponent(out Rigidbody targetRigidbody))
            {
                targetRigidbody.AddExplosionForce(1000f, transform.position, explosionRadius);
            }
        }

        // statemachine 이 idamageable 인터페이스 => takeeffect, takedamage
        // attacker는 플레이어로, attackeffecttype은 넉백으로, value로 값 지정가능
        Destroy(gameObject, 0);
    }

}
