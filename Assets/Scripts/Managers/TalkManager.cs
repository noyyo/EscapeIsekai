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
        talkData.Add(100 + 0, new string[] { "�ݰ���! ���� ������ �ʿ���?" ,"������ �ʿ��ϸ� �ѹ� �ѷ���"});
        //�丮��
        talkData.Add(200 + 0, new string[] { "���� �Դ°� ���� ����", "�Դ°Ϳ� ���� ����� �ʿ��Ե� �˷��ٰ�!" });
        //��ȭ��
        talkData.Add(300 + 0, new string[] { "���� �ʿ��Ѱ� �ִ�?", "�ѹ� �ѷ���" });
        //����
        talkData.Add(400 + 0, new string[] { "�������� ���±���", "���� �������� ���� ���°� �??" });
        //�˼�
        talkData.Add(500 + 0, new string[] { "����", "������﷡?" });
        talkData.Add(600 + 0, new string[] { "���� ���� ���Ͱ� ����","�װ��� ���� ���� �������Ͽ�","Ư���� ���𰡸� ���� ������ ����ִٰ���" });
        //����
        talkData.Add(1000 + 0, new string[] { "���� ���ڴ�" });
        talkData.Add(1200 + 0, new string[] { "�����༭ ����" });
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