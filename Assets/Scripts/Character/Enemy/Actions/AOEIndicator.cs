using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum AOETypes
{
    None = -1,
    Box,
    Circle,
    Circle30,
    Circle45,
    Circle90,
    Circle180,
    Circle270,
}
public class AOEIndicator : MonoBehaviour
{
    [field: SerializeField] public AOETypes AOEType { get; private set; }
    private DecalProjector projector;
    private Vector3 initialScale;
    private Quaternion initialRotation;

    private void Awake()
    {
        if (!TryGetComponent(out projector))
        {
            Debug.LogError("DecalProjector�� ã�� �� �����ϴ�.");
        }
        initialScale = transform.localScale;
        initialRotation = projector.transform.rotation;
    }
    /// <summary>
    /// �ڽ� ������ AOE ������ ǥ���մϴ�.
    /// </summary>
    /// <param name="startPosition">����Ʈ�� �����ϴ� ��ġ�Դϴ�.</param>
    /// <param name="forward">indicator�� �ٶ󺸴� �����Դϴ�. isTopView�� true��� forward�� x, z��ǥ ������ �ٶ󺾴ϴ�.</param>
    /// <param name="width">�ڽ� ����Ʈ�� �ʺ��Դϴ�.</param>
    /// <param name="height">�ڽ� ����Ʈ�� �����Դϴ�. depth�� ���� �������� ������ �ش� ���� depth�� ���Դϴ�.</param>
    /// <param name="depth">startPosition���� indicator�� forward ���������� �����Դϴ�. �� �������� depth��ŭ�� �Ÿ� �ȿ� Ground�� �־�� ����Ʈ�� ǥ�õ˴ϴ�.</param>
    /// <param name="depth">startPosition���� indicator�� forward ���������� �����Դϴ�. �� �������� depth��ŭ�� �Ÿ� �ȿ� Ground�� �־�� ����Ʈ�� ǥ�õ˴ϴ�.</param>
    public void IndicateBoxAOE(Vector3 startPosition, Vector3 forward, float width, float height, float depth = 0f, bool isTopView = true)
    {
        if (AOEType != AOETypes.Box)
        {
            Debug.Log("Indicator�� BoxŸ���� �ƴմϴ�.");
            return;
        }
        if (depth <= 0f)
        {
            depth = height;
        }
        Transform projectorTransform = projector.transform;
        projectorTransform.position = startPosition;
        if (isTopView)
        {
            forward.y = 0;
            projectorTransform.forward = forward;
            projectorTransform.rotation = projectorTransform.rotation * initialRotation;
        }
        else
        {
            projectorTransform.forward = forward;
        }
        projectorTransform.localScale = new Vector3(initialScale.x * width, initialScale.y * height, depth);
    }
    /// <summary>
    /// �������� �̷���� AOE ������ ǥ���մϴ�.
    /// </summary>
    /// <param name="startPosition">����Ʈ�� �����ϴ� ��ġ�Դϴ�.</param>
    /// <param name="forward">����Ʈ�� �ٶ� �����Դϴ�. Y�� ������ ���õ˴ϴ�.</param>
    /// <param name="radius">����Ʈ�� �ݰ��Դϴ�.</param>
    /// <param name="depth">startPosition���� indicator�� forward ���������� �����Դϴ�. �⺻ ���� radius�� �����ϴ�.</param>
    public void IndicateCircleAOE(Vector3 startPosition, Vector3 forward, float radius, float depth = 0f)
    {
        bool isTypeError;
        isTypeError = AOEType switch
        {
            AOETypes.Circle => false,
            AOETypes.Circle30 => false,
            AOETypes.Circle45 => false,
            AOETypes.Circle90 => false,
            AOETypes.Circle180 => false,
            AOETypes.Circle270 => false,
            _ => true
        };
        if (isTypeError)
        {
            Debug.Log("Indicator�� CircleAOE�� ���õ� Ÿ���� �ƴմϴ�.");
            return;
        }
        if (depth <= 0f)
            depth = radius;
        Transform projectorTransform = projector.transform;
        projectorTransform.position = startPosition;
        forward.y = 0;
        projectorTransform.forward = forward;
        projectorTransform.rotation = projectorTransform.rotation * initialRotation;
        projectorTransform.localScale = new Vector3(initialScale.x * radius, initialScale.y * radius, depth);
    }
}
