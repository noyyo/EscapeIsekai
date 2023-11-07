using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : CustomSingleton<GameManager>
{
    [Range(0.0f, 1.0f)]
    public float time; //하루 사이클 시간  0.2~0.8 해떠있는 시간
    public bool IsDay;
}
