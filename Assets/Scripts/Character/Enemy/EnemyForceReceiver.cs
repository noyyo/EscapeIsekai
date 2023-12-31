using UnityEngine;

public class EnemyForceReceiver : MonoBehaviour
{
    public float Drag = 0.3f;

    private Vector3 dampingVelocity;
    private Vector3 impact;

    public Vector3 Movement => impact;

    void Update()
    {
        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, Drag);
    }

    public void Reset()
    {
        impact = Vector3.zero;
    }

    public void AddForce(Vector3 force)
    {
        impact += force;
    }
}

