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
    // 발사한 Enemy객체.
    public Enemy Launcher { get; private set; }
    [Tooltip("해당 시간이 지날 때까지 부딪히지 않는다면 자동으로 소멸됩니다.")]
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
    private float indicatorLerpTime;
    private AOEIndicator indicator;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        colliderSize = collider.bounds.extents * 2;
    }
    /// <summary>
    /// 투사체가 발사될 정보를 설정합니다.
    /// </summary>
    /// <param name="launcher">발사하는 Enemy객체입니다.</param>
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
            Debug.LogError("투사체 정보가 설정되지 않았습니다.");
            return;
        }
        if (indicatorAOEType != AOETypes.Box)
        {
            Debug.LogError("Indicator가 Box형이 아닙니다.");
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
            Debug.LogError("투사체 정보가 설정되지 않았습니다.");
            return;
        }
        if (indicatorAOEType != AOETypes.Circle)
        {
            Debug.LogError("Indicator가 Box형이 아닙니다.");
            return;
        }
        if (radius <= 0)
            radius = Mathf.Max(colliderSize.x, colliderSize.y, colliderSize.z);
        if (depth <= 0)
            depth = launchSpeed * disappearTime;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, depth, LayerMask.NameToLayer(TagsAndLayers.GroundLayer)))
        {
            depth = hit.distance + 0.1f;
        }
        indicator.IndicateCircleAOE(transform.position, direction, radius, depth);
    }
    /// <summary>
    /// 투사체를 특정 방향으로 발사할 때 사용됩니다. SetProjectileBeforeLaunch이 되지 않았다면 오류가 발생할 수 있습니다.
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
        StartCoroutine(LerpScale(multScale, lerpTime));
    }
    private IEnumerator LerpScale(float multScale, float lerpTime)
    {
        indicatorLerpTime = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * multScale;
        while (indicatorLerpTime <= lerpTime)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, lerpTime);
            indicatorLerpTime += Time.deltaTime;
            yield return null;
        }
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
    }
    private void OnTriggerEnter(Collider other)
    {
        ProjetileColliderEnter?.Invoke(other, this);
    }
}
