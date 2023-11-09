using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour 
{
    [HideInInspector]
    public Vector3 targetDir;
    [HideInInspector]
    public UnityEvent hitBulletEvent;
    [SerializeField]
    private int bulletSpeed;

    public event Action<Collision,GameObject> ProjetileColliderEnter;

    private Collider colliders;
    private Rigidbody _rigidbody;
    private int bounceCount;
    private void OnCollisionEnter(Collision collision)
    {
        if(colliders.material == null)
        {
            StartCoroutine("WaitTime");
        }
        else
        {
            bounceCount++;
            if(bounceCount == 5)
            {
                Debug.Log("이제 사라진다");
                StartCoroutine("WaitTime");
            }
        }
        ProjetileColliderEnter?.Invoke(collision,this.gameObject);
    }
    private void Awake()
    {
        colliders = gameObject.GetComponent<BoxCollider>();
        if (colliders == null)
        {
            colliders = gameObject.GetComponent<CapsuleCollider>();
            if (colliders == null)
            {
                colliders = gameObject.GetComponent<MeshCollider>();
            }
        }
        _rigidbody = gameObject.GetComponent<Rigidbody>();

    }
    private void Start()
    {
        _rigidbody.AddForce(targetDir * bulletSpeed);
       
    }
    private void OnEnable()
    {
        bounceCount = 0;
        _rigidbody.AddForce(targetDir * bulletSpeed);
    }
    private void ReturnBulletToPool()
    {
        gameObject.SetActive(false);
        _rigidbody.velocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSecondsRealtime(1f);

        ReturnBulletToPool();
    }
}
