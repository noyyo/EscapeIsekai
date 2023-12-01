using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStats
{
    [SerializeField] private int id;
    [SerializeField] private int hp;
    [SerializeField] private int temperature;
    [SerializeField] private float atk;
    [SerializeField] private float def;
    [SerializeField] private float speed;
    [SerializeField] private float stamina;
    [SerializeField] private int hunger;

    public int ID { get { return id; } }
    public int HP { get { return hp; } }
    public int Temperature { get { return temperature; } }
    public float ATK { get { return atk; } }
    public float DEF { get { return def; } }
    public float Speed { get { return speed; } }
    public float Stamina { get { return stamina; } }
    public int Hunger { get { return hunger; } }
}
