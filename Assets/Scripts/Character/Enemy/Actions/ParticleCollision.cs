using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder.AOV;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    //public GameObject Player;
    public PlayerStateMachine health;
    //µ¥¹ÌÁö, ÆÄÆ¼Å¬ ¾Þ±Û, »ö±ò
    public int _damage = 0;
    public bool damagable = true;
    private string playertag = "Player";


    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == playertag)
        {
            if (health == null)
            {
                health = other.GetComponent<PlayerStateMachine>();
            }
            if (damagable)
            {
                damagable = false;
                Debug.Log("asdf");
                health.TakeDamage(_damage);
            }
        }

    }
    public void SetDamage(int damage)
    {
        _damage = damage;
    }
}
