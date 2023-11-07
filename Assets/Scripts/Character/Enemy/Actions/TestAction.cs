using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestActionSO", menuName = "Characters/Enemy/AttackAction/TestAction")]
public class TestAction : AttackAction
{
    /*
     * �׼��� �⺻���� �帧�� ������ �����ϴ�.
     * Awake�� ����Ǿ� �⺻���� �ʱ�ȭ�� �����մϴ�.
     * ĳ���Ͱ� �׼��� �����ϸ� Start�� ����ǰ� ���� Update���� �� ������ ȣ��˴ϴ�. �׼��� ������ �� ĳ���ʹ� ����� �ٶ󺸰� �ִ� �����Դϴ�.
     * �׼��� ������ Update�� Ȱ���ؼ� �ۼ��ؾ� �մϴ�. ������ Ȱ���ؾ��ϴ� ������ �������� �ٸ� �е��� �ν����ͷ� ������ �� �ְԲ� SerializeField �Ӽ��� ����ϴ� ���� �����մϴ�. ���������θ� ���Ǵ� ������ HideInInspector�Ӽ��� ����ؼ� ����ϴ�.
     * Update���� ������ ������ ������ bool���� ������ ���� ���θ� �Ǵ��� �� �����ϰų� Event�� ���� �˸��� �� �����Ӱ� �����ϸ� �˴ϴ�.
     * Update���� timeStarted �Ǵ� timeRunning���� �ʵ� ���� ���ؼ� �ð��� �������� ������ ������ �� �ֽ��ϴ�.
     * �׼��� ������ �Ϸ�Ǿ��ٸ� isCompleted ������ true�� ��������� �մϴ�. �׷��� ���� �����ӿ��� �׼��� ������ ĳ���ʹ� �ٸ� ���·� ��ȯ�˴ϴ�.
     * ����Ʈ�� ������ ���� OnEffectStart() �޼ҵ带 ��������� �մϴ�. �������� ������ ����Ʈ�� ���� ������� �ʽ��ϴ�.
     * �ִϸ��̼��� �����ϱ� ���ؼ� StartAnimation �޼ҵ带 ����ϰ� animState ��ųʸ��� �ش� �ִϸ��̼��� ������� �ʾҴ��� ���������� �������� Ȯ���� �� �ֽ��ϴ�.
     * ���� �������� �ִϸ��̼��� currentAnimHash �� currentAnimNormalizedTime �ʵ�� �ؽ����� �������̶�� ��ŭ ����ƴ����� Ȯ���� �� �ֽ��ϴ�.
     * ���� �ִϸ��̼��� Trigger�� �ƴ� Bool���� �Ķ���ͷ� �����ȴٸ� ������ ������ StopAnimation �޼ҵ带 ȣ�����־�� �մϴ�.
     * �̿ܿ��� baseŬ�������� ���Ǵ� �޼ҵ� �� �Ű������� ���콺�� �÷������� ������ �� �� �ֽ��ϴ�. 
     */

    // �׼ǿ��� �ʿ��� �ʱ�ȭ �۾��� �����ϸ� �˴ϴ�.
    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log($"Awake ���� �ð� : {Time.time}");
    }
    // �������� �ٽ� �������ְų� �ʿ��� �۾��� ���ָ� �˴ϴ�.
    public override void OnStart()
    {
        base.OnStart();
        // �ִϸ��̼��� �����մϴ�.
        StartAnimation(Config.AnimTriggerHash1);
        Debug.Log($"Start ���� �ð� : {Time.time}");
    }
    // �׼��� ���� �� �ʿ��� �۾��� ���ָ� �˴ϴ�.
    public override void OnEnd()
    {
        base.OnEnd();
        Debug.Log($"End ���� �ð� : {Time.time}");
    }

    // �׼��� ������ �����ϸ� �˴ϴ�.
    public override void OnUpdate()
    {
        // Anim1�� �Ϸ�Ǿ���, ����Ʈ�� ���� �������� �ʾҴٸ� ����Ʈ�� �����մϴ�.
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed && !isEffectStarted)
        {
            OnEffectStart();
        }
        // Anim2�� �Ϸ�Ǿ��ٸ� �׼��� �Ϸ��մϴ�.
        if (animState[Config.AnimTriggerHash2] == AnimState.Completed)
        {
            isCompleted = true;
        }
        base.OnUpdate();
    }
    // ����Ʈ�� ������ �� ȣ���ؾ��մϴ�.
    protected override void OnEffectStart()
    {
        base.OnEffectStart();
        // �ִϸ��̼��� �����մϴ�.
        StartAnimation(Config.AnimTriggerHash2);
        Debug.Log($"EffectStart �ð�(�׼Ǳ���) : {EffectStartTime}");
    }
    // ����Ʈ�� ���� �� �ڵ����� ȣ��˴ϴ�. �ʿ��� �۾��� �����մϴ�.
    protected override void OnEffectFinish()
    {
        base.OnEffectFinish();
        Debug.Log($"EffectFinish �ð�(�׼Ǳ���) : {timeRunning}");
    }
}
