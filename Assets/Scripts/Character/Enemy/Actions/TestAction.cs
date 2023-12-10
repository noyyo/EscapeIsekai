using UnityEngine;

[CreateAssetMenu(fileName = "TestActionSO", menuName = "Characters/Enemy/AttackAction/TestAction")]
public class TestAction : AttackAction
{
    /*
     * 액션의 기본적인 흐름은 다음과 같습니다.
     * Awake가 실행되어 기본적인 초기화를 실행합니다.
     * 캐릭터가 액션을 실행하면 Start가 실행되고 이후 Update문이 매 프레임 호출됩니다. 액션을 시작할 때 캐릭터는 대상을 바라보고 있는 상태입니다.
     * 액션의 로직은 Update를 활용해서 작성해야 합니다. 로직에 활용해야하는 참조나 변수등은 다른 분들이 인스펙터로 참조할 수 있게끔 SerializeField 속성을 사용하는 것을 권장합니다. 내부적으로만 사용되는 변수는 HideInInspector속성을 사용해서 숨깁니다.
     * Update에서 로직을 적용할 때에는 bool값을 가지고 적용 여부를 판단한 뒤 적용하거나 Event를 만들어서 알리는 등 자유롭게 구현하면 됩니다.
     * Update에서 timeStarted 또는 timeRunning등의 필드 값을 통해서 시간을 기준으로 로직을 적용할 수 있습니다.
     * 액션의 실행이 완료되었다면 isCompleted 변수를 true로 설정해줘야 합니다. 그러면 다음 프레임에서 액션이 끝나고 캐릭터는 다른 상태로 전환됩니다.
     * 이펙트를 시작할 때는 OnEffectStart() 메소드를 실행해줘야 합니다. 실행하지 않으면 이펙트가 영영 사라지지 않습니다.
     * 애니메이션을 시작하기 위해선 StartAnimation 메소드를 사용하고 animState 딕셔너리로 해당 애니메이션이 실행되지 않았는지 실행중인지 끝났는지 확인할 수 있습니다.
     * 현재 실행중인 애니메이션은 currentAnimHash 및 currentAnimNormalizedTime 필드로 해쉬값과 실행중이라면 얼만큼 실행됐는지를 확인할 수 있습니다.
     * 만약 애니메이션이 Trigger가 아닌 Bool값의 파라미터로 관리된다면 적절한 시점에 StopAnimation 메소드를 호출해주어야 합니다.
     * 이외에도 base클래스에서 사용되는 메소드 및 매개변수에 마우스를 올려놓으면 설명을 볼 수 있습니다. 
     */

    // 액션에서 필요한 초기화 작업을 진행하면 됩니다.
    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log($"Awake 시작 시간 : {Time.time}");
    }
    // 변수들을 다시 세팅해주거나 필요한 작업을 해주면 됩니다.
    public override void OnStart()
    {
        base.OnStart();
        // 애니메이션을 시작합니다.
        StartAnimation(Config.AnimTriggerHash1);
        Debug.Log($"Start 시작 시간 : {Time.time}");
    }
    // 액션이 끝날 때 필요한 작업을 해주면 됩니다.
    public override void OnEnd()
    {
        base.OnEnd();
        Debug.Log($"End 시작 시간 : {Time.time}");
    }

    // 액션의 로직을 실행하면 됩니다.
    public override void OnUpdate()
    {
        base.OnUpdate();
        // Anim1이 완료되었고, 이펙트를 아직 시작하지 않았다면 이펙트를 실행합니다.
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed && !isEffectStarted)
        {
            OnEffectStart();
        }
        // Anim2가 완료되었다면 액션을 완료합니다.
        if (animState[Config.AnimTriggerHash2] == AnimState.Completed)
        {
            isCompleted = true;
        }
    }
    // 이펙트를 시작할 때 호출해야합니다.
    protected override void OnEffectStart()
    {
        base.OnEffectStart();
        // 애니메이션을 시작합니다.
        StartAnimation(Config.AnimTriggerHash2);
        Debug.Log($"EffectStart 시간 : {EffectStartTime}");
    }
    // 이펙트가 끝날 때 자동으로 호출됩니다. 필요한 작업을 수행합니다.
    protected override void OnEffectFinish()
    {
        base.OnEffectFinish();
        Debug.Log($"EffectFinish 시간(액션기준) : {Time.time}");
    }

    protected override void ReleaseIndicator()
    {
        throw new System.NotImplementedException();
    }
}
