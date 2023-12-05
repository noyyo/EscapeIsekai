using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ServeQuestManager : MonoBehaviour
{
    [SerializeField]
    private ServeQuestDB ServeQuestDB;
    public Dictionary<int, ServeQuestData> questDBDic;
    public static ServeQuestManager Instance;
    public Dictionary<int, int> playerQuest; //0 미진행 1 진행중 2 완료
    public Dictionary<int, int> playerQuestKillCount;
    public Dictionary<int, int> playerQuestKillCount2;
    public GameObject parent;
    public TextMeshProUGUI header;
    public TextMeshProUGUI content;

    public event Action<int> CanAccept;
    public event Action<int> CanClear;
    public event Action<int> isAllClear;
    public event Action<int> updateQuest;

    public Dictionary<GameObject, int> questList = new Dictionary<GameObject, int>();
    public Dictionary<GameObject, int> questZoneDic = new Dictionary<GameObject, int>();
    private void Awake()
    {
        Instance = this;
        playerQuest = new Dictionary<int, int>();
        playerQuestKillCount = new Dictionary<int, int>();
        playerQuestKillCount2 = new Dictionary<int, int>();
        questDBDic = new Dictionary<int, ServeQuestData>();
        InitDB();
    }
    private void Start()
    {
        GameManager.Instance.Player.GetComponent<Inventory>().AddItem += QuestItemCheck;
        updateQuest += UpdateQuest;
    }
    public void UpdateQuest(int key)
    {
        var questPair = questList
 .Where(item => item.Value == key)
 .Select(item => item.Key)
 .ToList();
        if (questPair.Count != 0)
        {
            foreach (GameObject tmp in questPair)
            {
                if (tmp.gameObject.GetComponent<TextMeshProUGUI>().text == questDBDic[key].QuestName)
                {
                    tmp.gameObject.GetComponent<TextMeshProUGUI>().text += " -완료";
                }
            }
        }
    }
    public void AddQuestList(int key)
    {
        if (playerQuest[key] == 0)
        {
            if (questDBDic[key].QuestType == 1)
            {
                if (questDBDic[key].QuestItem2 > 0 && questDBDic[key].QuestItem > 0)
                {
                    TextMeshProUGUI tempheader = Instantiate(header, parent.transform);
                    tempheader.text = questDBDic[key].QuestName;
                    TextMeshProUGUI tempcontent = Instantiate(content, parent.transform);
                    ItemData DB = new ItemData();
                    ItemData DB2 = new ItemData();
                    string contentText = null;
                    if (ItemDB.Instance.GetItemData(questDBDic[key].QuestItem, out DB) && ItemDB.Instance.GetItemData(questDBDic[key].QuestItem2, out DB2))
                    {
                        contentText = $"{DB.ItemName} : {questDBDic[key].QuestItemCount}개를(을) 구해보자\n" +
                            $"{DB2.ItemName} : {questDBDic[key].QuestItem2Count}개를(을) 구해보자";
                    }
                    tempcontent.text = contentText;
                    questList.Add(tempheader.gameObject, key);
                    questList.Add(tempcontent.gameObject, key);
                }
                else
                {
                    TextMeshProUGUI tempheader = Instantiate(header, parent.transform);
                    tempheader.text = questDBDic[key].QuestName;
                    TextMeshProUGUI tempcontent = Instantiate(content, parent.transform);
                    ItemData DB = new ItemData();
                    string contentText = null;
                    if (ItemDB.Instance.GetItemData(questDBDic[key].QuestItem, out DB))
                    {
                        contentText = $"{DB.ItemName} : {questDBDic[key].QuestItemCount}개를(을) 구해보자";
                    }
                    tempcontent.text = contentText;
                    questList.Add(tempheader.gameObject, key);
                    questList.Add(tempcontent.gameObject, key);
                }
            }
            if (questDBDic[key].QuestType == 2)
            {
                if (questDBDic[key].QuestMonster > 0 && questDBDic[key].QuestMonster2 > 0)
                {
                    TextMeshProUGUI tempheader = Instantiate(header, parent.transform);
                    tempheader.text = questDBDic[key].QuestName;
                    TextMeshProUGUI tempcontent = Instantiate(content, parent.transform);
                    string contentText = null;
                    string monster1 = null;
                    string monster2 = null;
                    GameObject[] arry = GameObject.FindGameObjectsWithTag(TagsAndLayers.EnemySpawnerTag);
                    for (int i = 0; i < arry.Length; i++)
                    {
                        for (int j = 0; j < arry[i].GetComponent<EnemySpawner>().EnemyPrefabs.Length; j++)
                        {
                            if(questDBDic[key].QuestMonster == arry[i].GetComponent<EnemySpawner>().EnemyPrefabs[j].Data.ID)
                            {
                                monster1 = arry[i].GetComponent<EnemySpawner>().EnemyPrefabs[j].Data.Name;
                            }
                            if (questDBDic[key].QuestMonster2 == arry[i].GetComponent<EnemySpawner>().EnemyPrefabs[j].Data.ID)
                            {
                                monster2 = arry[i].GetComponent<EnemySpawner>().EnemyPrefabs[j].Data.Name;
                            }
                        }
                    }
                    contentText = $"{monster1}을(를) {questDBDic[key].QuestMonsterCount}마리 처치하자\n" +
                                  $"{monster2}을(를) {questDBDic[key].QuestMonster2Count}마리 처치하자";
                    tempcontent.text = contentText;
                    questList.Add(tempheader.gameObject, key);
                    questList.Add(tempcontent.gameObject, key);
                }
                else
                {
                    TextMeshProUGUI tempheader = Instantiate(header, parent.transform);
                    tempheader.text = questDBDic[key].QuestName;
                    TextMeshProUGUI tempcontent = Instantiate(content, parent.transform);
                    string contentText = null;
                    string monster1=null;
                    GameObject[] arry = GameObject.FindGameObjectsWithTag(TagsAndLayers.EnemySpawnerTag);
                    for (int i = 0; i < arry.Length; i++)
                    {
                        for (int j = 0; j < arry[i].GetComponent<EnemySpawner>().EnemyPrefabs.Length; j++)
                        {
                            if (questDBDic[key].QuestMonster == arry[i].GetComponent<EnemySpawner>().EnemyPrefabs[j].Data.ID)
                            {
                                monster1 = arry[i].GetComponent<EnemySpawner>().EnemyPrefabs[j].Data.Name;
                            }
                        }
                    }
                    contentText = $"{monster1}을(를) {questDBDic[key].QuestMonsterCount}마리 처치하자";
                    tempcontent.text = contentText;
                    questList.Add(tempheader.gameObject, key);
                    questList.Add(tempcontent.gameObject, key);
                }
            }
        }
        if (playerQuest[key] == 2)
        {
            List<GameObject> keysToRemove = new List<GameObject>();
            foreach (KeyValuePair<GameObject, int> Data in questList)
            {
                if (Data.Value == key)
                {
                    keysToRemove.Add(Data.Key);
                }
            }
            foreach (GameObject keyToRemove in keysToRemove)
            {
                Destroy(keyToRemove);
                questList.Remove(keyToRemove);
            }
        }
    }
    public bool MarkInit(int Npcid)
    {
        var questPair = questDBDic
.Where(item => item.Value.Npc == Npcid)
.Select(item => item.Key)
.ToList();
        if (questPair.Count == 0)
        {
            return false;
        }
        foreach (int val in questPair)
        {
            if (playerQuest[val] == 0)
            {
                return true;
            }
        }
        isAllClear?.Invoke(Npcid);
        return false;
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
        if (questPair.Count == 0)
        {
            return 0;
        }

        foreach (int val in questPair)
        {
            if (playerQuest[val] != 2)
            {
                return val;
            }
        }
        return questPair[0];
    }
    public string GetTalk(int key, int talkIndex)
    {
        string[] QuesttalkData = null;

        if (questDBDic.ContainsKey(key))
        {
            if (playerQuest[key] == 2)
                QuesttalkData = questDBDic[key].AfterDialog.Split(',');
            if (playerQuest[key] == 1 || playerQuest[key] == 0)
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
        if (questDBDic[key].QuestType == 1)
        {
            if (playerQuest.ContainsKey(key) && playerQuest[key] == 1)
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
            if (playerQuest.ContainsKey(key) && playerQuest[key] == 1)
            {
                if (questDBDic[key].QuestMonster > 0)
                {
                    if (questDBDic[key].QuestMonster2 > 0)
                    {
                        if (questDBDic[key].QuestMonsterCount <= playerQuestKillCount[key] && questDBDic[key].QuestMonster2Count <= playerQuestKillCount2[key])
                        {
                            playerQuest[key] = 2;
                            if (questDBDic[key].Reword > 0)
                            {
                                InventoryManager.Instance.CallAddItem(questDBDic[key].Reword, 1);
                            }
                        }
                    }
                    else if (questDBDic[key].QuestMonsterCount <= playerQuestKillCount[key])
                    {
                        playerQuest[key] = 2;
                        if (questDBDic[key].Reword > 0)
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
            if (ServeQuestDB.Sheet1[i].QuestType == 2)
            {
                if (ServeQuestDB.Sheet1[i].QuestMonster > 0)
                {
                    playerQuestKillCount.Add(ServeQuestDB.Sheet1[i].id, 0);
                }
                if (ServeQuestDB.Sheet1[i].QuestMonster2 > 0)
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
                if (questDBDic[id].QuestMonster == enemy.Data.ID)
                {
                    playerQuestKillCount[id]++;
                    if (playerQuestKillCount2.ContainsKey(id) && questDBDic[id].QuestMonster2 > 0)
                    {
                        if (questDBDic[id].QuestMonsterCount <= playerQuestKillCount[id] && questDBDic[id].QuestMonster2Count <= playerQuestKillCount2[id] && questDBDic[id].QuestType == 2)
                        {
                            CanClear?.Invoke(questDBDic[id].Npc);
                            updateQuest?.Invoke(id);
                        }
                    }
                    else
                    {
                        if (questDBDic[id].QuestMonsterCount <= playerQuestKillCount[id] && questDBDic[id].QuestType == 2)
                        {
                            CanClear?.Invoke(questDBDic[id].Npc);
                            updateQuest?.Invoke(id);
                        }
                    }
                }
                else if (questDBDic[id].QuestMonster2 == enemy.Data.ID)
                {
                    playerQuestKillCount2[id]++;
                    if (questDBDic[id].QuestMonsterCount <= playerQuestKillCount[id] && questDBDic[id].QuestMonster2Count <= playerQuestKillCount2[id] && questDBDic[id].QuestType == 2)
                    {
                        CanClear?.Invoke(questDBDic[id].Npc);
                        updateQuest?.Invoke(id);
                    }
                }
            }
        }
    }

    public void QuestItemCheck(int key)
    {
        if (questDBDic[key].QuestType == 1)
        {
            if (playerQuest[key] == 0 || (playerQuest[key] == 1))
            {
                if (questDBDic[key].QuestItem2 > 0)
                {
                    if (InventoryManager.Instance.CallIsCheckItem(questDBDic[key].QuestItem, questDBDic[key].QuestItemCount))
                    {
                        if (InventoryManager.Instance.CallIsCheckItem(questDBDic[key].QuestItem2, questDBDic[key].QuestItem2Count))
                        {
                            CanClear?.Invoke(questDBDic[key].Npc);
                            updateQuest?.Invoke(key);
                        }
                    }
                }
                else
                {
                    if (InventoryManager.Instance.CallIsCheckItem(questDBDic[key].QuestItem, questDBDic[key].QuestItemCount))
                    {
                        CanClear?.Invoke(questDBDic[key].Npc);
                        updateQuest?.Invoke(key);
                    }
                }
            }
        }
    }

    public void QuestItemCheck(int itemid, int count)
    {
        var Itemid = questDBDic
        .Where(item => item.Value.QuestItem == itemid)
        .Select(item => item.Key)
    .ToList();
        foreach (int val in Itemid)
        {
            if (playerQuest[val] == 1)
            {
                if (questDBDic[val].QuestItem == itemid)
                {
                    if (!InventoryManager.Instance.CallIsCheckItem(questDBDic[val].QuestItem, questDBDic[val].QuestItemCount))
                    {
                        return;
                    }
                }
                if (questDBDic[val].QuestItem2 == itemid)
                {
                    if (!InventoryManager.Instance.CallIsCheckItem(questDBDic[val].QuestItem2, questDBDic[val].QuestItem2Count))
                    {
                        return;
                    }
                }
                CanClear?.Invoke(questDBDic[val].Npc);
                updateQuest?.Invoke(val);
            }
        }
    }

    public void ChangeMark(int key, Npc npc)
    {
        npc.CheckeState(playerQuest[key]);
        if (playerQuest[key] == 2)
        {
            if (MarkInit(questDBDic[key].Npc))
            {
                CanAccept?.Invoke(questDBDic[key].Npc);
            }
        }
    }

    public void MakeQuestZone(int key)
    {
        if (questDBDic[key].QuestType == 1)
        {
            if (playerQuest[key] == 2)
            {
                List<GameObject> keysToRemove = new List<GameObject>();
                foreach (KeyValuePair<GameObject, int> Data in questZoneDic)
                {
                    if (Data.Value == key)
                    {
                        keysToRemove.Add(Data.Key);
                    }
                }
                foreach (GameObject keyToRemove in keysToRemove)
                {
                    Destroy(keyToRemove);
                    questZoneDic.Remove(keyToRemove);
                }
            }
            if (playerQuest[key] == 0)
            {
                GameObject[] arry = GameObject.FindGameObjectsWithTag(TagsAndLayers.ItemSpawnerTag);
                for (int i = 0; i < arry.Length; i++)
                {
                    if (arry[i].GetComponent<ItemSpawner>().itemId == questDBDic[key].QuestItem || arry[i].GetComponent<ItemSpawner>().itemId == questDBDic[key].QuestItem2)
                    {
                        GameObject gameObject = Instantiate(Resources.Load<GameObject>("Prefabs/Entities/Quest/QuestZone"), arry[i].transform.position + new Vector3(0, 20, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
                        gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        questZoneDic.Add(gameObject, key);

                    }
                }
            }
        }
        if (questDBDic[key].QuestType == 2)
        {
            if (playerQuest[key] == 2)
            {
                List<GameObject> keysToRemove = new List<GameObject>();
                foreach (KeyValuePair<GameObject, int> Data in questZoneDic)
                {
                    if (Data.Value == key)
                    {
                        keysToRemove.Add(Data.Key);
                    }
                }
                foreach (GameObject keyToRemove in keysToRemove)
                {
                    Destroy(keyToRemove);
                    questZoneDic.Remove(keyToRemove);
                }
            }
            if (playerQuest[key] == 0)
            {
                GameObject[] arry = GameObject.FindGameObjectsWithTag(TagsAndLayers.EnemySpawnerTag);
                for (int i = 0; i < arry.Length; i++)
                {
                    for (int j = 0; j < arry[i].GetComponent<EnemySpawner>().EnemyPrefabs.Length; j++)
                    {
                        if (arry[i].GetComponent<EnemySpawner>().EnemyPrefabs[j].Data.ID == questDBDic[key].QuestMonster || arry[i].GetComponent<EnemySpawner>().EnemyPrefabs[j].Data.ID == questDBDic[key].QuestMonster2)
                        {

                            questZoneDic.Add(Instantiate(Resources.Load<GameObject>("Prefabs/Entities/Quest/QuestZone"), arry[i].transform.position + new Vector3(0, 20, 0), Quaternion.Euler(new Vector3(90, 0, 0))), key);
                        }
                    }
                }
            }
        }
    }
}
