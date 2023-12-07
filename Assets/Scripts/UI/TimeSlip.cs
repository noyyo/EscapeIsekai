using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
    private GameObject obj;
    public GameObject tent;
    private void Awake()
    {
        action = GameManager.Instance.Player.GetComponent<Player>();
        animator = GetComponentInChildren<Animator>();
        ShowBtn();
        panel.SetActive(false);
        text.gameObject.SetActive(false);
    }
    private void Start()
    {
        action.Input.PlayerActions.TimeSlip.started += OnTimeSlip;
        npcs = GameObject.FindGameObjectsWithTag("Npc");
    }
    public IEnumerator ShowString()
    {
        GameManager.Instance.Player.GetComponent<PlayerInputSystem>().InputActions.Disable();
        panel.SetActive(true);
        text.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        for (int i = 0; i <= originalText.Length; i++)
        {
            SoundManager.Instance.CallPlaySFX(ClipType.UISFX, "ButtonSound", this.transform, false);
            text.text = originalText.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }
        ShowBtn();
    }

    void ShowBtn()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            if (btns[i].gameObject.activeSelf == true)
                btns[i].gameObject.SetActive(false);
            else
                btns[i].gameObject.SetActive(true);
        }
    }

    public void BtnAction()
    {
        if (EventSystem.current.currentSelectedGameObject.name == btns[0].name)
        {
            SoundManager.Instance.CallPlaySFX(ClipType.UISFX, "Click", this.transform, false);
            GameManager.Instance.time = 0.2f;
            StartCoroutine("Fade");
        }
        if (EventSystem.current.currentSelectedGameObject.name == btns[1].name)
        {
            SoundManager.Instance.CallPlaySFX(ClipType.UISFX, "Click", this.transform, false);
            GameManager.Instance.time = 0.5f;
            StartCoroutine("Fade");
        }
        if (EventSystem.current.currentSelectedGameObject.name == btns[2].name)
        {
            SoundManager.Instance.CallPlaySFX(ClipType.UISFX, "Click", this.transform, false);
            GameManager.Instance.time = 0.8f;
            StartCoroutine("Fade");
        }
    }
    IEnumerator Fade()
    {
        text.gameObject.SetActive(false);
        ShowBtn();
        Color c = panel.GetComponent<Image>().color;
        SoundManager.Instance.CallPlaySFX(ClipType.NPCSFX, "TimeSlip", this.transform, false);
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
        if (obj != null)
        {
            Destroy(obj);
        }
        SoundManager.Instance.CallPlaySFX(ClipType.NPCSFX, "TimeSlipLastSound", this.transform, false);
        yield return null;
    }

    void OnTimeSlip(InputAction.CallbackContext context)
    {
        StartCoroutine("ShowString");
        obj = Instantiate(tent, GameManager.Instance.Player.transform.position, Quaternion.identity);
    }
    public void NpcAction()
    {
        StartCoroutine("ShowString");
    }

    void MoveNpc()
    {
        if (GameManager.Instance.IsDay)
        {
            foreach (GameObject npc in npcs)
            {
                if (npc.GetComponent<NpcAi>().dayPosition != null)
                {
                    npc.GetComponent<NavMeshAgent>().enabled=false;
                    npc.GetComponent<NpcAi>().enabled = false;
                    // npc.GetComponent<NavMeshAgent>().SetDestination(npc.gameObject.transform.position);
                    npc.transform.position = npc.GetComponent<NpcAi>().dayPosition.transform.position;
                    npc.GetComponent<NavMeshAgent>().enabled = true;
                    npc.GetComponent<NpcAi>().enabled = true;
                }
            }
        }
        else
        {
            foreach (GameObject npc in npcs)
            {
                if (npc.GetComponent<NpcAi>().nightPosition != null)
                {
                    npc.GetComponent<NavMeshAgent>().enabled = false;
                    npc.GetComponent<NpcAi>().enabled = false;
                    //  npc.GetComponent<NavMeshAgent>().SetDestination(npc.gameObject.transform.position);
                    npc.transform.position = npc.GetComponent<NpcAi>().nightPosition.transform.position;
                    npc.GetComponent<NavMeshAgent>().enabled = true;
                    npc.GetComponent<NpcAi>().enabled = true;
                }
            }
        }
    }
}
