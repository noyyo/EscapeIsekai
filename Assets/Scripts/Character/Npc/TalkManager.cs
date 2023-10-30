using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    public static TalkManager instance;
    private QuestManager questManager;
    Dictionary<int, string[]> talkData;
    
    void Awake()
    {
        instance = this;
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData() //npc번호 , 퀘스트 번호
    {
        //마법사
        talkData.Add(1 + 0, new string[] { "나는법사" });
        //메인퀘스트 마법사
        talkData.Add(1 + 10, new string[] { "이곳 방문은 처음인가?","돌아가긴 위해선 유물을 찾아야해", "찾아오렴" });
        talkData.Add(1 + 11, new string[] { "첫번째 유물을 잘 찾아왔군","다음 땡떙 지역에 있다","몸조심 하도록" });
        talkData.Add(1 + 12, new string[] { "두번째 유물을 잘 찾아왔군", "다음 땡떙 지역에 있다","얼마 남지 않았어 힘내도록" });
        talkData.Add(1 + 13, new string[] { "수고했네 이제 마지막이야","곧 있으면 이곳을 떠날 수 있겠군","남쪽으로가게" });
        talkData.Add(1 + 14, new string[] { "유물을 모두 모았군","밤에 와주게 포탈을 만들어 놓을테니" });

        //대장장이
        talkData.Add(100 + 0, new string[] { "하이" ,"뚝딱뚝딱해봐"});
        //요리사
        talkData.Add(200 + 0, new string[] { "하이", "지글지글해보실" });
        //잡화점
        talkData.Add(300 + 0, new string[] { "하이", "뭐살래" });
        //여관
        talkData.Add(400 + 0, new string[] { "하이", "잘래?" });
        //검술
        talkData.Add(500 + 0, new string[] { "하이", "검좀배울래?" });

        //상자
        talkData.Add(1000 + 0, new string[] { "낡은 상자다" });
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
        if (talkIndex == talkData[id].Length) 
            return null;
        else
            return talkData[id][talkIndex]; 
    }
}