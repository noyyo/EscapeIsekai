using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Dialog : MonoBehaviour
{
    public QuestManager QuestManager;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI talkText;
    public RawImage texture;
    public GameObject panel;
    public Button nextB;
    public int talkIndex;

    private bool isAction;
    private GameObject targetNpc;

    public static Dialog Instance;
    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }
    public void Action(GameObject scanObj) //대화시작
    {
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
            panel.SetActive(isAction);
        }
         
            
    }
   public void Talk(int id, bool isNPC)
    {
        int questTalkIndex = QuestManager.GetQuestTalkIndex(id);
        string talkData = TalkManager.Instance.GetTalk(id+ questTalkIndex, talkIndex);
        if (talkData == null) 
        {
            ExitTalk();
            if(id == 1)
            {
                QuestManager.Instance.questId=10;
            }
            if (id == 100) //대장장이
            {
                ItemCraftingManager.Instance.CallOnCraftingUI();
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

            }
            if (id == 500) //검술
            {
                instructor.Instance.gameObject.SetActive(true);
                instructor.Instance.StartCoroutine("StartMission");
                    return;
            }
           
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

    public void ExitTalk()
    {
        StopTalkMotion();
        if (targetNpc.GetComponent<NavMeshAgent>() != null)
        {
            targetNpc.GetComponent<NavMeshAgent>().speed = 3.5f;
        }
        isAction = false;
        talkIndex = 0;
        Cursor.lockState = CursorLockMode.Locked;
        targetNpc = null;
        panel.SetActive(isAction);
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
}
