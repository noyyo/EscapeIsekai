using UnityEngine;

public class NpcQuestManager : CustomSingleton<NpcQuestManager>
{
    protected NpcQuestManager() { }
    private GameObject dialog;
    private GameObject questManager;
    private GameObject talkManager;

    public GameObject Dialog { get { return dialog; } }
    public GameObject QuestManager { get { return questManager; } }
    public GameObject TalkManager { get { return TalkManager; } }
    private void Awake()
    {
        if (dialog == null)
        {
            dialog = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/UI_Dialog"));
        }
        if (questManager == null)
        {
            questManager = Instantiate(Resources.Load<GameObject>("Prefabs/Manager/QuestManager"));
        }
        if (talkManager == null)
        {
            talkManager = Instantiate(Resources.Load<GameObject>("Prefabs/Manager/TalkManager"));
        }
    }
}
