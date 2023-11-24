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
            Debug.LogError("DecalProjector를 찾을 수 없습니다.");
        }
        initialScale = transform.localScale;
        initialRotation = projector.transform.rotation;
    }
    /// <summary>
    /// 박스 형태의 AOE 범위를 표시합니다.
    /// </summary>
    /// <param name="startPosition">이펙트가 시작하는 위치입니다.</param>
    /// <param name="forward">indicator가 바라보는 방향입니다. isTopView가 true라면 forward의 x, z좌표 방향을 바라봅니다.</param>
    /// <param name="width">박스 이펙트의 너비입니다.</param>
    /// <param name="height">박스 이펙트의 높이입니다. depth를 따로 지정하지 않으면 해당 값이 depth에 쓰입니다.</param>
    /// <param name="depth">startPosition에서 indicator의 forward 방향으로의 깊이입니다. 이 방향으로 depth만큼의 거리 안에 Ground가 있어야 이펙트가 표시됩니다.</param>
    /// <param name="depth">startPosition에서 indicator의 forward 방향으로의 깊이입니다. 이 방향으로 depth만큼의 거리 안에 Ground가 있어야 이펙트가 표시됩니다.</param>
    public void IndicateBoxAOE(Vector3 startPosition, Vector3 forward, float width, float height, float depth = 0f, bool isTopView = true)
    {
        if (AOEType != AOETypes.Box)
        {
            Debug.Log("Indicator가 Box타입이 아닙니다.");
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
    /// 원형으로 이루어진 AOE 범위를 표시합니다.
    /// </summary>
    /// <param name="startPosition">이펙트가 시작하는 위치입니다.</param>
    /// <param name="forward">이펙트가 바라볼 방향입니다. Y축 방향은 무시됩니다.</param>
    /// <param name="radius">이펙트의 반경입니다.</param>
    /// <param name="depth">startPosition에서 indicator의 forward 방향으로의 깊이입니다. 기본 값은 radius와 같습니다.</param>
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
            Debug.Log("Indicator가 CircleAOE와 관련된 타입이 아닙니다.");
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
