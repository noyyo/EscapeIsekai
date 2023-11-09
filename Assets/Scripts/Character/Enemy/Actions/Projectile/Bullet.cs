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

    public Rigidbody _rigidbody;
    private void OnCollisionEnter(Collision collision)
    {
        ReturnBulletToPool();
        if (collision.transform.tag == Tags.PlayerTag)
        {
            hitBulletEvent?.Invoke();
        }
    }
    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        _rigidbody.AddForce(targetDir * bulletSpeed);
       
    }
    private void OnEnable()
    {
        _rigidbody.AddForce(targetDir * bulletSpeed);
    }
    private void ReturnBulletToPool()
    {
        gameObject.SetActive(false);
        _rigidbody.velocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
    }
}
