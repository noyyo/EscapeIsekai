using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class QuestManager : MonoBehaviour
{
    //보스 완료후 퀘스트 완료 희망 시 QuestClear() 사용
    public bool[] questCheck = new bool[4];
    public int questId;

    Dictionary<int, QuestData> questList;

    private void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerataData();
    }

    private void GenerataData()
    {
        //메인 퀘스트
        questList.Add(10, new QuestData("마법사와 첫만남- 유물을 찾아서", new int[] {1} ));
        questList.Add(11, new QuestData("두번째 유물을 찾아서", new int[] { 1 }));
        questList.Add(12, new QuestData("세번째 유물을 찾아서", new int[] { 1 }));
        questList.Add(13, new QuestData("마지막 유물을 찾아서", new int[] { 1 }));
        questList.Add(14, new QuestData("집으로", new int[] { 1 }));

    }

    public int GetQuestTalkIndex(int id)
    {
        return questId;
    }

    public void QuestClear() //메인퀘 클리어
    {
        for(int i = 0; i < 4; i++ )
        {
            if(!questCheck[i])
            {
                questCheck[i] = true;
                break;
            }
        }
    }
}
