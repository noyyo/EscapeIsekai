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
    public float explosionRadius = 5f; // ���� �ݰ�

    public void Init()
    {
        rigid = GetComponent<Rigidbody>();

        if (rigid != null)
        {
            // Vector3.up * (���̰�) + transform.forward * ���� �� => ������
            float throwForce = 5f; // ������ �� ����
            float throwHeight = 10f;
            rigid.AddForce(Vector3.up * throwHeight + transform.forward * throwForce * rigid.mass, ForceMode.Impulse);
        }
        StartCoroutine(Explosion());
    }


    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(2.5f);    // ���߿� �����ͷ� ���� ���������� ������ => �Ұ���. ����ü���ð��� �����ؾ���
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

        // statemachine �� idamageable �������̽� => takeeffect, takedamage
        // attacker�� �÷��̾��, attackeffecttype�� �˹�����, value�� �� ��������
        Destroy(gameObject, 0);
    }

}
