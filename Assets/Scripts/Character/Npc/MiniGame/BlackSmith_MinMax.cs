using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackSmith_MinMax : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        collision.rigidbody.velocity = Vector3.zero;
    }
}
