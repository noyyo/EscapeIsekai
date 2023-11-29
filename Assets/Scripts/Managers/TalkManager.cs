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

    void GenerateData() //npc번호 , 디폴트 대화 (*아무런 퀘스트가 없는 상태일때)
    {
        //마법사
        talkData.Add(1 + 0, new string[] { "마을에서는 처음보는 사람이네??","혹시 다른 세계에서 또 빨려들어온 이방인인가...","잠시 생각할 시간을 좀 주게","내 방법을 좀 찾아보지" });
        //메인퀘스트 마법사
        talkData.Add(1 + 10, new string[] { "저번 이방인이 돌아갔던 방법을 참고해서 문헌을 뒤져보았네","돌아가긴 위해선 유물을 찾아야 할꺼 같네", "마을 주위 지역에 거주하고 있는 괴물들을 피해서 유물을 모아와야하네","발견 하면 다시 오게나.." });
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
        //경비병
        talkData.Add(600 + 0, new string[] { "마을 밖은 몬스터가 많아","항간에 듣기로 마을 동서남북에","특별한 무언가를 지닌 괴물이 살고있다고해" });
        //차원문
        talkData.Add(700 + 0, new string[] { "차원문" });
        //상자
        talkData.Add(1000 + 0, new string[] { "버려진 상자인거 같다" });

        talkData.Add(1100 + 0, new string[] { "구해준 아령도 얼마안가 못쓰게 될꺼같아..","다음에 또 부탁하면 좀 그렇겠지?..." });
        talkData.Add(1200 + 0, new string[] { "도와줘서 고마워","꼭 원래 세계로 돌아가길" });
        talkData.Add(1300 + 0, new string[] { "이정도면 거래면 조만간 부자가 될거야","다 너 덕분이야 이방인" });
        talkData.Add(1400 + 0, new string[] { "도적이 되고싶은자...나에게로" });
        talkData.Add(1500 + 0, new string[] { "모험가 때려치우고 사기꾼이나 해볼까.." });
        talkData.Add(1600 + 0, new string[] { "덕분에 이번 겨울도 잘 날 수 있을꺼같아","고마워" });
        talkData.Add(1700 + 0, new string[] { "나도 이참에 이방인의 세계로 떠나볼까.." });
        talkData.Add(1800 + 0, new string[] { "하루빨리 깊은숲속으로 들어가","숲의 정령이 당신을 기다리고있어" });
        talkData.Add(1900 + 0, new string[] { "숲의 가호가 함께하길" });
        talkData.Add(2000 + 0, new string[] { "하루빨리 깊은숲속으로 들어가", "숲의 정령이 당신을 기다리고있어" });
        talkData.Add(2100 + 0, new string[] { "나도 이참에 이방인의 세계로 떠나볼까.." });
        talkData.Add(2200 + 0, new string[] { "왜 여기서 쓰러져 있는거야??","그냥 지나가려다가 챙겨줄게","내가 엎고 갈꺼니까 숙박료는 두배다??","앞으론 조심해" });
        //튜토리얼
        talkData.Add(9900 + 0, new string[] { "차원문에 딸려온 이방인이구나??","여기는 너랑 살던 세계랑은 좀 다른곳이야","종종 이런일이 발생해서 들고다니는 책자가 있는데 한번 읽어봐","다시 원래 세계로 돌아가길 나도 기도하고있을게" });
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