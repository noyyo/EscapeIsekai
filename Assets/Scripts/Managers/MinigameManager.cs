using System;
using System.Collections;
using UnityEngine;

public class MinigameManager : CustomSingleton<MinigameManager>
{
    protected MinigameManager() { }
    private GameObject _kamen;
    private GameObject _mouseSlider;
    private GameObject _instructor;
    private GameObject _blackSmith;

    //public event Action<bool> MiniGameSuccess; //�Ű�Ƚᵵ��
    int sucecesOrFail;
    public event Action<int> ChangeSuccess; //�̴ϰ��� �������� Ȯ�� �̺�Ʈ ���� �ǾƷ��� ���� ���� �����
    public GameObject kamen { get { return _kamen; } }
    public GameObject MouseSlider { get { return _mouseSlider; } }
    public GameObject instructor { get { return _instructor; } }
    public GameObject BlackSmith { get { return _blackSmith; } }

    private void Awake()
    {
        if (_kamen == null)
        {
            _kamen = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/MiniGame/kamen"));
            _kamen.GetComponent<TimingMinigame>().MiniGameFinished += OnMiniGameFinished;
        }
        if (_mouseSlider == null)
        {
            _mouseSlider = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/MiniGame/MouseSlider"));
            _mouseSlider.GetComponent<MouseSlideMinigame>().MiniGameFinished += OnMiniGameFinished;
        }
        if (_instructor == null)
        {
            _instructor = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/MiniGame/instructor"));
            _instructor.GetComponent<ArrowMinigame>().MiniGameFinished += OnMiniGameFinished;
        }
        if (_blackSmith == null)
        {
            _blackSmith = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/MiniGame/BlackSmith"));
            _blackSmith.GetComponent<GaugeMinigame>().MiniGameFinished += OnMiniGameFinished;
        }
    }
    public void OnMiniGameFinished(bool success)
    {
        if (success)
            sucecesOrFail = 1;
        if (!success)
            sucecesOrFail = -1;
    }

    /// <summary>
    /// 1.ī��
    /// 2.���콺�����̴�
    /// 3.����Ű�Է�
    /// 4.���������߱�
    /// </summary>
    public IEnumerator StartMissionCoroutine(int index)
    {
        sucecesOrFail = 0;
        if (index == 1)
            _kamen.GetComponent<TimingMinigame>().StartCoroutine("StartMission");
        else if (index == 2)
            _mouseSlider.GetComponent<MouseSlideMinigame>().StartCoroutine("StartMission");
        else if (index == 3)
            _instructor.GetComponent<ArrowMinigame>().StartCoroutine("StartMission");
        else if (index == 4)
            _blackSmith.GetComponent<GaugeMinigame>().StartCoroutine("StartMission");

        // �̴ϰ����� ����� ������ ���
        yield return new WaitUntil(() => sucecesOrFail != 0);
        ChangeSuccess?.Invoke(sucecesOrFail);

    }


    /*void testsuc()
    {
     ������ ������ �޼���
    }
    void testfali()
    {
       ���н� ������ �޼���
      MinigameManager.Instance.ChangeSuccess -=test;
    }

    
    ������ ���θ� ���� CK�ؾ��ϴºκ���  MinigameManager.Instance.ChangeSuccess +=test;
                                         StartCoroutine(MinigameManager.Instance.StartMissionCoroutine(3));

    �Ŀ� �޼��尡 ����Ǹ� �ٽ� ���ּž��մϴ� => MinigameManager.Instance.ChangeSuccess -=test;

    void test(int val) //�̺�Ʈ�� �ޱ����� �޼���
    {
        if (val == 1)
            testsuc();
        if (val == -1)
            testfali();
    }*/

}
