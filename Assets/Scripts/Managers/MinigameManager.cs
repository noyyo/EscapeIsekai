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

    public event Action<bool> MiniGameSuccess; //신경안써도됨
     int sucecesOrFail;
    public event Action<int> ChangeSuccess; //미니게임 성공여부 확인 이벤트 구문 맨아래쪽 예시 구문 적어둠
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
    /// 1.카멘
    /// 2.마우스슬라이더
    /// 3.방향키입력
    /// 4.게이지맞추기
    /// </summary>
    public IEnumerator StartMissionCoroutine(int index)
    {
        sucecesOrFail = 0;
        if (index == 1)
            _kamen.GetComponent<kamen>().StartCoroutine("StartMission");
        else if (index == 2)
            _mouseSlider.GetComponent<MouseSlider>().StartCoroutine("StartMission");
        else if (index == 3)
            _instructor.GetComponent<instructor>().StartCoroutine("StartMission");
        else if (index == 4)
            _blackSmith.GetComponent<BlackSmith>().StartCoroutine("StartMission");

        // 미니게임이 종료될 때까지 대기
        yield return new WaitUntil(() => sucecesOrFail != 0);
        ChangeSuccess?.Invoke(sucecesOrFail);
      
    }


    /*void testsuc()
    {
     성공시 실행할 메서드
    }
    void testfali()
    {
       실패시 실행할 메서드
      MinigameManager.Instance.ChangeSuccess -=test;
    }

    
    참거짓 여부를 위해 CK해야하는부분은  MinigameManager.Instance.ChangeSuccess +=test;
                                         StartCoroutine(MinigameManager.Instance.StartMissionCoroutine(3));

    후에 메서드가 실행되면 다시 빼주셔야합니다 => MinigameManager.Instance.ChangeSuccess -=test;

    void test(int val) //이벤트를 받기위한 메서드
    {
        if (val == 1)
            testsuc();
        if (val == -1)
            testfali();
    }*/

}
