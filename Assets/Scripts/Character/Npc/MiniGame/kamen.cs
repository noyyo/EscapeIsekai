using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class kamen : MonoBehaviour
{   
    public static kamen Instance;
    public GameObject parent;
    public GameObject mark; //복사해서 쓸 프리팹
    [Tooltip("마크 몇개나 만들껀지")]
    private int howManyMark;
    public event Action<bool> MiniGameFinished;
    private GameObject markClone ;

    [HideInInspector]
    public int inputKey = 0;
    private bool isSuccess;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
       // StartCoroutine("StartMission");
    }
    private void Update()
    {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                inputKey = (int)Key.LEFT;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                inputKey = (int)Key.RIGHT;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                inputKey = (int)Key.UP;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                inputKey = (int)Key.DOWN;
            }
    }


    IEnumerator MakeMark(int x)
    {
        for (int i = 0; i < x;  i++)
        { 
            markClone = Instantiate(mark, parent.transform);
            markClone.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-300,300), Random.Range(-300, 300));
            yield return new WaitUntil(() => markClone.activeSelf==false);
            if (i == x-1&& markClone.activeSelf == false)
            {
                isSuccess = true;
            }              
        }
        MiniGameFinished?.Invoke(isSuccess);
        yield return null;
    }

    IEnumerator StartMission()
    {
        parent.SetActive(true);
        StartCoroutine("MakeMark", 2);
        yield return null;
    }
    
    public void MarkFail()
    {
        isSuccess=false;
        MiniGameFinished?.Invoke(isSuccess);
    }
}
