using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : CustomSingleton<GameManager>
{
    [Range(0.0f, 1.0f)]
    public float time; //�Ϸ� ����Ŭ �ð�  0.2~0.8 �ض��ִ� �ð�
}
