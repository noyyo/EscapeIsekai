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

    void GenerateData() //npc��ȣ , ����Ʈ ��ȣ
    {
        //������
        talkData.Add(1 + 0, new string[] { "���¹���","�����ʿ���","���� �ɾ��" });
        //��������Ʈ ������
        talkData.Add(1 + 10, new string[] { "������ ����","���ư��� ���ؼ� ������ ã�ƾ���", "ã�ƿ���" });
        talkData.Add(1 + 11, new string[] { "ù��° ������ �� ã�ƿԱ�","���� ���� ������ �ִ�","������ �ϵ���" });
        talkData.Add(1 + 12, new string[] { "�ι�° ������ �� ã�ƿԱ�", "���� ���� ������ �ִ�","�� ���� �ʾҾ� ��������" });
        talkData.Add(1 + 13, new string[] { "�����߳� ���� �������̾�","�� ������ �̰��� ���� �� �ְڱ�","�������ΰ���" });
        talkData.Add(1 + 14, new string[] { "������ ��� ��ұ�","�㿡 ���ְ� ��Ż�� ����� �����״�" });

        //��������
        talkData.Add(100 + 0, new string[] { "����" ,"�ҵ��ҵ��غ�"});
        //�丮��
        talkData.Add(200 + 0, new string[] { "����", "���������غ���" });
        //��ȭ��
        talkData.Add(300 + 0, new string[] { "����", "���췡" });
        //����
        talkData.Add(400 + 0, new string[] { "����", "�߷�?" });
        //�˼�
        talkData.Add(500 + 0, new string[] { "����", "������﷡?" });
        talkData.Add(505 + 0, new string[] { "�� ����� ����." });
        talkData.Add(506 + 0, new string[] { "�������� ���ϳ�" });
        talkData.Add(507 + 0, new string[] { "������ Ÿ��� ������"});
        //����
        talkData.Add(1000 + 0, new string[] { "���� ���ڴ�" });
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