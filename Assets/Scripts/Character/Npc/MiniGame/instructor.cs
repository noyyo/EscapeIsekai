using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using Random = UnityEngine.Random;

public enum Key
{
    LEFT=61,
    RIGHT=62,
    UP=63,
    DOWN=64
}
public class instructor : MonoBehaviour
{
    public static instructor Instance;

    public bool[] getSkill = new bool[3];
    public Sprite[] arrows = new Sprite[4];
    public GameObject parent;
    public Image combo;
    public Image fail;
    public GameObject targetCanvas;
    public Slider timeGauge;

    private float failTime= 10f; //타임아웃 시간
    private int failCount;
    private int inputKey = 0;
    private List<Key> comboKey = new List<Key>(); //초기배열
    private bool isSuccess;
    public event System.Action<bool> MiniGameFinished;
    private void Start()
    {
        fail.gameObject.SetActive(false);
      // StartCoroutine("StartMission");
    }

    private void Awake()
    {
        targetCanvas.gameObject.SetActive(false);
        Instance = this;
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
    void MakeCombo() //combokey의 초기화
    {
        int x =  Random.Range(5, 10); // 몇개 만들껀지

        for(int i = 0; i < x; i++)
        {
            Key key = (Key)Random.Range(61,64);
            if (key == Key.LEFT)
            {
                combo.sprite = arrows[0];
                combo.GetComponent<ComboData>().value = 61;
            }
            if (key == Key.RIGHT)
            {
                combo.sprite = arrows[1];
                combo.GetComponent<ComboData>().value = 62;
            }
            if (key == Key.UP)
            {
                combo.sprite = arrows[2];
                combo.GetComponent<ComboData>().value = 63;
            }
            if (key == Key.DOWN)
            {
                combo.sprite = arrows[3];
                combo.GetComponent<ComboData>().value = 64;
            }
            Instantiate(combo, parent.transform);
            comboKey.Add(key);
        }
    }

    void ReMake()
    {
        for(int i = 0; i <parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
        for(int i = 0; i < comboKey.Count; i++)
        {
            combo.GetComponent<ComboData>().value = (int)comboKey[i];
            if (comboKey[i] == Key.LEFT)
            {
                combo.sprite = arrows[0];
            }
            if (comboKey[i] == Key.RIGHT)
            {
                combo.sprite = arrows[1];
            }
            if (comboKey[i] == Key.UP)
            {
                combo.sprite = arrows[2];
            }
            if (comboKey[i] == Key.DOWN)
            {
                combo.sprite = arrows[3];
            }
            Instantiate(combo, parent.transform);
        }
    }
  public  IEnumerator StartMission()
    {
        targetCanvas.gameObject.SetActive(true);
        StartCoroutine("TimeOut");
        MakeCombo();
        parent.transform.GetChild(0).GetComponent<Image>().color = Color.red;
        while (true)
        {
            if (parent.transform.childCount != 0)
            {
                if (parent.transform.GetChild(0).GetComponent<ComboData>().value == inputKey)
                {
                    StartCoroutine("RemoveCombo");

                }
                else if (parent.transform.GetChild(0).GetComponent<ComboData>().value != inputKey && Input.anyKeyDown == true)
                {
                    if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 체크
                    {
                        yield return null;
                    }
                    else if (Input.GetMouseButtonDown(1)) // 마우스 오른쪽 버튼 체크
                    {
                        yield return null;
                    }
                    else
                    StartCoroutine("Fail");
                }
            }
            inputKey = 0;
            yield return null;
        }
    }

    IEnumerator RemoveCombo()
    {
        Destroy(parent.transform.GetChild(0).gameObject);
        parent.transform.GetChild(0).parent = null;
        if (parent.transform.childCount == 0) //전부 성공
        {
            targetCanvas.SetActive(false);
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Destroy(parent.transform.GetChild(i).gameObject);
            }
            isSuccess = true;
            comboKey.Clear();
            MiniGameFinished?.Invoke(isSuccess);
            failCount = 0;
            StopAllCoroutines();
        }
        else
        parent.transform.GetChild(0).GetComponent<Image>().color = Color.red;
        yield return null ;
    }

    IEnumerator Fail()
    {
        failCount++;
        if(failCount == 5)
        {
            targetCanvas.SetActive(false);
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Destroy(parent.transform.GetChild(i).gameObject);
            }
            comboKey.Clear();
            StopAllCoroutines();
            failCount = 0;
            isSuccess= false;
            MiniGameFinished?.Invoke(isSuccess);
        }
        ReMake();
        fail.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.3f);
        fail.gameObject.SetActive(false);
    }

    IEnumerator TimeOut()
    {
        failTime = 10f;
        while (true)
        {
          failTime -= Time.deltaTime; //시간체크
          timeGauge.value = failTime; //시간체크
          if (timeGauge.value == 0)
              {
            failCount = 4;
            StartCoroutine("Fail");
              }
             yield return null;

        }
      
    }
}
