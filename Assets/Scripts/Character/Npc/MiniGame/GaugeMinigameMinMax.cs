using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeMinigameMinMax : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        collision.rigidbody.velocity = Vector3.zero;
    }
}
