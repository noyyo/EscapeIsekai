public class QuestData
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
