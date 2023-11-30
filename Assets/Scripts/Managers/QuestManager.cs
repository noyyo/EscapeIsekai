using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class QuestManager : MonoBehaviour
{
    //보스 완료후 퀘스트 완료 희망 시 QuestClear() 사용
    public bool[] questCheck = new bool[4];
    public int questId;
    public TextMeshProUGUI header;
    public TextMeshProUGUI content;
    Dictionary<int, QuestData> questList;
    public static QuestManager Instance;

    private void Awake()
    {
        Instance = this;
        questList = new Dictionary<int, QuestData>();
        GenerataData();
    }

    private void GenerataData()
    {
        //메인 퀘스트
        questList.Add(10, new QuestData("마법사와 첫만남- 유물을 찾아서", new int[] {1} , "마법사에게 정보를 듣고 유물을 찾아보자."));
        questList.Add(11, new QuestData("두번째 유물을 찾아서", new int[] { 1 },"유물을 찾아서 마법사에게 전달하자."));
        questList.Add(12, new QuestData("세번째 유물을 찾아서", new int[] { 1 }, "유물을 찾아서 마법사에게 전달하자."));
        questList.Add(13, new QuestData("마지막 유물을 찾아서", new int[] { 1 }, "유물을 찾아서 마법사에게 전달하자."));
        questList.Add(14, new QuestData("집으로", new int[] { 1 }, "뒷마당으로"));

    }

    public int GetQuestTalkIndex(int id)
    {
        int index = 0;
        for (int i = 0; i < 4; i++) //퀘스트 클리어했는지 체크
        {
            if (questCheck[i])
            {
                index++;
            }
            else break;
        }
        if(questId != 0 )
        {
        header.text = questList[questId + index].questName;
        content.text = questList[questId + index].questDescription;
        }

        return questId+ index;
    }

    public void QuestClear() //메인퀘 클리어
    {
        for(int i = 0; i < 4; i++ )
        {
            if(!questCheck[i])
            {
                if(i<3)
                {
                    GameManager.Instance.Player.GetComponent<Player>().playerUI.Locks[i].gameObject.SetActive(false);
                }
                questCheck[i] = true;
                break;
            }
        }
    }


}
