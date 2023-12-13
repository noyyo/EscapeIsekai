using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : CustomSingleton<GameManager>
{
    protected GameManager() { }

    private UI_Manager uiManager;
    private SoundManager soundManager;
    private GameObject panel;
    private PlayerInputSystem playerInputSystem;

    [SerializeField][ReadOnly] private GameObject player;
    [SerializeField][ReadOnly] private CinemachineVirtualCamera characterCamera;

    //초기화 순서에 따른 문제 또는 Scene이동, 의도치 않은 Player 삭제를 위한 안전장치
    public GameObject Player
    {
        get
        {
            if (player == null)
                PlayerInit();
            return player;
        }
    }
    public UI_Manager Ui_Manager { get { return uiManager; } }

    [Range(0.0f, 1.0f)] public float time; //하루 사이클 시간  0.2~0.8 해떠있는 시간
    public bool IsDay;

    [HideInInspector] public GameObject dialogCamera;
    [HideInInspector] public GameObject timeSlip;
    [HideInInspector] public GameObject deadNpc;
    [HideInInspector] public GameObject endPotal;

    public event Action OnPauseEvent;
    public event Action OnUnpauseEvent;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
        uiManager = UI_Manager.Instance;
        PlayerInit();
        
        if (timeSlip == null)
        {
            timeSlip = Instantiate(Resources.Load<GameObject>("Prefabs/UI/TimeSlip"));
            panel = timeSlip.GetComponent<TimeSlip>().panel;
        }

        if (dialogCamera == null)
        {
            dialogCamera = player.GetComponentInChildren<Camera>().gameObject;
            dialogCamera.SetActive(false);
        }

        if (characterCamera == null)
            characterCamera = GameObject.FindGameObjectWithTag("CharacterCamera").GetComponent<CinemachineVirtualCamera>();

        if (deadNpc == null)
            deadNpc = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/여관주인"));
        if (endPotal == null)
        {
            endPotal = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/차원문"));
            endPotal.name = "차원문";
            endPotal.SetActive(false);
        }
        CursorEnable();
    }

    private void Start()
    {
        uiManager.UI_InventoryTurnOnEvent += PlayInputSystemDisable;
        uiManager.UI_InventoryTurnOnEvent += CursorEnable;
        uiManager.UI_InventoryTurnOnEvent += CallOnPauseEvent;
        uiManager.UI_InventoryTurnOnEvent += CameraLock;

        uiManager.UI_InventoryTurnOffEvent += PlayInputSystemEnable;
        uiManager.UI_InventoryTurnOffEvent += CursorDisable;
        uiManager.UI_InventoryTurnOffEvent += CallOnUnpauseEvent;
        uiManager.UI_InventoryTurnOffEvent += CameraUnLock;

        uiManager.UI_ItemCraftingTurnOnEvent += CursorEnable;
        uiManager.UI_ItemCraftingTurnOnEvent += PlayInputSystemDisable;
        uiManager.UI_ItemCraftingTurnOnEvent += CallOnPauseEvent;
        uiManager.UI_ItemCraftingTurnOnEvent += CameraLock;

        uiManager.UI_ItemCraftingTurnOffEvent += CursorDisable;
        uiManager.UI_ItemCraftingTurnOffEvent += PlayInputSystemEnable;
        uiManager.UI_ItemCraftingTurnOffEvent += CallOnUnpauseEvent;
        uiManager.UI_ItemCraftingTurnOffEvent += CameraUnLock;

        uiManager.UI_TradingTurnOnEvent += CursorEnable;
        uiManager.UI_TradingTurnOnEvent += PlayInputSystemDisable;
        uiManager.UI_TradingTurnOnEvent += CallOnPauseEvent;
        uiManager.UI_TradingTurnOnEvent += CameraLock;

        uiManager.UI_TradingTurnOffEvent += CursorDisable;
        uiManager.UI_TradingTurnOffEvent += PlayInputSystemEnable;
        uiManager.UI_TradingTurnOffEvent += CallOnUnpauseEvent;
        uiManager.UI_TradingTurnOffEvent += CameraUnLock;

        uiManager.UI_OptionTurnOnEvent += CursorEnable;
        uiManager.UI_OptionTurnOnEvent += PlayInputSystemDisable;
        uiManager.UI_OptionTurnOnEvent += CallOnPauseEvent;
        uiManager.UI_OptionTurnOnEvent += CameraLock;

        uiManager.UI_OptionTurnOffEvent += CursorDisable;
        uiManager.UI_OptionTurnOffEvent += PlayInputSystemEnable;
        uiManager.UI_OptionTurnOffEvent += CallOnUnpauseEvent;
        uiManager.UI_OptionTurnOffEvent += CameraUnLock;
        Player.GetComponent<Player>().StateMachine.OnDie += DieEvent;
    }

    private void PlayerInit()
    {
        //다른 오브젝트에 Player태그가 설정되어가 있을경우 걸러내기 위한 foreach문
        player = GameObject.FindGameObjectWithTag(TagsAndLayers.PlayerTag);
        if (player == null)
            player = Instantiate(Resources.Load<GameObject>("Prefabs/Player/Player"));

        playerInputSystem = player.GetComponent<PlayerInputSystem>();
    }

    private void CallOnPauseEvent()
    {
        Time.timeScale = 0f; //임시
        OnPauseEvent?.Invoke();
    }

    private void CallOnUnpauseEvent()
    {
        Time.timeScale = 1f; //임시
        OnUnpauseEvent?.Invoke();
    }

    private void PlayInputSystemEnable()
    {
        playerInputSystem.PlayerActions.Enable();
    }

    private void PlayInputSystemDisable()
    {
        playerInputSystem.PlayerActions.Disable();
    }

    private void CursorEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CursorDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void CameraLock()
    {
        characterCamera.enabled = false;
    }

    private void CameraUnLock()
    {
        characterCamera.enabled = true;
    }

    public void DieEvent()
    {
        deadNpc.SetActive(true);
        deadNpc.transform.position = Player.transform.position + Player.transform.forward * 2;
        soundManager.CallPlaySFXReturnSource(ClipType.NPCSFX, "DeadNpc", this.transform, false, 1f);
        Dialog.Instance.Action(deadNpc);
    }
    public IEnumerator Revive()
    {
        Image panelImage = panel.GetComponent<Image>();
        Player playerScript = player.GetComponent<Player>();
        Playerconditions playerConditions = playerScript.Playerconditions;
        Color c = panelImage.color;
        c.a = 0.0f;
        panel.SetActive(true);
        for (float i = panelImage.color.a; i < 1.1; i += 0.01f)
        {
            c.a = i;
            panelImage.color = c;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        yield return new WaitForSecondsRealtime(2f);
        GameObject[] pub = GameObject.FindGameObjectsWithTag("Npc");
        for (int i = 0; i < pub.Length; i++)
        {
            if (pub[i].GetComponent<Npc>().id == 400)
            {
                Player.transform.position = pub[i].transform.position;
            }
        }
        for (float i = 1; i >= 0; i -= 0.05f)
        {
            c.a = i;
            panelImage.color = c;
            yield return new WaitForSecondsRealtime(0.005f);
        }
        c.a = 0.8f;
        panelImage.color = c;
        panel.SetActive(false);
        playerScript.enabled = true;
        playerScript.Controller.enabled = true;
        playerScript.Collider.enabled = true;
        playerConditions.Heal(playerConditions.health.startValue);
        playerConditions.Heal(playerConditions.stamina.startValue);
        playerConditions.Heal(playerConditions.hunger.startValue);
        playerScript.StateMachine.ChangeState(playerScript.StateMachine.IdleState);
        soundManager.PlayDefaultBGM();
        yield return null;
    }
}
