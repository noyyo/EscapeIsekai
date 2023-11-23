using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : CustomSingleton<TalkManager>
{
    private QuestManager questManager;
    Dictionary<int, string[]> talkData;
    
    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData() //npc번호 , 퀘스트 번호
    {
        //마법사
        talkData.Add(1 + 0, new string[] { "나는법사","템이필요해","말좀 걸어봐" });
        //메인퀘스트 마법사
        talkData.Add(1 + 10, new string[] { "모험을 떠나","돌아가긴 위해선 유물을 찾아야해", "찾아오렴" });
        talkData.Add(1 + 11, new string[] { "첫번째 유물을 잘 찾아왔군","다음 땡떙 지역에 있다","몸조심 하도록" });
        talkData.Add(1 + 12, new string[] { "두번째 유물을 잘 찾아왔군", "다음 땡떙 지역에 있다","얼마 남지 않았어 힘내도록" });
        talkData.Add(1 + 13, new string[] { "수고했네 이제 마지막이야","곧 있으면 이곳을 떠날 수 있겠군","남쪽으로가게" });
        talkData.Add(1 + 14, new string[] { "유물을 모두 모았군","밤에 와주게 포탈을 만들어 놓을테니" });

        //대장장이
        talkData.Add(100 + 0, new string[] { "반가워! 뭔가 도움이 필요해?" ,"제작이 필요하면 한번 둘러봐"});
        //요리사
        talkData.Add(200 + 0, new string[] { "나는 먹는게 제일 좋아", "먹는것에 대한 기쁨을 너에게도 알려줄게!" });
        //잡화점
        talkData.Add(300 + 0, new string[] { "무언가 필요한게 있니?", "한번 둘러봐" });
        //여관
        talkData.Add(400 + 0, new string[] { "오랜만에 보는구나", "많이 지쳤으면 쉬고 가는건 어때??" });
        //검술
        talkData.Add(500 + 0, new string[] { "하이", "검좀배울래?" });
        talkData.Add(600 + 0, new string[] { "마을 밖은 몬스터가 많아","항간에 듣기로 마을 동서남북에","특별한 무언가를 지닌 괴물이 살고있다고해" });
        //상자
        talkData.Add(1000 + 0, new string[] { "낡은 상자다" });
        talkData.Add(1200 + 0, new string[] { "도와줘서 고마워" });
    }
    public string GetTalk(int id, int talkIndex) 
    {
        if(!talkData.ContainsKey(id))
        {
            if (talkIndex == talkData[id - id % 100].Length)
                return null;
            else
                return talkData[id - id % 100][talkIndex];
        }
        if (talkIndex >= talkData[id].Length) 
            return null;
        else
            return talkData[id][talkIndex]; 
    }
}