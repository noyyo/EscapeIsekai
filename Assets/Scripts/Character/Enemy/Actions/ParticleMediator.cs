using System;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class ParticleMediator : MonoBehaviour
{
    public event Action<GameObject> OnCollisionOccured;

    private void OnParticleCollision(GameObject other)
    {
        OnCollisionOccured?.Invoke(other);
    }
}
