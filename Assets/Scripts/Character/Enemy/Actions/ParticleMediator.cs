using System;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class ParticleMediator : MonoBehaviour
{
    public event Action<GameObject> OnCollisionOccured;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("파티클 충돌 들어왔어");
        OnCollisionOccured?.Invoke(other);
    }
}
