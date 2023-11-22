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
    // �߻��� Enemy��ü.
    public Enemy Launcher { get; private set; }
    [Tooltip("�ش� �ð��� ���� ������ �ε����� �ʴ´ٸ� �ڵ����� �Ҹ�˴ϴ�.")]
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
    /// ����ü�� �߻�� ������ �����մϴ�.
    /// </summary>
    /// <param name="position">�߻� ������ ���� ��ǥ�Դϴ�.</param>
    /// <param name="direction">�߻� ���������� ���� �����Դϴ�.</param>
    /// <param name="launchSpeed">�߻��� ���� �ӵ��Դϴ�.</param>
    /// <param name="launcher">�߻��ϴ� Enemy��ü�Դϴ�.</param>
    /// <param name="aoeYOffset">AOE������ ǥ���� �� ����ü�� �Ʒ��������� �߰����� AOE������ ǥ�õ� ��ġ�Դϴ�. AOE������ ������ �ʾ������� �ǰݵǴ� ���� �����ϱ� ���� ���Դϴ�.</param>
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
