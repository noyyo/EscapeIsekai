using UnityEngine;


public enum BuffTypes
{
    speed,
    nostamina,
    shield
}

public class Buff : MonoBehaviour
{
    public float duration;
    public float buffStartTime;
    private PlayerStateMachine playerStateMachine;
    private Playerconditions playerconditions;
    public BuffTypes buffType;


    public Buff(BuffTypes type, PlayerStateMachine stateMachine)   // 생성자
    {
        buffType = type;
        playerStateMachine = stateMachine;
        playerconditions = playerStateMachine.Player.Playerconditions;
    }


    public void ApplyBuff(float powerUpDuration)
    {
        duration = powerUpDuration;
        buffStartTime = Time.time;

        switch (buffType)
        {
            case BuffTypes.speed:
                playerStateMachine.MovementSpeedModifier = 2.0f;
                break;
            case BuffTypes.nostamina:
                Debug.Log("스태미나 버프!");
                playerconditions.ActiveNoStaminaBuff();
                break;
            case BuffTypes.shield:
                Debug.Log("보호막 활성화");
                playerStateMachine.ActivateShield();
                break;
            default:
                break;
        }
    }

    public void EndBuff()
    {
        switch (buffType)
        {
            case BuffTypes.speed:
                playerStateMachine.MovementSpeedModifier = 1.0f;
                break;
            case BuffTypes.nostamina:
                Debug.Log("스태미너 버프 빠짐");
                playerconditions.DeActivateNoStaminaBuff();
                break;
            case BuffTypes.shield:
                Debug.Log("보호막 사라짐");
                playerStateMachine.DeActivateShield();
                break;
            default:
                break;
        }
    }
}
