using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBossOnWaterDead : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.OnTriggerEntered += OnTrrigerEntered;
    }
    private void OnTrrigerEntered(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            enemy.StateMachine.TakeDamage(9999, other.gameObject);
        }
    }
}
