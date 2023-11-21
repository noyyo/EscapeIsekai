using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class TimeSlip : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI text;
    public Button[] btns;
    [SerializeField]
    private Animator animator;
    private Player action;
    private string originalText = "얼마나 시간을 보낼까?";
    private GameObject[] npcs;

    private void Awake()
    {
        action = GameManager.Instance.Player.GetComponent<Player>();
        action.Input.PlayerActions.TimeSlip.started += OnTimeSlip;
        animator = GetComponentInChildren<Animator>();
        ShowBtn();
        panel.SetActive(false);
        text.gameObject.SetActive(false);
    }
    private void Start()
    {
        npcs = GameObject.FindGameObjectsWithTag("Npc");

    }
    IEnumerator ShowString()
    {
        panel.SetActive(true);
        text.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        for (int i = 0; i <= originalText.Length; i++)
        {
            text.text = originalText.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }
        ShowBtn();
    }

    void ShowBtn()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            if (btns[i].gameObject.activeSelf==true)
            btns[i].gameObject.SetActive(false);
            else
                btns[i].gameObject.SetActive(true);
        }
    }

    public void BtnAction()
    {
        if (EventSystem.current.currentSelectedGameObject.name == btns[0].name)
        {
            GameManager.Instance.time = 0.2f;
            StartCoroutine("Fade");
        }
        if (EventSystem.current.currentSelectedGameObject.name == btns[1].name)
        {
            GameManager.Instance.time = 0.5f;
            StartCoroutine("Fade");
        }
        if (EventSystem.current.currentSelectedGameObject.name == btns[2].name)
        {
            GameManager.Instance.time = 0.8f;
            StartCoroutine("Fade");
        }
    }
    IEnumerator Fade()
    {
        text.gameObject.SetActive(false);
        ShowBtn();
        Color c = panel.GetComponent<Image>().color;
        for (float i = panel.GetComponent<Image>().color.a; i < 1.1; i += 0.01f)
        {
            c.a = i;
            panel.GetComponent<Image>().color = c;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        animator.SetBool("Turn", true);
        yield return new WaitForSecondsRealtime(2f);
        MoveNpc();
        for (float i = 1; i >= 0; i -= 0.05f)
        {
            c.a = i;
            panel.GetComponent<Image>().color = c;
            yield return new WaitForSecondsRealtime(0.005f);
        }
        c.a = 0.8f;
        panel.GetComponent<Image>().color = c;
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.Player.GetComponent<PlayerInputSystem>().InputActions.Enable();
        yield return null;
    }

    void OnTimeSlip(InputAction.CallbackContext context)
    {
        GameManager.Instance.Player.GetComponent<PlayerInputSystem>().InputActions.Disable();
        StartCoroutine("ShowString");
    }

    void MoveNpc()
    {
        if(GameManager.Instance.IsDay)
        {
            foreach(GameObject npc in npcs)
            {
                if(npc.GetComponent<NpcAi>().dayPosition != null)
                {
                    npc.GetComponent<NavMeshAgent>().SetDestination(npc.gameObject.transform.position);
                    npc.transform.position = npc.GetComponent<NpcAi>().dayPosition.transform.position;
                }
            }
        }
        else
        {
            foreach (GameObject npc in npcs)
            {
                if (npc.GetComponent<NpcAi>().nightPosition != null)
                {
                    npc.GetComponent<NavMeshAgent>().SetDestination(npc.gameObject.transform.position);
                    npc.transform.position = npc.GetComponent<NpcAi>().nightPosition.transform.position;
                }
            }
        }
    }
}
