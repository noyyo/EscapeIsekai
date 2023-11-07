using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum BuffTypes
{
    speed,
    strength
}

public class Buff : MonoBehaviour
{
    public float Duration;
    public float buffStartTime;
    private PlayerStateMachine playerStateMachine;
    public BuffTypes BuffType;
   
    
    public Buff(BuffTypes type, PlayerStateMachine stateMachine)   // 생성자
    {
        BuffType = type;
        playerStateMachine = stateMachine;
    }


    public void ApplyBuff(float powerUpDuration)
    {
        Duration = powerUpDuration;
        buffStartTime = Time.time;

        switch (BuffType)
        {
            case BuffTypes.speed:
                playerStateMachine.MovementSpeedModifier = 2.0f;
                break;
            case BuffTypes.strength:
                playerStateMachine.AttackPowerModifier = 2.0f;
                break;
            default:
                break;
        }
    }

    public void EndBuff()
    {
        Debug.Log("엔드");
        switch (BuffType)
        {
            case BuffTypes.speed:
                playerStateMachine.MovementSpeedModifier = 1.0f;
                break;
            case BuffTypes.strength:
                playerStateMachine.AttackPowerModifier = 1.0f;
                break;
            default:
                break;
        }
    }
}
