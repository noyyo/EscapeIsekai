using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder.AOV;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    private int damage = 0;
    public bool damageable = true;


    private void OnParticleCollision(GameObject other)
    {
        if (!damageable)
            return;

        if (other.tag == Tags.PlayerTag)
        {
            PlayerStateMachine stateMachine;
            other.TryGetComponent(out stateMachine);
            if (stateMachine == null)
            {
                Debug.LogError("�÷��̾�� stateMachine ������Ʈ�� �����ϴ�.");
                return;
            }

            damageable = false;
            stateMachine.TakeDamage(damage);
        }
        else if (other.tag == Tags.EnemyTag)
        {

        }
        else if (other.tag == Tags.EnvironmentTag)
        {

        }

    }
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
