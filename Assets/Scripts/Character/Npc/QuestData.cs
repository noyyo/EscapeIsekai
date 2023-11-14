using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : MonoBehaviour
{
   public string questName;
    public int[] npcId;
    public string questDescription;
    public QuestData(string name, int[] npc, string Description)
    {
        questName = name;
        npcId = npc;
        questDescription = Description;
    }
}
