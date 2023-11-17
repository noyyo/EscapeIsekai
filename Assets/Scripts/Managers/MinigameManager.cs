using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MinigameManager : CustomSingleton<MinigameManager>
{
    protected MinigameManager() { }
    private GameObject _kamen;
    private GameObject _mouseSlider;
    private GameObject _instructor;
    private GameObject _blackSmith;

    public event Action<bool> MiniGameSuccess; //�Ű�Ƚᵵ��
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
            _kamen.GetComponent<kamen>().MiniGameFinished += OnMiniGameFinished;
        }
        if (_mouseSlider == null)
        {
            _mouseSlider = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/MiniGame/MouseSlider"));
            _mouseSlider.GetComponent<MouseSlider>().MiniGameFinished += OnMiniGameFinished;
        }
        if (_instructor == null)
        {
            _instructor = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/MiniGame/instructor"));
            _instructor.GetComponent<instructor>().MiniGameFinished += OnMiniGameFinished;
        }
        if (_blackSmith == null)
        {
            _blackSmith = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/MiniGame/BlackSmith"));
            _blackSmith.GetComponent<BlackSmith>().MiniGameFinished += OnMiniGameFinished;
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

        if (index == 1)
            _kamen.GetComponent<kamen>().StartCoroutine("StartMission");
        else if (index == 2)
            _mouseSlider.GetComponent<MouseSlider>().StartCoroutine("StartMission");
        else if (index == 3)
            _instructor.GetComponent<instructor>().StartCoroutine("StartMission");
        else if (index == 4)
            _blackSmith.GetComponent<BlackSmith>().StartCoroutine("StartMission");

        // �̴ϰ����� ����� ������ ���
        yield return new WaitUntil(() => sucecesOrFail != 0);
        ChangeSuccess?.Invoke(sucecesOrFail);
        sucecesOrFail = 0;
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
