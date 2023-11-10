using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MouseSlider : MonoBehaviour
{
    public bool isClick;
    public Image fadeImage;

    private Vector3 startpos;
    private Vector3 endpos;
    private List<int> sliceList;
    private Color c;
    private bool isFade = true;
    private bool isSuccess;

    public static MouseSlider Instance;
    private void Awake()
    {
        Instance = this;
        sliceList = new List<int>();
        gameObject.SetActive(false);
    }
    private void Start()
    {
        c = fadeImage.GetComponent<Image>().color;
      // StartCoroutine("StartMission");
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startpos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            endpos = Input.mousePosition;
            CheckDir();
            if (CheckDir() == sliceList[0])
            {
                StartCoroutine("Success") ;
            }
            else
            {
                StartCoroutine("Fail");
            }
        }
       
    }
    private int CheckDir()
    {
       
        Vector2 direction = (endpos - startpos).normalized;  
        float angle = Vector2.Angle(Vector2.right, direction);

        if (angle < 20f || angle > 160f)
        {
            return 0; //����
        }
        else if (angle > 80f && angle < 100f)
        {
            return 1; //����
        }
        else
        {
            return 2; //�밢
        }
    }

    IEnumerator StartMission()
    {
        Cursor.lockState = CursorLockMode.Confined;
        for (int i = 0; i < 4; i++)
        {
            int temp = Random.Range(0, 3);
            sliceList.Add(temp);
            switch (temp) 
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
        yield return null;
    }

    IEnumerator Success() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        sliceList.RemoveAt(0);
        Debug.Log("����");
        if(sliceList.Count == 0)
        {
            isSuccess = true;
            StopAllCoroutines();
        }
        return null; 
    }
    IEnumerator Fail() //�����ؼ� ó������
    {
        sliceList.Clear();
        StartCoroutine("StartMission");
        return null;
    }

    IEnumerator Fade()
    {
        isFade = false;
        for (float i = 0; i <= 1; i+=0.01f)
        {
            c.a = i;
            fadeImage.color = c;
            yield return new WaitForSecondsRealtime (0.01f);
        }
        c.a = 0;
        fadeImage.color = c;
        isFade = true;
        yield return null;
    }

    public bool GetSuccess()
    {
        return isSuccess;
    }
}
