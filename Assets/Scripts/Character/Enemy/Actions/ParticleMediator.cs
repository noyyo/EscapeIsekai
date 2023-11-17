using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder.AOV;
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
