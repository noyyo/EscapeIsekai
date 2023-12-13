using System;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class ParticleMediator : MonoBehaviour
{
    public event Action<GameObject> OnCollisionOccured;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("��ƼŬ �浹 ���Ծ�");
        OnCollisionOccured?.Invoke(other);
    }
}
