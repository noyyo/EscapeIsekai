using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder.AOV;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public PlayerStateMachine health;
    public int _damage = 0;
    public bool damagable = true;


    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == Tags.PlayerTag)
        {
            if (health == null)
            {
                health = other.GetComponent<PlayerStateMachine>();
            }
            if (damagable)
            {
                damagable = false;
                health.TakeDamage(_damage);
            }
        }

    }
    public void SetDamage(int damage)
    {
        _damage = damage;
    }
}
