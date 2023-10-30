using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NpcAi : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject want;
    public GameObject nightPosition;
    [SerializeField]
    float range = 1; //움직일 반경
    [SerializeField]
    float time; // 새로운 경로 탐색 쿨타임
    private bool isRunning = false;
    public bool testbool; //나중에 밤낮 가져오기 getreturun
    Vector3 point;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        StartCoroutine("NpcMove");
    }

    IEnumerator NpcMove() //이동시작, 밤낮에 따라 경로 달라짐 밤은 고정경로
    {
        while (true)
        {
            if (!testbool)
            {
                isRunning = false;
                yield return new WaitForSecondsRealtime(1f);
            }

            if (!isRunning)
            {
                isRunning = true;
                StartCoroutine(StartMoving());
            }

            yield return null ;
        }
    }
    IEnumerator StartMoving()
    {
        while (true)
        {
            if (!testbool)
                break;

            if (RandomPoint(want.transform.position, range, out point))
            {
                want.transform.position = point;
            }
            agent.SetDestination(want.transform.position);
            yield return new WaitForSecondsRealtime(time);
        }
        agent.SetDestination(nightPosition.transform.position);
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result) //랜덤 경로탐색
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

}
