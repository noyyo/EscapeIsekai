using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public static class ActionFactory
{
    private static Dictionary<ActionID, ObjectPool<AttackAction>> ActionPools = new Dictionary<ActionID, ObjectPool<AttackAction>>();

    //private static ObjectPool<AttackAction> GetActionPool(ActionID actionID)
    //{
    //    if (!ActionPools.TryGetValue(actionID, out var actionPool))
    //    {
    //        actionPool = new ObjectPool<AttackAction>(
    //            createFunc: () => Object.Instantiate(ActionDataSources.Instance.GetActionPrototypeByID(actionID)),
    //            actionOnRelease: action => action.ResetAction(),
    //            actionOnDestroy: Object.Destroy);

    //        ActionPools.Add(actionID, actionPool);
    //    }

    //    return actionPool;
    //}


    /// <summary>
    /// Factory method that creates Actions from their request data.
    /// </summary>
    /// <param name="data">the data to instantiate this skill from. </param>
    /// <returns>the newly created action. </returns>
    //public static Action CreateCopyAction(Action data)
    //{
    //    var ret = GetActionPool(data.ActionID).Get();
    //    ret.Initialize(ref data);
    //    return ret;
    //}

    //public static void ReturnAction(Action action)
    //{
    //    var pool = GetActionPool(action.ActionID);
    //    pool.Release(action);
    //}

    public static void PurgePooledActions()
    {
        foreach (var actionPool in ActionPools.Values)
        {
            actionPool.Clear();
        }
    }
}
