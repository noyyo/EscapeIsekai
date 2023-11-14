using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class ActionDataSources : MonoBehaviour
{
    public static ActionDataSources Instance { get; private set; }
    private List<AttackAction> allActions;
    [InspectorLabel("게임내에 존재하는 모든 액션 프로토타입은 여기 위치해야 합니다.")]
    public AttackAction[] ActionPrototypes;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Already ActionDataSource exist!");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        //BuildActionIDs();
        DontDestroyOnLoad(gameObject);
    }

    public AttackAction GetActionPrototypeByID(ActionID index)
    {
        return allActions[index.ID];
    }

    //public bool TryGetActionPrototypeByID(ActionID index, out Action action)
    //{
    //    for (int i = 0; i < allActions.Count; i++)
    //    {
    //        if (allActions[i].ActionID == index)
    //        {
    //            action = allActions[i];
    //            return true;
    //        }
    //    }

    //    action = null;
    //    return false;
    //}

    //void BuildActionIDs()
    //{
    //    HashSet<Action> uniqueActions = new HashSet<Action>(ActionPrototypes);

    //    allActions = new List<Action>(uniqueActions.Count);

    //    int i = 0;
    //    foreach (Action uniqueAction in uniqueActions)
    //    {
    //        uniqueAction.ActionID = new ActionID { ID = i };
    //        allActions.Add(uniqueAction);
    //        i++;
    //    }
    //}
}
