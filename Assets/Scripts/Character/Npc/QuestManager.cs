using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class QuestManager : MonoBehaviour
{
    //���� �Ϸ��� ����Ʈ �Ϸ� ��� �� QuestClear() ���
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
        //���� ����Ʈ
        questList.Add(10, new QuestData("������� ù����- ������ ã�Ƽ�", new int[] {1} ));
        questList.Add(11, new QuestData("�ι�° ������ ã�Ƽ�", new int[] { 1 }));
        questList.Add(12, new QuestData("����° ������ ã�Ƽ�", new int[] { 1 }));
        questList.Add(13, new QuestData("������ ������ ã�Ƽ�", new int[] { 1 }));
        questList.Add(14, new QuestData("������", new int[] { 1 }));

    }

    public int GetQuestTalkIndex(int id)
    {
        return questId;
    }

    public void QuestClear() //������ Ŭ����
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
