using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Dialog : MonoBehaviour
{
    public QuestManager questManager;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI talkText;
    public RawImage texture;
    public GameObject panel;
    public Button nextB;
    private int talkIndex;
    private int serveQuestTalkIndex;
    private int tmp;
    private bool isAction;
    private Animator animator;
    private GameObject targetNpc;
    public static Dialog Instance;


    private GameObject player;
    private void Awake()
    {
        Instance = this;
        player = GameManager.Instance.Player;
        panel.SetActive(false);
    }

    public void Action(GameObject scanObj) //대화시작
    {
        player.GetComponent<PlayerInputSystem>().PlayerActions.Disable();
        Cursor.lockState = CursorLockMode.Confined;
        if (isAction)
        {
            isAction = false;
        }
        else  
        {
            isAction = true;
            targetNpc = scanObj;
            if (targetNpc.GetComponent<NavMeshAgent>() != null)
            {
                targetNpc.GetComponent<NavMeshAgent>().speed = 0 ;
            }
           
            Npc npcData = targetNpc.GetComponent<Npc>();
            if(targetNpc != null)
            {
                TalkMotion();
            }
          
            Talk(npcData.id, npcData.isNPC);
        }
          panel.SetActive(isAction); 
    }

    public void BtnAction()
    {
        if(targetNpc != null)
        {
             Npc npcData = targetNpc.GetComponent<Npc>();
            Talk(npcData.id, npcData.isNPC);

        }
        else
        {
            isAction = false;
            talkIndex = 0;
            serveQuestTalkIndex = 0;
            panel.SetActive(isAction);
        }
         
            
    }
    public void Talk(int id, bool isNPC)
    {
        int questTalkIndex = questManager.GetQuestTalkIndex(id);
        string talkData = TalkManager.Instance.GetTalk(id + questTalkIndex, talkIndex);
        int key = ServeQuestManager.Instance.GetQuest(id);
        if (tmp != 0&&ServeQuestManager.Instance.playerQuest[key]!=2 && ServeQuestManager.Instance.playerQuest[tmp] ==2)
        {
            key = tmp;
        }
        if(key != tmp && tmp !=0&& ServeQuestManager.Instance.GetTalk(tmp, serveQuestTalkIndex)!= ServeQuestManager.Instance.GetTalk(key, serveQuestTalkIndex))
        {
            key = tmp;
        }
        if ((ServeQuestManager.Instance.playerQuest.ContainsKey(key)&& ServeQuestManager.Instance.playerQuest[key] == 2 &&  serveQuestTalkIndex <= talkIndex) || key ==0)
        {
            if (talkData == null)

            {
                if (id == 1)
                {
                    QuestManager.Instance.questId = 10;
                }
                if (id == 100) //대장장이
                {
                    ItemCraftingManager.Instance.CallOnCrafting();
                    Cursor.lockState = CursorLockMode.None;
                }
                if (id == 200) //요리
                {

                }
                if (id == 300) //잡화
                {
                }
                if (id == 400) //여관
                {
                    GameManager.Instance.timeSlip.GetComponent<TimeSlip>().NpcAction();
                }
                if (id == 500) //검술
                {

                }
                if (id == 1000) //상자
                {
                    if(targetNpc.GetComponentInChildren<Animator>() != null)
                    {
                        animator = targetNpc.GetComponentInChildren<Animator>();
                    }
                    MinigameManager.Instance.ChangeSuccess += ChestOpen;
                    StartCoroutine(MinigameManager.Instance.StartMissionCoroutine(1));
                }
                ExitTalk();
                return;
            }

            if (isNPC)
            {
                talkText.text = talkData;
                nameText.text = targetNpc.name;

            }
            else
            {
                talkText.text = talkData;
                if (targetNpc != null)
                    nameText.text = targetNpc.name;
            }

            isAction = true;
            talkIndex++;
        }
        else if (ServeQuestManager.Instance.questDBDic.ContainsKey(key)&& ServeQuestManager.Instance.playerQuest[key] <=2)
        {
            tmp = key;
            ServeQuestManager.Instance.QuestItemCheck(key);
            ServeQuestManager.Instance.QuestClearCheck(key);
            if (ServeQuestManager.Instance.GetTalk(key, serveQuestTalkIndex) == null)
            {
                ServeQuestManager.Instance.MakeQuestZone(key);
                ServeQuestManager.Instance.AddQuestList(key);
                if (ServeQuestManager.Instance.playerQuest[key] == 0)
                {
                    ServeQuestManager.Instance.playerQuest[key] = 1;
                }
                Npc npc = targetNpc.GetComponent<Npc>();
                ServeQuestManager.Instance.ChangeMark(key, npc);

                ExitTalk();
                return;
            }
            nameText.text = targetNpc.name;
            talkText.text = ServeQuestManager.Instance.GetTalk(key, serveQuestTalkIndex);
            isAction = true;
            serveQuestTalkIndex++;
        }
}
        public void ExitTalk()
    {
        StopTalkMotion();
        if (targetNpc.GetComponent<NavMeshAgent>() != null)
        {
            targetNpc.GetComponent<NavMeshAgent>().speed = 3.5f;
        }
        isAction = false;
        talkIndex = 0;
        tmp = 0;
        serveQuestTalkIndex = 0;
        Cursor.lockState = CursorLockMode.Locked;
        targetNpc = null;
        panel.SetActive(isAction);
        player.GetComponent<PlayerInputSystem>().PlayerActions.Enable();
    }

    private void TalkMotion()
    {
        TimelineAsset timelineAsset = targetNpc.GetComponent<Npc>().Motion[0];
        targetNpc.GetComponent<PlayableDirector>().playableAsset = timelineAsset;
        targetNpc.GetComponent<PlayableDirector>().Play();
    }

    private void StopTalkMotion()
    {
        TimelineAsset timelineAsset = targetNpc.GetComponent<Npc>().Motion[1];
        targetNpc.GetComponent<PlayableDirector>().playableAsset = timelineAsset;
        targetNpc.GetComponent<PlayableDirector>().Play();
        targetNpc.GetComponent<Npc>().ResetTarget();
        targetNpc.GetComponent<Npc>().isHit= false;
    }

    private void ChestOpenFail()
    {
        Debug.Log("상자열기 실패");
        MinigameManager.Instance.ChangeSuccess -= ChestOpen;
    }
    private void ChestOpenSuccess()
    {
        Debug.Log("상자열기 성공");
        animator.SetBool("Open",true);
        MinigameManager.Instance.ChangeSuccess -= ChestOpen;
    }
   private void ChestOpen(int val) 
    {
        if (val == 1)
            ChestOpenSuccess();
        if (val == -1)
            ChestOpenFail();
    }
}