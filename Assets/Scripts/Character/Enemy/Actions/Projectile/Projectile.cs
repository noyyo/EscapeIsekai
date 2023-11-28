using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum ProjectileLaunchTypes
{
    Shoot,
    Drop,
}
public class Projectile : MonoBehaviour
{
    public event Action<Collider, Projectile> ProjetileColliderEnter;
    public event Action<Projectile> TimeExpired;
    // �߻��� Enemy��ü.
    public Enemy Launcher { get; private set; }
    [Tooltip("�ش� �ð��� ���� ������ �ε����� �ʴ´ٸ� �ڵ����� �Ҹ�˴ϴ�.")]
    [SerializeField][Range(1f, 20f)] private float disappearTime = 5f;
    private ProjectileLaunchTypes launchType;
    private AOETypes indicatorAOEType;
    private bool isInfoSetted;
    private float timeSinceLaunced;
    private new Rigidbody rigidbody;
    private new Collider collider;
    private Vector3 colliderSize;
    private Vector3 direction;
    private float launchSpeed;
    private AOEIndicator indicator;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        colliderSize = collider.bounds.extents * 2;
    }
    /// <summary>
    /// ����ü�� �߻�� ������ �����մϴ�.
    /// </summary>
    /// <param name="launcher">�߻��ϴ� Enemy��ü�Դϴ�.</param>
    public void SetProjectileInfo(ProjectileLaunchTypes launchType, Vector3 position, Vector3 direction, float launchSpeed, Enemy launcher)
    {
        this.launchType = launchType;
        switch (launchType)
        {
            case ProjectileLaunchTypes.Shoot:
                indicatorAOEType = AOETypes.Box;
                break;
            case ProjectileLaunchTypes.Drop:
                indicatorAOEType = AOETypes.Circle;
                break;
            default:
                indicatorAOEType = AOETypes.None;
                break;
        }
        this.direction = direction;
        this.launchSpeed = launchSpeed;
        Launcher = launcher;
        transform.position = position;
        transform.forward = direction;
        SetIndicator();
        isInfoSetted = true;
    }
    public void IndicateBoxAOE(float depth = 0, float yOffset = 0)
    {
        if (!isInfoSetted)
        {
            Debug.LogError("����ü ������ �������� �ʾҽ��ϴ�.");
            return;
        }
        if (indicatorAOEType != AOETypes.Box)
        {
            Debug.LogError("Indicator�� Box���� �ƴմϴ�.");
            return;
        }
        if (depth <= 0)
            depth = launchSpeed * disappearTime;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, depth, LayerMask.NameToLayer(TagsAndLayers.GroundLayer)))
        {
            depth = hit.distance + 0.1f;
        }
        indicator.IndicateBoxAOE(transform.position, direction, colliderSize.x, colliderSize.y + yOffset, depth, false);
    }
    public void IndicateCircleAOE(float radius = 0, float depth = 0)
    {
        if (!isInfoSetted)
        {
            Debug.LogError("����ü ������ �������� �ʾҽ��ϴ�.");
            return;
        }
        if (indicatorAOEType != AOETypes.Circle)
        {
            Debug.LogError("Indicator�� Box���� �ƴմϴ�.");
            return;
        }
        if (radius <= 0)
            radius = Mathf.Max(colliderSize.x, colliderSize.y, colliderSize.z);
        if (depth <= 0)
            depth = launchSpeed * disappearTime;
        indicator.IndicateCircleAOE(transform.position, direction, radius, depth, false);
    }
    /// <summary>
    /// ����ü�� Ư�� �������� �߻��� �� ���˴ϴ�. SetProjectileBeforeLaunch�� ���� �ʾҴٸ� ������ �߻��� �� �ֽ��ϴ�.
    /// </summary>
    public void Launch()
    {
        switch (launchType)
        {
            case ProjectileLaunchTypes.Shoot:
                ReleaseIndicator();
                break;
            case ProjectileLaunchTypes.Drop:
                break;
        }
        rigidbody.AddForce(direction * rigidbody.mass * launchSpeed, ForceMode.Impulse);
        StartCoroutine(WaitDisapperTime());
    }
    public void LerpIndicatorScale(float multScale, float lerpTime)
    {
        indicator.LerpIndicatorScale(multScale, lerpTime);
    }
    private IEnumerator WaitDisapperTime()
    {
        timeSinceLaunced = 0f;
        while (timeSinceLaunced <= disappearTime)
        {
            timeSinceLaunced += Time.deltaTime;
            yield return null;
        }
        TimeExpired?.Invoke(this);
    }
    public void OnGet()
    {
        gameObject.SetActive(true);
    }
    public void OnRelease()
    {
        if (indicator != null)
            ReleaseIndicator();
        StopAllCoroutines();
        gameObject.SetActive(false);
        rigidbody.velocity = Vector3.zero;
    }
    private void SetIndicator()
    {
        if (indicator != null && indicator.AOEType == indicatorAOEType)
            return;
        else if (indicator != null && indicator.AOEType != indicatorAOEType)
        {
            ReleaseIndicator();
            indicator =  AOEIndicatorPool.Instance.GetIndicatorPool(indicatorAOEType).Get();
        }
        else
            indicator =  AOEIndicatorPool.Instance.GetIndicatorPool(indicatorAOEType).Get();
    }
    public void ReleaseIndicator()
    {
        if (indicator == null)
            return;
        AOEIndicatorPool.Instance.GetIndicatorPool(indicatorAOEType).Release(indicator);
        indicator = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        ProjetileColliderEnter?.Invoke(other, this);
    }
    public void SetDisappearTime(float disappearTime)
    {
        this.disappearTime = Mathf.Clamp(disappearTime, 1f, 20f);
    }
}
