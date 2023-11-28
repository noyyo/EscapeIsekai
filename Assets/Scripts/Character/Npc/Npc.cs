using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public bool isHit;
    private GameObject target;
    private GameObject player;
    private PlayerStateMachine stateMachine;
    PlayableDirector playableDirector;
    public GameObject[] marks;

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }
    private void Update()
    {
        if(isHit && target != null&& isNPC)
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
        ServeQuestManager.Instance.CanClear += OnClearMark;
        ServeQuestManager.Instance.CanAccept += CanAcceptMark;
        ServeQuestManager.Instance.isAllClear += ShutDownMark;
        OnAcceptMark();
        MotionInit();
    }

    void MotionInit()
    {
        for (int i = 0; i < Motion.Length; i++)
        {
            for (int j = 0; j < Motion[i].outputTrackCount; j++)
            {
                ActivationTrack actiTrack = Motion[i].GetOutputTrack(j) as ActivationTrack;
                AnimationTrack animaTrack = Motion[i].GetOutputTrack(j) as AnimationTrack;
                GameObject targetObject = GameManager.Instance.dialogCamera.gameObject;
                playableDirector.SetGenericBinding(actiTrack, targetObject);
                playableDirector.SetGenericBinding(animaTrack, targetObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        target = other.gameObject;
        if(other.tag == "Player")
        {
            UI_Manager.Instance.itemName = "";
            UI_Manager.Instance.itemExplanation = "대화하기";
            UI_Manager.Instance.gathering.SetActive(true);
            isHit = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        stateMachine.ChangeState(stateMachine.IdleState);
        if (other.tag == "Player")
        {
            isHit = false;
                        UI_Manager.Instance.gathering.SetActive(false);
        }
        target = null;
    }


    private void OnInteraction()
    {
        if(isHit)
        {
            Dialog.Instance.Action(gameObject);
            Dialog.Instance.panel.SetActive(true);
            UI_Manager.Instance.gathering.SetActive(false);
            stateMachine.ChangeState(stateMachine.NothingState);
        }
    }


    private void OnClearMark(int Npcid)
    {
        if(Npcid == id)
        {
            marks[0].SetActive(false);
            marks[1].SetActive(false);
            marks[2].SetActive(true);
        }
    }

    private void OnAcceptMark()
    {
        if(ServeQuestManager.Instance.MarkInit(id))
        {
            marks[0].SetActive(true);
            marks[1].SetActive(false);
            marks[2].SetActive(false);
        }
    }

    private void ShutDownMark(int npcid)
    {
        if(npcid == id)
        {
            marks[0].SetActive(false);
            marks[1].SetActive(false);
            marks[2].SetActive(false);
        }
    }
    private void CanAcceptMark(int Npcid)
    {
        if(Npcid== id)
        {
            marks[0].SetActive(true);
            marks[1].SetActive(false);
            marks[2].SetActive(false);
        }
    }

    public void CheckeState(int state)
    {
        for(int i = 0; i < 3; i++)
        {
            marks[i].SetActive(false);
            if(state == i && i !=2)
            {
                marks[i].SetActive(true);
            }
        }
    }
}
