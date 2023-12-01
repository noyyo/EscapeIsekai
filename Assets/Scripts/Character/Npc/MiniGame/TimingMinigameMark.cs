using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class TimingMinigameMark : MonoBehaviour
{
    public Sprite[] arrows = new Sprite[4];
    public Image arrowIcon;
    private Vector2 reSize;
    [SerializeField]
    private int speed;
    private int inputkey;
    private RectTransform rect;
    private void Awake()
    {
        reSize = new Vector2(200, 200);
        rect= gameObject.GetComponent<RectTransform>();
    }
    private void Update()
    {
        if(rect.sizeDelta.x < 30)
        {
            StartCoroutine("Fail"); //실패
        }
        reSize.y -= Time.deltaTime * speed;
        reSize.x -= Time.deltaTime * speed;
        rect.sizeDelta = reSize;
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
                if (TimingMinigame.Instance.inputKey == inputkey)
                {
                    if (rect.sizeDelta.x < 110 && rect.sizeDelta.x > 90)//성공
                    {
                        //플레이어 검술 애니메이션
                        gameObject.SetActive(false);
                    }
                    if (rect.sizeDelta.x > 110) // 타이밍실패
                    {
                        TimingMinigame.Instance.MarkFail();
                        StartCoroutine("Fail");
                    }
                    if (rect.sizeDelta.x < 90) // 타이밍실패
                    {
                        TimingMinigame.Instance.MarkFail();
                        StartCoroutine("Fail");
                    }
                }
                if(TimingMinigame.Instance.inputKey != inputkey&& TimingMinigame.Instance.inputKey != 0) //버튼 실패
                {
                    TimingMinigame.Instance.MarkFail();
                    StartCoroutine("Fail");
                }
            }
          
            TimingMinigame.Instance.inputKey = 0;
            yield return null;
        }

    }

    IEnumerator Fail()
    {
        TimingMinigame.Instance.StopAllCoroutines();
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
