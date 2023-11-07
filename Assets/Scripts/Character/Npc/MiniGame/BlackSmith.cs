using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackSmith : MonoBehaviour
{
    public GameObject parent;
    public Image target;
    public Image point;
    public Slider timeGauge;

    [Tooltip("제한시간")]
    [SerializeField]
    private float time;
    bool isRunning;
    RaycastHit hit;
    private bool isSuccess;

    private void Awake()
    {
        parent.SetActive(false);
    }
    private void Start()
    {
     //   StartCoroutine("StarMission");
    }
    private void Update()
    {
        if (isRunning)
        {
            time -= Time.deltaTime;
            timeGauge.value = time;
        }
    }
    private void FixedUpdate()
    {
        if (isRunning) 
        {
            point.GetComponent<Rigidbody>().AddForce(Vector2.down * 50f);

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (point.GetComponent<RectTransform>().anchoredPosition.y > 340)
                {
                    point.GetComponent<Rigidbody>().velocity = Vector2.zero;
                }
                point.GetComponent<Rigidbody>().AddForce(Vector2.up * 5000f);
            }

            if (Physics.Raycast(point.transform.position, transform.right, out hit, 300))
            {
                if (timeGauge.value == 0&& hit.transform.name == target.name)
                {
                        Debug.Log("성공");
                    isSuccess = true;
                        isRunning = false;
                        parent.SetActive(isRunning);
                }
            }
            else
            {
                if(timeGauge.value == 0)
                {
                    Debug.Log("실패");
                    isRunning = false;
                    parent.SetActive(isRunning);
                }
              
            }
        }
    }

    IEnumerator StarMission()
    {
        time = 10f;
        target.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Random.Range(-300, 300));
        isRunning = true;
        parent.SetActive(isRunning);
        yield return null;
    }
    public bool GetSuccess()
    {
        return isSuccess;
    }
}
