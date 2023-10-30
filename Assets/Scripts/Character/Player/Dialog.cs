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

    public static Dialog instance;
    private void Awake()
    {
        instance = this;
        panel.SetActive(false);
    }
    public void Action(GameObject scanObj) //��ȭ����
    {
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
            TalkMotion();
            Talk(npcData.id, npcData.isNPC);
        }
          panel.SetActive(isAction); 
    }

    public void BtnAction()
    {
            Npc npcData = targetNpc.GetComponent<Npc>();
            Talk(npcData.id, npcData.isNPC);
    }
    void Talk(int id, bool isNPC)
    {
        int questTalkIndex = QuestManager.GetQuestTalkIndex(id);
        for (int i = 0; i < 4; i++) //����Ʈ Ŭ�����ߴ��� üũ
        {
            if (QuestManager.questCheck[i])
            {
                questTalkIndex++;
            }
            else break;
        }
        string talkData = TalkManager.instance.GetTalk(id+ questTalkIndex, talkIndex);
        if (talkData == null) 
        {
            if(id == 100) //��������
            {
                Debug.Log("��������ui�˾�");
            }
            if (id == 200) //�丮
            {

            }
            if (id == 300) //��ȭ
            {

            }
            if (id == 400) //����
            {

            }
            if (id == 500) //�˼�
            {

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
    }
}
