using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public GameObject[] miniGames;

    private void Awake()
    {

    }

    private void Start()
    {
       GameObject temp= Instantiate(miniGames[0]);
        temp.GetComponent<instructor>().StartCoroutine("StartMission");
    }
}
