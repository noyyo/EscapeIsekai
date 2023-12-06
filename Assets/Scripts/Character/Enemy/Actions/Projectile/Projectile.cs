using System;
using System.Collections;
using UnityEngine;

public enum ProjectileLaunchTypes
{
    Shoot,
    Drop,
}
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public event Action<Collider, Projectile> ProjetileColliderEnter;
    public event Action<Projectile> TimeExpired;
    // 발사한 Enemy객체.
    public Enemy Launcher { get; private set; }
    [Tooltip("해당 시간이 지날 때까지 부딪히지 않는다면 자동으로 소멸됩니다.")]
    [SerializeField][Range(1f, 20f)] private float disappearTime = 5f;
    [Tooltip("꼬리 이펙트를 남길 파티클을 지정합니다. 없어도 무관합니다.")]
    [SerializeField] private ParticleSystem trailParticlePrefab;
    private ParticleSystem trailParticle;
    private float burstCount;
    private ProjectileLaunchTypes launchType;
    private AOETypes indicatorAOEType;
    private bool isInfoSetted;
    private float timeSinceLaunced;
    private Rigidbody rigidbody;
    private Collider collider;
    private Vector3 colliderSize;
    private Vector3 direction;
    private float launchSpeed;
    private AOEIndicator indicator;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        colliderSize = collider.bounds.extents * 2;
        if (trailParticlePrefab != null)
        {
            trailParticle = Instantiate(trailParticlePrefab, transform);
            ParticleSystem.EmissionModule emissionModule = trailParticle.emission;
            burstCount = emissionModule.burstCount;
        }
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
        if (Physics.Raycast(ray, out hit, depth, 1 << LayerMask.NameToLayer(TagsAndLayers.GroundLayer)))
        {
            depth = hit.distance + 0.1f;
        }
        indicator.IndicateBoxAOE(transform.position, direction, colliderSize.x * transform.localScale.x, colliderSize.y * transform.localScale.y + yOffset, depth, false);
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
            radius = Mathf.Max(colliderSize.x * transform.localScale.x, colliderSize.y * transform.localScale.y, colliderSize.z * transform.localScale.z);
        if (depth <= 0)
            depth = launchSpeed * disappearTime;
        indicator.IndicateCircleAOE(transform.position, direction, radius, depth, false);
    }
    /// <summary>
    /// 투사체를 특정 방향으로 발사할 때 사용됩니다. SetProjectileBeforeLaunch이 되지 않았다면 오류가 발생할 수 있습니다.
    /// </summary>
    public void Launch()
    {
        gameObject.SetActive(true);
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
        if (trailParticle != null)
        {
            ParticleSystem.MainModule mainModule = trailParticle.main;
            ParticleSystem.VelocityOverLifetimeModule velocity = trailParticle.velocityOverLifetime;
            velocity.z = launchSpeed;
            trailParticle.Play();
        }
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
    }
    public void OnRelease()
    {
        if (indicator != null)
            ReleaseIndicator();
        if (trailParticle != null)
            trailParticle.Stop();
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
            indicator = AOEIndicatorPool.Instance.GetIndicatorPool(indicatorAOEType).Get();
        }
        else
            indicator = AOEIndicatorPool.Instance.GetIndicatorPool(indicatorAOEType).Get();
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
