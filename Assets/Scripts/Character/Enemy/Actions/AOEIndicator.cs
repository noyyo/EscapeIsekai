using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum AOETypes
{
    None = -1,
    Box,
    Circle,
    Circle45,
    Circle90,
    Circle180,
    Circle270,
}
public class AOEIndicator : MonoBehaviour
{
    [field: SerializeField] public AOETypes AOEType { get; private set; }
    private DecalProjector projector;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    private void Awake()
    {
        if (!TryGetComponent(out projector))
        {
            Debug.LogError("DecalProjector를 찾을 수 없습니다.");
        }
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    public void IndicateAOE(Vector3 startPosition, Vector3 forward, float radius)
    {
        Transform projectorTransform = projector.transform;
        forward.y = startPosition.y;
        projectorTransform.forward = forward;
        projectorTransform.SetPositionAndRotation(startPosition, projectorTransform.rotation * initialRotation);
        projectorTransform.localScale = new Vector3(initialScale.x * radius, initialScale.y * radius, initialScale.z);
    }
}
