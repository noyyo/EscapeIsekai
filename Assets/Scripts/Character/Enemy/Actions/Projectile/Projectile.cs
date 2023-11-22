using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour 
{
    public event Action<GameObject, Projectile> ProjetileColliderEnter;
    public event Action<Projectile> TimeExpired;
    // 발사한 Enemy객체.
    public Enemy Launcher { get; private set; }
    [Tooltip("해당 시간이 지날 때까지 부딪히지 않는다면 자동으로 소멸됩니다.")]
    [SerializeField][Range(1f, 20f)] private float disappearTime = 5f;
    private float timeSinceLaunced;
    public new Rigidbody rigidbody;
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
    /// 투사체가 발사될 정보를 설정합니다.
    /// </summary>
    /// <param name="position">발사 지점의 월드 좌표입니다.</param>
    /// <param name="direction">발사 지점에서의 방향 벡터입니다.</param>
    /// <param name="launchSpeed">발사할 때의 속도입니다.</param>
    /// <param name="launcher">발사하는 Enemy객체입니다.</param>
    /// <param name="aoeYOffset">AOE범위를 표시할 때 투사체의 아래방향으로 추가적인 AOE범위가 표시될 수치입니다. AOE범위가 보이지 않았음에도 피격되는 것을 방지하기 위한 값입니다.</param>
    public void SetProjectile(Vector3 position, Vector3 direction, float launchSpeed, Enemy launcher, float aoeYOffset = 0f)
    {
        transform.position = position;
        transform.forward = direction;
        this.direction = direction;
        this.launchSpeed = launchSpeed;
        Launcher = launcher;
        float depth = launchSpeed * disappearTime;
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, launchSpeed * disappearTime, LayerMask.NameToLayer(TagsAndLayers.GroundLayer)))
        {
            depth = hit.distance;
        }

        indicator = AOEIndicatorPool.Instance.GetIndicatorPool(AOETypes.Box).Get();
        Vector3 indicatorPosition = transform.TransformPoint(new Vector3(0, -aoeYOffset / 2, 0));
        indicator.IndicateBoxAOE(indicatorPosition, direction, colliderSize.x, colliderSize.y + aoeYOffset, depth, false);
    }
    public void Launch()
    {
        if (indicator != null)
        {
            AOEIndicatorPool.Instance.GetIndicatorPool(AOETypes.Box).Release(indicator);
        }
        rigidbody.AddForce(direction * rigidbody.mass * launchSpeed, ForceMode.Impulse);
        timeSinceLaunced = 0f;
        StartCoroutine(WaitDisapperTime());
    }
    private void OnTriggerEnter(Collider other)
    {
        ProjetileColliderEnter?.Invoke(other.gameObject, this);
    }
    private IEnumerator WaitDisapperTime()
    {
        while (timeSinceLaunced <= disappearTime)
        {
            timeSinceLaunced += Time.deltaTime;
            yield return null;
        }
        TimeExpired?.Invoke(this);
        StopAllCoroutines();
    }
}
