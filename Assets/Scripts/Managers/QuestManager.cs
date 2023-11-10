using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class QuestManager : MonoBehaviour
{
    //���� �Ϸ��� ����Ʈ �Ϸ� ��� �� QuestClear() ���
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
        //���� ����Ʈ
        questList.Add(10, new QuestData("������� ù����- ������ ã�Ƽ�", new int[] {1} ,"������ ã���� ������ ��������"));
        questList.Add(11, new QuestData("�ι�° ������ ã�Ƽ�", new int[] { 1 },"�Ȱ��������� ������"));
        questList.Add(12, new QuestData("����° ������ ã�Ƽ�", new int[] { 1 }, "�Ȱ��������� ������"));
        questList.Add(13, new QuestData("������ ������ ã�Ƽ�", new int[] { 1 }, "�Ȱ��������� ������"));
        questList.Add(14, new QuestData("������", new int[] { 1 }, "�޸�������"));

    }

    public int GetQuestTalkIndex(int id)
    {
        int index = 0;
        for (int i = 0; i < 4; i++) //����Ʈ Ŭ�����ߴ��� üũ
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
