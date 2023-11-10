using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Npc : MonoBehaviour
{
    public int id;
    public bool isNPC;
    public TimelineAsset[] Motion;
    private bool isHit;
    private GameObject target;

    private GameObject player;
    private PlayerStateMachine stateMachine;
    private void Update()
    {
        if(isHit && target != null)
        {
            Vector3 lookDirection = target.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2.0f * Time.deltaTime);
        }
    }
    public void ResetTarget()
    {
        stateMachine.ChangeState(stateMachine.IdleState);
        isHit = false;
        target = null;
    }
    private void Start()
    {
        player = GameManager.Instance.Player;
        stateMachine = player.GetComponent<Player>().StateMachine;
    }
    private void OnTriggerEnter(Collider other)
    {
        target = other.gameObject;
        isHit = true;
    }

    private void OnInteraction()
    {
        if(isHit)
        {
            Dialog.Instance.Action(gameObject);
            Dialog.Instance.panel.SetActive(true);
            stateMachine.ChangeState(stateMachine.NothingState);
        }
    }

}
