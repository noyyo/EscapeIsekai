using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int HP { get; set; }
    public abstract void TakeDamage(int damage);
    public event Action OnDie;
    public bool CanTakeAttackEffect { get; set; }
}
