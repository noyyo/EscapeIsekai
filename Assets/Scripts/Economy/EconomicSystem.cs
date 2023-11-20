using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomicSystem : ITradable
{
    public event Action OnDeal;

    public void TakeItem(Item newItem)
    {
        throw new NotImplementedException();
    }

    public void TakeMonery(int Monery)
    {
        throw new NotImplementedException();
    }
}
