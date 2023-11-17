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
            StartCoroutine("Fail"); //����
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
                if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư üũ
                {
                    yield return null;
                }
                if (Input.GetMouseButtonDown(1)) // ���콺 ������ ��ư üũ
                {
                    yield return null;
                }
                if (kamen.Instance.inputKey == inputkey)
                {
                    if (rect.sizeDelta.x < 110 && rect.sizeDelta.x > 90)//����
                    {
                        //�÷��̾� �˼� �ִϸ��̼�
                        gameObject.SetActive(false);
                    }
                    if (rect.sizeDelta.x > 110) // Ÿ�ֽ̹���
                    {
                        kamen.Instance.MarkFail();
                        StartCoroutine("Fail");
                    }
                    if (rect.sizeDelta.x < 90) // Ÿ�ֽ̹���
                    {
                        kamen.Instance.MarkFail();
                        StartCoroutine("Fail");
                    }
                }
                if(kamen.Instance.inputKey != inputkey&& kamen.Instance.inputKey != 0) //��ư ����
                {
                    kamen.Instance.MarkFail();
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
