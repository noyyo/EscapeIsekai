using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    private Rigidbody rb;
    private RectTransform rect;
    public event Action<bool> MiniGameFinished;

    private void Awake()
    {
        parent.SetActive(false);
        rb = point.GetComponent<Rigidbody>();
        rect = point.GetComponent<RectTransform>();
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
            rb.AddForce(Vector2.down * 50f);

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (rect.anchoredPosition.y > 340)
                {
                    rb.velocity = Vector2.zero;
                }
                rb.AddForce(Vector2.up * 5000f);
            }

            if (Physics.Raycast(point.transform.position, transform.right, out hit, 300))
            {
                if (timeGauge.value == 0&& hit.transform.name == target.name)
                {

                    isSuccess = true;
                    MiniGameFinished?.Invoke(isSuccess);
                        isRunning = false;
                        parent.SetActive(isRunning);
                }
            }
            else
            {
                if(timeGauge.value == 0)
                {
                    isRunning = false;
                    MiniGameFinished?.Invoke(isSuccess);
                    parent.SetActive(isRunning);
                }
              
            }
        }
    }

    IEnumerator StartMission()
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
