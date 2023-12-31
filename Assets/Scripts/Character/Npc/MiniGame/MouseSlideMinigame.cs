using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MouseSlideMinigame : MonoBehaviour
{
    public bool isClick;
    public Image fadeImage;
    public GameObject parent;
    public GameObject mouseEffect;
    private Vector3 startpos;
    private Vector3 endpos;
    private List<int> sliceList;
    private Color c;
    private bool isFade = true;
    [SerializeField]private bool isAllFade;
    private bool isSuccess;
    public event Action<bool> MiniGameFinished;
    public static MouseSlideMinigame Instance;
    private void Awake()
    {
        Instance = this;
        sliceList = new List<int>();
        parent.SetActive(false);
        mouseEffect.SetActive(false);
    }
    private void Start()
    {
        c = fadeImage.GetComponent<Image>().color;
    }
    private void Update()
    {
        if (parent.activeSelf&& sliceList.Count >0&&isAllFade)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startpos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                endpos = Input.mousePosition;
                if(startpos != Vector3.zero || endpos != Vector3.zero)
                {
                    CheckDir();
                    if (CheckDir() == sliceList[0])
                    {
                        StartCoroutine("Success");
                    }
                    else
                    {
                        StartCoroutine("Fail");
                    }
                }
            }
        }

    }
    private int CheckDir()
    {
        Vector2 direction = (endpos - startpos).normalized;
        float angle = Vector2.Angle(Vector2.right, direction);

        if (angle < 20f || angle > 160f)
        {
            return 0; //수평
        }
        else if (angle > 80f && angle < 100f)
        {
            return 1; //수직
        }
        else
        {
            return 2; //대각
        }
    }

    IEnumerator StartMission()
    {
        sliceList.Clear();
        parent.SetActive(true);
        mouseEffect.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        for (int i = 0; i < 4; i++)
        {
            int temp = Random.Range(0, 3);
            sliceList.Add(temp);
        }
            for (int i = 0; i < 4; i++)
        {
            switch (sliceList[i])
            {
                case 0:
                    fadeImage.transform.rotation = Quaternion.Euler(0, 0, 0);
                    StartCoroutine("Fade");
                    break;
                case 1:
                    fadeImage.transform.rotation = Quaternion.Euler(0, 0, 90);
                    StartCoroutine("Fade");
                    break;
                case 2:
                    fadeImage.transform.rotation = Quaternion.Euler(0, 0, 45);
                    StartCoroutine("Fade");
                    break;
            }
            yield return new WaitUntil(() => isFade);
        }
        isAllFade = true;
    yield return null;
    }

    IEnumerator Success()
    {
        sliceList.RemoveAt(0);
        if (sliceList.Count == 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            isSuccess = true;
            ResetField();
            MiniGameFinished?.Invoke(isSuccess);
            StopAllCoroutines();
        }
        return null;
    }
    IEnumerator Fail() //실패해서 처음부터
    {
        isSuccess = false;

        ResetField();
        MiniGameFinished?.Invoke(isSuccess);
        StopCoroutine("Fade");
        StopCoroutine("StartMission");
        return null;
    }

    IEnumerator Fade()
    {
        isFade = false;
        for (float i = 0; i <= 1; i += 0.01f)
        {
            c.a = i;
            fadeImage.color = c;
            yield return new WaitForSecondsRealtime(0.0005f);
        }
        c.a = 0;
        fadeImage.color = c;
        isFade = true;
        yield return null;
    }

    private void ResetField()
    {
        parent.SetActive(false);
        mouseEffect.SetActive(false);
        isFade = true;
        isAllFade = false;
        startpos = Vector3.zero; endpos = Vector3.zero;
    }
    public bool GetSuccess()
    {
        return isSuccess;
    }
}
