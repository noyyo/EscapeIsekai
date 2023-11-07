using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class kamen_Mark : MonoBehaviour
{
    public Sprite[] arrows = new Sprite[4];
    public Image arrowIcon;
    private Vector2 reSize;
    [SerializeField]
    private int speed;
    private int inputkey;
    private void Awake()
    {
        reSize = new Vector2(200, 200);

    }
    private void Update()
    {
        if(gameObject.GetComponent<RectTransform>().sizeDelta.x < 30)
        {
            StartCoroutine("Fail"); //실패
        }
        reSize.y -= Time.deltaTime * speed;
        reSize.x -= Time.deltaTime * speed;
        gameObject.GetComponent<RectTransform>().sizeDelta = reSize;
    }
    IEnumerator RemoveMark()
    {
        while (true)
        {
            if(Input.anyKeyDown)
            {
                if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 체크
                {
                    yield return null;
                }
                if (Input.GetMouseButtonDown(1)) // 마우스 오른쪽 버튼 체크
                {
                    yield return null;
                }
                if (kamen.Instance.inputKey == inputkey)
                {
                    if (gameObject.GetComponent<RectTransform>().sizeDelta.x < 110 && gameObject.GetComponent<RectTransform>().sizeDelta.x > 90)//성공
                    {
                        //플레이어 검술 애니메이션
                        gameObject.SetActive(false);
                    }
                    if (gameObject.GetComponent<RectTransform>().sizeDelta.x > 110) // 타이밍실패
                    {

                        StartCoroutine("Fail");
                    }
                    if (gameObject.GetComponent<RectTransform>().sizeDelta.x < 90) // 타이밍실패
                    {

                        StartCoroutine("Fail");
                    }
                }
                if(kamen.Instance.inputKey != inputkey&& kamen.Instance.inputKey != 0) //버튼 실패
                {
                    StartCoroutine("Fail");
                }
            }
          
            kamen.Instance.inputKey = 0;
            yield return null;
        }

    }

    IEnumerator Fail()
    {
        kamen.Instance.StopAllCoroutines();
        gameObject.SetActive(false);
        yield return null;
    }
    private void Start()
    {
        StartCoroutine("RemoveMark");

        inputkey=Random.Range(61, 64);
        arrowIcon.sprite = arrows[inputkey-61];
    }
}
