using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : CustomSingleton<GameManager>
{
    protected GameManager() { }
    [SerializeField] private GameObject _player;
    [Range(0.0f, 1.0f)]
    public float time; //하루 사이클 시간  0.2~0.8 해떠있는 시간
    public bool IsDay;
    public GameObject dialogCamera;
    private UI_Manager _ui_Manager;
    private SoundManager _soundManager;
    private PlayerInputSystem _playerInputSystem;
    private GameObject _soundManagerObject;
    public GameObject timeSlip;
    public UI_Manager Ui_Manager { get { return _ui_Manager; } }
    public GameObject deadNpc;
    public GameObject endPotal;
    private GameObject panel;
    //초기화 순서에 따른 문제 또는 Scene이동, 의도치 않은 Player 삭제를 위한 안전장치
    public GameObject Player 
    { 
        get 
        {
            if (_player == null)
                PlayerInit();
            return _player; 
        } 
    }

    public event Action OnPauseEvent;
    public event Action OnUnpauseEvent;

    private void Awake()
    {
        if (_soundManagerObject == null)
            _soundManagerObject = Instantiate(Resources.Load<GameObject>("Prefabs/Manager/SoundManager"));
        _soundManager = _soundManagerObject.GetComponent<SoundManager>();
        PlayerInit();
        _ui_Manager = UI_Manager.Instance;
        if (timeSlip == null)
        {
            timeSlip = Instantiate(Resources.Load<GameObject>("Prefabs/UI/TimeSlip"));
             panel = timeSlip.GetComponent<TimeSlip>().panel;
        }

        if(dialogCamera ==null)
        {
            dialogCamera = Player.GetComponentInChildren<Camera>().gameObject;
            dialogCamera.SetActive(false);
        }
        if (deadNpc == null)
        {
            deadNpc = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/여관주인"));
        }
        if (endPotal == null)
        {
            endPotal = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/차원문"));
            endPotal.SetActive(false);
        }
        CursorEnable();
    }

    private void Start()
    {
        _ui_Manager.UI_InventoryTurnOnEvent += CursorEnable;
        _ui_Manager.UI_InventoryTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_InventoryTurnOnEvent += CallOnPauseEvent;

        _ui_Manager.UI_InventoryTurnOffEvent += CursorDisable;
        _ui_Manager.UI_InventoryTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_InventoryTurnOffEvent += CallOnUnpauseEvent;

        _ui_Manager.UI_ItemCraftingTurnOnEvent += CursorEnable;
        _ui_Manager.UI_ItemCraftingTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_ItemCraftingTurnOnEvent += CallOnPauseEvent;

        _ui_Manager.UI_ItemCraftingTurnOffEvent += CursorDisable;
        _ui_Manager.UI_ItemCraftingTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_ItemCraftingTurnOffEvent += CallOnUnpauseEvent;

        _ui_Manager.UI_TradingTurnOnEvent += CursorEnable;
        _ui_Manager.UI_TradingTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_TradingTurnOnEvent += CallOnPauseEvent;

        _ui_Manager.UI_TradingTurnOffEvent += CursorDisable;
        _ui_Manager.UI_TradingTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_TradingTurnOffEvent += CallOnUnpauseEvent;

        _ui_Manager.UI_OptionTurnOnEvent += CursorEnable;
        _ui_Manager.UI_OptionTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_OptionTurnOnEvent += CallOnPauseEvent;

        _ui_Manager.UI_OptionTurnOffEvent += CursorDisable;
        _ui_Manager.UI_OptionTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_OptionTurnOffEvent += CallOnUnpauseEvent;
        Player.GetComponent<Player>().StateMachine.OnDie += DieEvent;
    }

    private void PlayerInit()
    {
        //다른 오브젝트에 Player태그가 설정되어가 있을경우 걸러내기 위한 foreach문
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(TagsAndLayers.PlayerTag);
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.name == "Player")
                _player = gameObject;
        }
        if (_player == null)
            _player = Instantiate(Resources.Load<GameObject>("Prefabs/Player/Player"));

        _playerInputSystem = _player.GetComponent<PlayerInputSystem>();
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
        _playerInputSystem.PlayerActions.Enable();
    }

    private void PlayInputSystemDisable()
    {
        _playerInputSystem.PlayerActions.Disable();
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

    public void DieEvent()
    {
        deadNpc.SetActive(true);
        deadNpc.transform.position = Player.transform.position+ Player.transform.forward*2;
        Dialog.Instance.Action(deadNpc);
    }
   public IEnumerator Revive()
    {
       
        Color c = panel.GetComponent<Image>().color;
        c.a = 0.0f;
        panel.SetActive(true);
        for (float i = panel.GetComponent<Image>().color.a; i < 1.1; i += 0.01f)
        {
            c.a = i;
            panel.GetComponent<Image>().color = c;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        yield return new WaitForSecondsRealtime(2f);
        GameObject[] pub = GameObject.FindGameObjectsWithTag("Npc");
        for(int i = 0; i < pub.Length;i++)
        {
            if (pub[i].GetComponent<Npc>().id == 400)
            {
                Player.transform.position = pub[i].transform.position;
            }
        }
        for (float i = 1; i >= 0; i -= 0.05f)
        {
            c.a = i;
            panel.GetComponent<Image>().color = c;
            yield return new WaitForSecondsRealtime(0.005f);
        }
        c.a = 0.8f;
        panel.GetComponent<Image>().color = c;
        panel.SetActive(false);
        Player.GetComponent<Player>().enabled = true;
        Player.GetComponent<CharacterController>().enabled = true;
        Player.GetComponent<CapsuleCollider>().enabled = true;
        Player.GetComponent<Playerconditions>().Heal(Player.GetComponent<Playerconditions>().health.startValue);
        Player.GetComponent<Playerconditions>().Heal(Player.GetComponent<Playerconditions>().stamina.startValue);
        Player.GetComponent<Playerconditions>().Heal(Player.GetComponent<Playerconditions>().hunger.startValue);
        Player.GetComponent<Player>().StateMachine.ChangeState(Player.GetComponent<Player>().StateMachine.IdleState);
        yield return null;
    }
}
