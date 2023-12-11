using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "ChargingAttack", menuName = "Characters/Enemy/AttackAction/ChargingAttack")]
public class ChargingAttack : AttackAction
{
    [SerializeField] private float chargingSpeed;
    [Tooltip("돌진을 시행할 횟수입니다.")]
    [SerializeField][Range(1, 3)] private int chargingCount;
    [SerializeField][Range(0.5f, 2f)] private float waitTimeBetweenCharging;
    private float waitedTime;
    private int alreadyChargedCount = 0;
    private bool isCharging;
    private bool isChargingEnd;
    private bool isIndicatorOn;
    private Vector3 targetPosition;
    private AOEIndicator indicator;
    private NavMeshAgent agent;

    public ChargingAttack()
    {
        ActionType = ActionTypes.CharingAttack;
    }
    public override void OnAwake()
    {
        base.OnAwake();
        agent = StateMachine.Enemy.Agent;
        if (agent == null)
            Debug.LogError("Enemy에 Agent컴포넌트가 없습니다.");
    }
    public override void OnStart()
    {
        base.OnStart();
        StateMachine.Enemy.OnCollisionOcurred += OnCollisionEnter;
    }
    public override void OnEnd()
    {
        base.OnEnd();
        StateMachine.Enemy.OnCollisionOcurred -= OnCollisionEnter;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (isCompleted)
            return;
        if (animState[Config.AnimTriggerHash1] == AnimState.NotStarted)
        {
            StartAnimation(Config.AnimTriggerHash1);
        }
        if (animState[Config.AnimTriggerHash1] == AnimState.Playing && currentAnimNormalizedTime > 0)
        {
            IndicatorOn();
        }
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed && !isChargingEnd)
        {
            Charge();
        }
        if (isChargingEnd)
        {
            LookTarget();
        }
    }
    private void IndicatorOn()
    {
        if (isIndicatorOn)
            return;
        Vector3 indicatorPosition = agent.transform.position;
        indicatorPosition.y += agent.baseOffset + agent.height - 0.1f;
        float maxSlopeHeight = Mathf.Sin(NavMesh.GetSettingsByID(agent.agentTypeID).agentSlope * Mathf.Deg2Rad) * Condition.LessThanThisDistance;
        indicatorPosition.y += maxSlopeHeight;
        indicator = AOEIndicatorPool.Instance.GetIndicatorPool(AOETypes.Box).Get();
        indicator.IndicateBoxAOE(indicatorPosition, agent.transform.forward, agent.radius * 2, agent.height + maxSlopeHeight * 2, Condition.LessThanThisDistance + agent.radius, false);
        isIndicatorOn = true;
    }
    private void Charge()
    {
        if (!isCharging)
        {
            Vector3 forward = agent.transform.forward;
            forward.y = 0;
            targetPosition = agent.transform.position + forward.normalized * Condition.LessThanThisDistance;
            Vector3 raycastPosition = targetPosition;
            raycastPosition.y += Condition.LessThanThisDistance;

            Ray ray = new Ray(raycastPosition, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Condition.LessThanThisDistance * 2, 1 << TagsAndLayers.GroundLayerIndex))
            {
                targetPosition = hit.point;
            }
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(targetPosition, out navHit, Condition.LessThanThisDistance, agent.areaMask))
            {
                targetPosition = navHit.position;
            }
            else
            {
                Debug.LogError("NavMesh영역을 찾을 수 없습니다.");
            }

            AOEIndicatorPool.Instance.GetIndicatorPool(AOETypes.Box).Release(indicator);
            isIndicatorOn = false;

            StartAnimation(Config.AnimTriggerHash2);
            StateMachine.Enemy.Rigidbody.isKinematic = true;
            isCharging = true;
        }

        Vector3 direction = targetPosition - agent.transform.position;
        direction.y = 0;
        if (chargingSpeed * Time.deltaTime < direction.magnitude)
        {
            direction = direction.normalized * chargingSpeed * Time.deltaTime;
        }
        agent.Move(direction);
        Vector3 currentPosition = agent.transform.position;
        currentPosition.y = 0;
        Vector3 targetPositionExceptY = targetPosition;
        targetPositionExceptY.y = 0;
        if (Vector3.Distance(currentPosition, targetPositionExceptY) < 0.1f)
        {
            alreadyChargedCount++;
            StopAnimation(Config.AnimTriggerHash2);
            isCharging = false;
            if (alreadyChargedCount >= chargingCount)
            {
                alreadyChargedCount = 0;
                isCompleted = true;
            }
            else
            {
                isChargingEnd = true;
            }
        }
    }
    private void LookTarget()
    {
        if (waitedTime < waitTimeBetweenCharging)
        {
            waitedTime += Time.deltaTime;
            return;
        }
        Vector3 targetDirection = StateMachine.Player.transform.position - agent.transform.position;
        targetDirection.y = 0;
        Vector3 forward = agent.transform.forward;
        forward.y = 0;
        float angle = Vector3.SignedAngle(forward, targetDirection, Vector3.up);
        angle = Mathf.Min(angle, agent.angularSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.up);
        agent.transform.rotation = agent.transform.rotation * targetRotation;


        targetDirection = StateMachine.Player.transform.position - agent.transform.position;
        targetDirection.y = 0;
        forward = agent.transform.forward;
        forward.y = 0;
        if (Vector3.Angle(targetDirection, forward) < 0.1f)
        {
            animState[Config.AnimTriggerHash1] = AnimState.NotStarted;
            waitedTime = 0f;
            isChargingEnd = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        isChargingEnd = true;
        StopAnimation(Config.AnimTriggerHash2);
        ApplyAttack(collision.gameObject, isPossibleMultiEffect: true);
        ReleaseIndicator();
    }

    protected override void ReleaseIndicator()
    {
        if (indicator == null)
            return;
        AOEIndicatorPool.Instance.GetIndicatorPool(AOETypes.Box).Release(indicator);
        indicator = null;
    }
}
