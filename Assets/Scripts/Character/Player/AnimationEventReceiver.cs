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
    // �ִϸ��̼� Ŭ������ �ش� �޼ҵ忡 �̺�Ʈ�� �������ָ� �������� �޼ҵ�鿡 �̺�Ʈ ������ �����մϴ�.
    private void OnAnimationEventCalled(AnimationEvent animEvent)
    {
        AnimEventCalled?.Invoke(animEvent);
    }
}
