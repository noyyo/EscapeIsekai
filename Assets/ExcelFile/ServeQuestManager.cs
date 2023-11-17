using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class ServeQuestManager : MonoBehaviour
{
    [SerializeField]
    private ServeQuestDB ServeQuestDB;
    public Dictionary<int, ServeQuestData> questDBDic;
    public static ServeQuestManager Instance;
    public Dictionary<int, int> playerQuest; //0 미진행 1 진행중 2 완료
    public Dictionary<int, int> playerQuestKillCount;
    public Dictionary<int, int> playerQuestKillCount2;
    private void Awake()
    {
        Instance = this;
        playerQuest = new Dictionary<int, int>();
        playerQuestKillCount = new Dictionary<int, int>();
        playerQuestKillCount2 = new Dictionary<int, int>();
        questDBDic = new Dictionary<int, ServeQuestData>();
    }
    private void Start()
    {
        InitDB();
    }

    public int GetQuest(int Npcid)
    {
        var questPair = questDBDic
.Where(item => item.Value.Npc == Npcid || item.Value.Npc == Npcid)
.Select(item => item.Key)
.ToList();
        if (questPair.Equals(default(KeyValuePair<int, ServeQuestData>)))
        {
            return 0;
        }
        if(questPair.Count ==0) 
        {
            return 0; }
     
        foreach(int val in questPair)
        {
            if (playerQuest[val] !=2)
            {
                return val;
            }
        }
        return questPair[0];
        //return key;
        //var questPair = questDBDic.FirstOrDefault(item => item.Value.Npc == Npcid );
        //if (questPair.Equals(default(KeyValuePair<int, ServeQuestData>)))
        //{
        //    return 0; 
        //}
        //int key = questPair.Key;
        //return key;
    }
    public string GetTalk(int key, int talkIndex)
    {
        string[] QuesttalkData = null;

        if (questDBDic.ContainsKey(key))
        {
            if (playerQuest[key]==2)
                QuesttalkData = questDBDic[key].AfterDialog.Split(',');
            if(playerQuest[key]==1 || playerQuest[key] == 0)
                QuesttalkData = questDBDic[key].BeforeDialog.Split(',');
        }

        if (QuesttalkData == null)
            return null;

        if (talkIndex >= 0 && talkIndex < QuesttalkData.Length)
            return QuesttalkData[talkIndex];

        return null;
    }

    public void QuestClearCheck(int key)
    {
        if(questDBDic[key].QuestType==1)
        {
            if(playerQuest.ContainsKey(key) && playerQuest[key] == 1)
            {
                if (questDBDic[key].QuestItem2 > 0)
                {
                    if (InventoryManager.Instance.CallIsCheckItem(questDBDic[key].QuestItem, questDBDic[key].QuestItemCount))
                    {
                        if (InventoryManager.Instance.CallIsCheckItem(questDBDic[key].QuestItem2, questDBDic[key].QuestItem2Count))
                        {
                            playerQuest[key] = 2;
                            InventoryManager.Instance.CallAddItem(questDBDic[key].QuestItem, -questDBDic[key].QuestItemCount);
                            InventoryManager.Instance.CallAddItem(questDBDic[key].QuestItem2, -questDBDic[key].QuestItem2Count);
                            if (questDBDic[key].Reword > 0)
                            {
                                InventoryManager.Instance.CallAddItem(questDBDic[key].Reword, 1);
                            }
                        }
                    }
                }
                else
                {
                    if (InventoryManager.Instance.CallIsCheckItem(questDBDic[key].QuestItem, questDBDic[key].QuestItemCount))
                    {
                        if (playerQuest[key] == 1)
                        {
                            playerQuest[key] = 2;
                            Debug.Log("d");
                            InventoryManager.Instance.CallAddItem(questDBDic[key].QuestItem, -questDBDic[key].QuestItemCount);
                            if (questDBDic[key].Reword > 0)
                            {
                                InventoryManager.Instance.CallAddItem(questDBDic[key].Reword, 1);
                            }
                        }
                    }
                }
            }
        }
        if (questDBDic[key].QuestType == 2)
        {
            if(playerQuest.ContainsKey(key)&&playerQuest[key]==1)
            {
                if(questDBDic[key].QuestMonster > 0)
                {
                    if(questDBDic[key].QuestMonster2 > 0)
                    {
                        if (questDBDic[key].QuestMonsterCount <= playerQuestKillCount[key] && questDBDic[key].QuestMonsterCount <= playerQuestKillCount2[key])
                        {
                            Debug.Log("퀘성공");
                            playerQuest[key] = 2;
                            playerQuestKillCount.Remove(key);
                            if (questDBDic[key].Reword != 0)
                            {
                                InventoryManager.Instance.CallAddItem(questDBDic[key].Reword, 1);
                            }
                        }
                    }
                   else if (questDBDic[key].QuestMonsterCount <= playerQuestKillCount[key])
                    {
                        Debug.Log("퀘성공");
                        playerQuest[key] = 2;
                        playerQuestKillCount.Remove(key);
                        if (questDBDic[key].Reword != 0)
                        {
                            InventoryManager.Instance.Inventory.TryAddItem(questDBDic[key].Reword, 1);
                        }
                    }
                }
            }
        }

    }

    private void InitDB()
    {
        for (int i = 0; i < ServeQuestDB.Sheet1.Count; i++)
        {
            questDBDic.Add(ServeQuestDB.Sheet1[i].id, ServeQuestDB.Sheet1[i]);
            playerQuest.Add(ServeQuestDB.Sheet1[i].id, 0);
            if (ServeQuestDB.Sheet1[i].QuestType ==2)
            {
                if(ServeQuestDB.Sheet1[i].QuestMonster > 0)
                {
                    playerQuestKillCount.Add(ServeQuestDB.Sheet1[i].id, 0);
                }
                if(ServeQuestDB.Sheet1[i].QuestMonster2 > 0)
                {
                    playerQuestKillCount2.Add(ServeQuestDB.Sheet1[i].id, 0);
                }
            }
        }
    }

    public void QuestMonsterCheck(Enemy enemy)
    {
        var monsterid = questDBDic
    .Where(item => item.Value.QuestMonster == enemy.Data.ID || item.Value.QuestMonster2 == enemy.Data.ID)
    .Select(item => item.Key)
    .ToList();
        if (monsterid.Equals(default(KeyValuePair<int, ServeQuestData>)))
        {
            return;
        }

        foreach (int id in monsterid)
        {
            if (playerQuest[id] == 1)
            {
                if(questDBDic[id].QuestMonster == enemy.Data.ID)
                {
                    Debug.Log(id);
                    playerQuestKillCount[id]++;
                }
                else if (questDBDic[id].QuestMonster2 == enemy.Data.ID)
                {
                    Debug.Log(id);
                    playerQuestKillCount2[id]++;
                }
            }
        }
    }
}
