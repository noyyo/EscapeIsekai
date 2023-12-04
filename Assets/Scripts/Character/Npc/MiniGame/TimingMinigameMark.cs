using System.Collections;
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
        rect = gameObject.GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (rect.sizeDelta.x < 30)
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
            if (Input.anyKeyDown)
            {
                if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư üũ
                {
                    yield return null;
                }
                if (Input.GetMouseButtonDown(1)) // ���콺 ������ ��ư üũ
                {
                    yield return null;
                }
                if (TimingMinigame.Instance.inputKey == inputkey)
                {
                    if (rect.sizeDelta.x < 110 && rect.sizeDelta.x > 90)//����
                    {
                        //�÷��̾� �˼� �ִϸ��̼�
                        gameObject.SetActive(false);
                    }
                    if (rect.sizeDelta.x > 110) // Ÿ�ֽ̹���
                    {
                        TimingMinigame.Instance.MarkFail();
                        StartCoroutine("Fail");
                    }
                    if (rect.sizeDelta.x < 90) // Ÿ�ֽ̹���
                    {
                        TimingMinigame.Instance.MarkFail();
                        StartCoroutine("Fail");
                    }
                }
                if (TimingMinigame.Instance.inputKey != inputkey && TimingMinigame.Instance.inputKey != 0) //��ư ����
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

        inputkey = Random.Range(61, 64);
        arrowIcon.sprite = arrows[inputkey - 61];
    }
}
