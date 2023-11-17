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

    private void Awake()
    {
        if (!TryGetComponent(out projector))
        {
            Debug.LogError("DecalProjector를 찾을 수 없습니다.");
        }
        initialRotation = transform.rotation;
    }

    public void IndicateAOE(Vector3 startPosition, Vector3 forward, float radius)
    {
        Transform projectorTransform = projector.transform;
        projectorTransform.position = startPosition;
        forward.y = 0;
        projectorTransform.forward = forward;
        projectorTransform.rotation = initialRotation;
        projectorTransform.localScale = new Vector3(radius, radius, 1);
    }
}
