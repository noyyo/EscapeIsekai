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


    public Buff(BuffTypes type, PlayerStateMachine stateMachine)   // ������
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
                Debug.Log("���¹̳� ����!");
                playerconditions.ActiveNoStaminaBuff();
                break;
            case BuffTypes.shield:
                Debug.Log("��ȣ�� Ȱ��ȭ");
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
                Debug.Log("���¹̳� ���� ����");
                playerconditions.DeActivateNoStaminaBuff();
                break;
            case BuffTypes.shield:
                Debug.Log("��ȣ�� �����");
                playerStateMachine.DeActivateShield();
                break;
            default:
                break;
        }
    }
}
