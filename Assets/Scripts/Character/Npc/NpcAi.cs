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
    public GameObject dayPosition;
    public GameObject nightPosition;
    [SerializeField]
    float range = 2; //¿òÁ÷ÀÏ ¹Ý°æ
    [SerializeField]
    float time; // »õ·Î¿î °æ·Î Å½»ö ÄðÅ¸ÀÓ
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
    IEnumerator NpcMove() //ÀÌµ¿½ÃÀÛ, ¹ã³·¿¡ µû¶ó °æ·Î ´Þ¶óÁü ¹ãÀº °íÁ¤°æ·Î
    {
        while (true)
        {
            if(agent.velocity != Vector3.zero)
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
            if (!GameManager.Instance.IsDay) //¹ãÀÏ¶§
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
            if (!GameManager.Instance.IsDay) //¹ã
                break;

            if(dayPosition ==null)
            {
                RandomPoint(selfPoint, range, out point);
                agent.SetDestination(point);
                float tmptime = Random.Range(1, 5);
                yield return new WaitForSecondsRealtime(tmptime);
            }
            else if (RandomPoint(dayPosition.transform.position, range, out point)&& dayPosition !=null)
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
    bool RandomPoint(Vector3 center, float range, out Vector3 result) //·£´ý °æ·ÎÅ½»ö
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
