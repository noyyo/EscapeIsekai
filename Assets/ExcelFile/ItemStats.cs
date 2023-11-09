using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStats
{
    [SerializeField] private int _id;
    [SerializeField] private int _hp;
    [SerializeField] private int _temperature;
    [SerializeField] private float _atk;
    [SerializeField] private float _def;
    [SerializeField] private float _speed;
    [SerializeField] private float _stamina;
    private Dictionary<string, float> _stats;


    public int ID { get { return _id; } }
    public int HP { get { return _hp; } }
    public int Temperature { get { return _temperature; } }
    public float ATK { get { return _atk; } }
    public float DEF { get { return _def; } }
    public float Speed { get { return _speed; } }
    public float Stamina { get { return _stamina; } }

    public Dictionary<string, float> Stats
    {
        get
        {
            if(_stats == null)
            {
                _stats = new Dictionary<string, float>()
                {
                    {"HP", (float)_hp},
                    {"Temperature", (float)_temperature},
                    {"ATK", (float)_atk},
                    {"DEF", (float)_def},
                    {"Speed", (float)_speed},
                    {"Stamina", (float)_stamina}
                };
            }
            return _stats;
        }
    }
}
