using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "HideAndAttack", menuName = "Characters/Enemy/AttackAction/HideAndAttack")]
public class HideAndAttack : AttackAction
{
    [Tooltip("해당 시간이 지난 뒤 출현하며 공격합니다.")]
    [SerializeField][Range(3f, 6f)] private float hideTime = 3f;
    [Tooltip("숨고나서 해당 시간이 지난 뒤 공격할 지점을 결정합니다. HideTime보다 1초 이상 작도록 세팅됩니다.")]
    [SerializeField][Range(2f, 5f)] private float setDestinationTime = 2f;
    [SerializeField] private AOETypes aoeType;
    [Header("** 원형 범위일 경우 큰 값이 Raidus로 사용됨 **")]
    [SerializeField][Range(1f, 10f)] private float attackWidth;
    [SerializeField][Range(1f, 10f)] private float attackHeight;

    private Transform targetTransform;
    private NavMeshAgent agent;
    private ObstacleAvoidanceType initialObstacleAvoidanceType;
    private Collider collider;
    private Dictionary<int, Weapon> weapons;
    private Vector3 destination;
    private float hideStartTime;
    private bool isHide;
    private bool isDestinationSetted;
    private bool isAttackStarted;
    private AOEIndicator indicator;

    public override void OnAwake()
    {
        base.OnAwake();
        targetTransform = StateMachine.Player.transform;
        agent = StateMachine.Enemy.Agent;
        initialObstacleAvoidanceType = agent.obstacleAvoidanceType;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        collider = StateMachine.Enemy.Collider;
        Mathf.Clamp(setDestinationTime, 2, hideTime - 1);
        weapons = StateMachine.Enemy.Weapons;
    }
    public override void OnStart()
    {
        base.OnStart();
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled += AnimationEventDecision;
        SubscribeWeaponEvent();
        collider.enabled = false;
        isHide = false;
        isDestinationSetted = false;
        isAttackStarted = false;
        StartAnimation(Config.AnimTriggerHash1);
    }
    public override void OnEnd()
    {
        base.OnEnd();
        collider.enabled = true;
        agent.obstacleAvoidanceType = initialObstacleAvoidanceType;
        StateMachine.Enemy.AnimEventReceiver.AnimEventCalled -= AnimationEventDecision;
        UnsubscribeWeaponEvent();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed && !isHide)
        {
            hideStartTime = Time.time;
            isHide = true;
        }
        if (isHide && Time.time - hideStartTime >= setDestinationTime && !isDestinationSetted)
        {
            isDestinationSetted = true;
            Ray ray = new Ray(targetTransform.position + new Vector3(0, 0.2f, 0), Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, 1 << TagsAndLayers.GroundLayerIndex))
            {
                destination = hit.point;
                indicator = AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Get();
                Vector3 indicatorPosition = destination;
                float maxAttackRange = Mathf.Max(attackWidth, attackHeight);
                indicatorPosition.y += maxAttackRange;
                if (aoeType == AOETypes.Box)
                {
                    indicatorPosition.z += attackHeight / 2;
                    indicator.IndicateBoxAOE(indicatorPosition, Vector3.down, attackWidth, attackHeight, maxAttackRange * 2, false);
                }
                else
                {
                    indicator.IndicateCircleAOE(indicatorPosition, Vector3.down, maxAttackRange, maxAttackRange * 2, false);
                }
                agent.Move(destination - agent.transform.position);
            }
            else
            {
                Debug.LogError("적절한 Ground를 찾을 수 없습니다. 타겟 또는 지형을 확인하세요.");
            }
        }
        if (isDestinationSetted && Time.time - hideStartTime >= hideTime && !isAttackStarted)
        {
            isAttackStarted = true;
            ReleaseIndicator();
            StartAnimation(Config.AnimTriggerHash2);
        }
        if (animState[Config.AnimTriggerHash2] == AnimState.Completed)
        {
            isCompleted = true;
        }
    }
    private void AnimationEventDecision(AnimationEvent animEvent)
    {
        if (animEvent.stringParameter == "EnableCollider")
        {
            collider.enabled = true;
        }
    }
    private void SubscribeWeaponEvent()
    {
        foreach (Weapon weapon in weapons.Values)
        {
            weapon.WeaponColliderEnter += OnWeaponTriggerEnter;
        }
    }
    private void UnsubscribeWeaponEvent()
    {
        foreach (Weapon weapon in weapons.Values)
        {
            weapon.WeaponColliderEnter -= OnWeaponTriggerEnter;
        }
    }
    private void OnWeaponTriggerEnter(Collider other)
    {
        ApplyAttack(other.gameObject);
    }
    protected override void ReleaseIndicator()
    {
        if (indicator == null)
            return;
        AOEIndicatorPool.Instance.GetIndicatorPool(aoeType).Release(indicator);
        indicator = null;
    }
}
