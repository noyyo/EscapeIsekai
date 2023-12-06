using System;
using UnityEngine;

[Serializable]
public class EnemyAnimationData
{
    [Header("Peace")]
    [SerializeField][ReadOnly] private string peaceParameterName = "@Peace";
    [SerializeField][ReadOnly] private string idleParameterName = "Idle";
    [SerializeField][ReadOnly] private string walkParameterName = "Walk";
    [SerializeField][ReadOnly] private string returnToBaseParameterName = "ReturnToBase";

    [Header("Battle")]
    [SerializeField][ReadOnly] private string battleParameterName = "@Battle";
    [SerializeField][ReadOnly] private string runParameterName = "Run";
    [SerializeField][ReadOnly] private string DeadParameterName = "Dead";
    [SerializeField][ReadOnly] private string StunParameterName = "Stun";

    public int PeaceParameterHash { get; private set; }
    public int IdleParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }
    public int ReturnToBaseParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }
    public int BattleParameterHash { get; private set; }
    public int DeadParameterHash { get; private set; }
    public int StunParameterHash { get; private set; }
    public void Initialize()
    {
        PeaceParameterHash = Animator.StringToHash(peaceParameterName);
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        WalkParameterHash = Animator.StringToHash(walkParameterName);
        ReturnToBaseParameterHash = Animator.StringToHash(returnToBaseParameterName);
        RunParameterHash = Animator.StringToHash(runParameterName);
        DeadParameterHash = Animator.StringToHash(DeadParameterName);
        StunParameterHash = Animator.StringToHash(StunParameterName);
        BattleParameterHash = Animator.StringToHash(battleParameterName);
    }
}
