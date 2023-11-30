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

    void GenerateData() //npc��ȣ , ����Ʈ ��ȭ (*�ƹ��� ����Ʈ�� ���� �����϶�)
    {
        //������
        talkData.Add(1 + 0, new string[] { "���������� ó������ ����̳�??","Ȥ�� �ٸ� ���迡�� �� �������� �̹����ΰ�...","��� ������ �ð��� �� �ְ�","�� ����� �� ã�ƺ���" });
        //��������Ʈ ������
        talkData.Add(1 + 10, new string[] { "���� �̹����� ���ư��� ����� �����ؼ� ������ �������ҳ�","���ư��� ���ؼ� ������ ã�ƾ� �Ҳ� ����", "���� ���� ������ �����ϰ� �ִ� �������� ���ؼ� ������ ��ƿ;��ϳ�","�߰� �ϸ� �ٽ� ���Գ�.." });
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
        //���
        talkData.Add(600 + 0, new string[] { "���� ���� ���Ͱ� ����","�װ��� ���� ���� �������Ͽ�","Ư���� ���𰡸� ���� ������ ����ִٰ���" });
        //������
        talkData.Add(700 + 0, new string[] { "������" });
        //����
        talkData.Add(1000 + 0, new string[] { "������ �����ΰ� ����" });

        talkData.Add(1100 + 0, new string[] { "������ �Ʒɵ� �󸶾Ȱ� ������ �ɲ�����..","������ �� ��Ź�ϸ� �� �׷�����?..." });
        talkData.Add(1200 + 0, new string[] { "�����༭ ����","�� ���� ����� ���ư���" });
        talkData.Add(1300 + 0, new string[] { "�������� �ŷ��� ������ ���ڰ� �ɰž�","�� �� �����̾� �̹���" });
        talkData.Add(1400 + 0, new string[] { "������ �ǰ������...�����Է�" });
        talkData.Add(1500 + 0, new string[] { "���谡 ����ġ��� �����̳� �غ���.." });
        talkData.Add(1600 + 0, new string[] { "���п� �̹� �ܿﵵ �� �� �� ����������","����" });
        talkData.Add(1700 + 0, new string[] { "���� ������ �̹����� ����� ��������.." });
        talkData.Add(1800 + 0, new string[] { "�Ϸ绡�� ������������ ��","���� ������ ����� ��ٸ����־�" });
        talkData.Add(1900 + 0, new string[] { "���� ��ȣ�� �Բ��ϱ�" });
        talkData.Add(2000 + 0, new string[] { "�Ϸ绡�� ������������ ��", "���� ������ ����� ��ٸ����־�" });
        talkData.Add(2100 + 0, new string[] { "���� ������ �̹����� ����� ��������.." });
        talkData.Add(2200 + 0, new string[] { "�� ���⼭ ������ �ִ°ž�??","�׳� ���������ٰ� ì���ٰ�","���� ���� �����ϱ� ���ڷ�� �ι��??","������ ������" });
        //Ʃ�丮��
        talkData.Add(9900 + 0, new string[] { "�������� ������ �̹����̱���??","����� �ʶ� ��� ������� �� �ٸ����̾�","���� �̷����� �߻��ؼ� ���ٴϴ� å�ڰ� �ִµ� �ѹ� �о��","�ٽ� ���� ����� ���ư��� ���� �⵵�ϰ�������" });
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