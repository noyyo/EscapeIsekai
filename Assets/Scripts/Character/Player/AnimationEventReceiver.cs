using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    Animator animator;
    public event Action<AnimationEvent> AnimEventCalled;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // 애니메이션 클립에서 해당 메소드에 이벤트를 연결해주면 구독중인 메소드들에 이벤트 정보를 전달합니다.
    private void OnAnimationEventCalled(AnimationEvent animEvent)
    {
        AnimEventCalled?.Invoke(animEvent);
    }
}
