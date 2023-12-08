using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Npc : MonoBehaviour
{
    public int id;
    public bool isNPC;
    public TimelineAsset[] Motion;
    public bool isHit;
    public Sprite pofile;
    private GameObject target;
    private GameObject player;
    private PlayerStateMachine stateMachine;
    PlayableDirector playableDirector;
    public GameObject[] marks;
    private UI_Manager uI_Manager;
    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        uI_Manager = UI_Manager.Instance;
    }
    private void Update()
    {
        if (isHit && target != null && isNPC)
        {
            Vector3 lookDirection = target.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2.0f * Time.deltaTime);
        }
    }
    public void ResetTarget()
    {
        stateMachine.ChangeState(stateMachine.IdleState);
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
        if (other.tag == "Player")
        {
            uI_Manager.itemName = gameObject.name;
            uI_Manager.itemExplanation = "대화하기";
            uI_Manager.UI_gathering.Setting();
            UI_Manager.Instance.gathering.SetActive(true);
            isHit = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //   stateMachine.ChangeState(stateMachine.IdleState);
        if (other.tag == "Player")
        {
            isHit = false;
            UI_Manager.Instance.gathering.SetActive(false);
        }
        target = null;
    }


    private void OnInteraction()
    {
        if (isHit)
        {
            int temp = Random.Range(0,3);
            string tempstr = null;
            if (temp == 0)
                tempstr = "npc";
            if (temp == 1)
                tempstr = "npc1";
            if (temp == 2)
                tempstr = "npc2";
            if(id!=800)
            {
                if(id==200|| id == 300|| id == 400||id ==1100)
                {
                    SoundManager.Instance.CallPlaySFXReturnSource(ClipType.NPCSFX, tempstr, this.transform, false, 1f, soundValue: 0.2f);
                }
                else
                {
                    SoundManager.Instance.CallPlaySFXReturnSource(ClipType.NPCSFX, tempstr, this.transform, false, 0.4f, soundValue:0.2f);
                }
            }
            
            Dialog.Instance.Action(gameObject);
            Dialog.Instance.panel.SetActive(true);
            UI_Manager.Instance.gathering.SetActive(false);
            //  stateMachine.ChangeState(stateMachine.NothingState);
        }
    }


    private void OnClearMark(int Npcid)
    {
        if (Npcid == id)
        {
            marks[0].SetActive(false);
            marks[1].SetActive(false);
            marks[2].SetActive(true);
        }
    }

    private void OnAcceptMark()
    {
        if (ServeQuestManager.Instance.MarkInit(id))
        {
            marks[0].SetActive(true);
            marks[1].SetActive(false);
            marks[2].SetActive(false);
        }
    }

    private void ShutDownMark(int npcid)
    {
        if (npcid == id)
        {
            marks[0].SetActive(false);
            marks[1].SetActive(false);
            marks[2].SetActive(false);
        }
    }
    private void CanAcceptMark(int Npcid)
    {
        if (Npcid == id)
        {
            marks[0].SetActive(true);
            marks[1].SetActive(false);
            marks[2].SetActive(false);
        }
    }

    public void CheckeState(int state)
    {
        for (int i = 0; i < 3; i++)
        {
            marks[i].SetActive(false);
            if (state == i && i != 2)
            {
                marks[i].SetActive(true);
            }
        }
    }
}
