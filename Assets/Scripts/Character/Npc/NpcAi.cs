using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NpcAi : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject dayPosition;
    public GameObject nightPosition;
    [SerializeField]
    float range = 2; //������ �ݰ�
    [SerializeField]
    float time; // ���ο� ��� Ž�� ��Ÿ��
    private bool isRunning = false;
    Vector3 point;
    Vector3 selfPoint;
    private Animator animator;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        selfPoint = gameObject.transform.position;
        StartCoroutine("NpcMove");
    }
    IEnumerator NpcMove() //�̵�����, �㳷�� ���� ��� �޶��� ���� �������
    {
        while (true)
        {
            if (agent.velocity != Vector3.zero)
            {
                if (animator != null)
                {
                    animator.SetBool("Walk", true);
                }
            }
            if (agent.velocity == Vector3.zero)
            {
                if (animator != null)
                {
                    animator.SetBool("Walk", false);
                }
            }
            if (!GameManager.Instance.IsDay) //���϶�
            {
                isRunning = false;
                yield return new WaitForSecondsRealtime(1f);
            }

            if (!isRunning)
            {
                isRunning = true;
                StartCoroutine(StartMoving());
            }
            yield return null;
        }
    }
    IEnumerator StartMoving()
    {
        while (true)
        {
            if (!GameManager.Instance.IsDay) //��
                break;

            if (dayPosition == null)
            {
                RandomPoint(selfPoint, range, out point);
                agent.SetDestination(point);
                float tmptime = Random.Range(1, 5);
                yield return new WaitForSecondsRealtime(tmptime);
            }
            else if (RandomPoint(dayPosition.transform.position, range, out point) && dayPosition != null)
            {
                dayPosition.transform.position = point;
                agent.SetDestination(dayPosition.transform.position);
                yield return new WaitForSecondsRealtime(time);
            }
        }
        if (nightPosition == null)
        {
            yield break;
        }
        agent.SetDestination(nightPosition.transform.position);
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result) //���� ���Ž��
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
