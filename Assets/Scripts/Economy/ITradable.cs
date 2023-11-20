using System;

public interface ITradable
{
    public event Action OnDeal;
    public abstract void TakeMonery(int Monery);
    public abstract void TakeItem(Item newItem);
}